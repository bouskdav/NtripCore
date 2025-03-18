using System.Net;
using System.Text;
using NetCoreServer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Hosting.Internal;
using NtripCore.Caster.Configs;
using NtripCore.Caster.Connections;
using Microsoft.Extensions.DependencyInjection;
using NtripCore.Caster.Core;
using System.Net.Sockets;
using Microsoft.Extensions.Logging;
using NtripCore.Caster.Core.NtripHttp;
using NtripCore.Caster.Core.NtripHttp.Request;
using System.Runtime.CompilerServices;
using NtripCore.Caster.Connections.DataPushers;
using System.Globalization;
using NtripCore.Caster.Core.NMEA;
using NtripCore.Caster.Utility.Sources;
using System.Collections.ObjectModel;
using System.IO;
using System.Runtime.InteropServices;
using NtripCore.Shared.Constants;
using NtripCore.Caster.Connections.Interop;
using NtripCore.Caster.Interfaces;
using System;

namespace NtripCore.Caster
{
    internal class Program
    {
        public static IConfiguration _configuration;
        static IHostEnvironment _hostingEnvironment;
        static ILogger<Program> _logger;

        static async Task Main(string[] args)
        {
            using IHost host = CreateHostBuilder(args).Build();

            _configuration = (IConfiguration)host.Services.GetService(typeof(IConfiguration));

            //_logger = new LoggerConfiguration()
            //    .ReadFrom.Configuration(_configuration)
            //    .Enrich.FromLogContext()
            //    .WriteTo.Console(
            //        outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}"
            //    )
            //    .WriteTo.File(
            //        path: Path.Combine(_hostingEnvironment.ContentRootPath, "logs", $"{DateTime.Today:yyyyMMdd}.log"),
            //        rollingInterval: RollingInterval.Day,
            //        rollOnFileSizeLimit: true,
            //        fileSizeLimitBytes: 10_000_000,
            //        outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}"
            //    )
            //    .CreateLogger();

            _logger = host.Services.GetRequiredService<ILogger<Program>>();

            _logger.LogInformation($"---------------------------");
            _logger.LogInformation($"Started at {DateTime.Now.ToString("dd.MM.yyyy HH:mm")}");
            _logger.LogInformation($"Using root path: {_hostingEnvironment.ContentRootPath}");

            var caster = host.Services.GetRequiredService<NtripCaster>();

            // add clients from config
            NtripHttpClient ntripHttpClient = new NtripHttpClient();

            var preconfiguredSources = _configuration.GetSection("Sources").Get<List<NtripSource>>();

            foreach (NtripSource source in preconfiguredSources)
            {
                caster.AddNtripSource(source);

                if (source.SourceType == SourceType.Local)
                {
                    _logger.LogInformation($"Creating local source: {source.Mountpoint}");

                    NtripSourceTable table = new(new ReadOnlyDictionary<string, NtripStrRecord>(new Dictionary<string, NtripStrRecord>()
                    {
                        { source.Mountpoint, new NtripStrRecord($"STR;{source.Mountpoint};Dronoskola;RTCM 3.2;1005(30),1074(1),1084(1),1094(1);2;GPS+GLO+GAL;SNIP;CZE;49.93;14.28;1;0;sNTRIP;none;B;N;3100;") }
                    }));

                    await source.InsertSourceTable(table);
                }
                else if (!String.IsNullOrEmpty(source.Host) && source.Port.HasValue)
                {
                    IPEndPoint serverEndPoint;

                    try
                    {
                        IPAddress[] ipAddressList = Dns.GetHostAddresses(source.Host);

                        // TODO: check if DNS resolved something

                        serverEndPoint = new IPEndPoint(ipAddressList[0], source.Port.Value);
                    }
                    catch (Exception ex) 
                    {
                        _logger.LogWarning($"Cannot connect to {source.Host}:{source.Port} - {ex.Message}");

                        continue;
                    }

                    NtripGetSourceTableHttpRequestMessage getSourceTableHttpRequestMessage;

                    if (source.AuthRequired)
                        getSourceTableHttpRequestMessage = new NtripGetSourceTableHttpRequestMessage(source.Host, source.Port.Value, source.Username, source.Password);
                    else
                        getSourceTableHttpRequestMessage = new NtripGetSourceTableHttpRequestMessage(source.Host, source.Port.Value);

                    // get source table
                    _logger.LogInformation($"Downloading source table from {source.Host}:{source.Port}");

                    var sourceTableResponse = await ntripHttpClient.GetSourceTableAsync(getSourceTableHttpRequestMessage);

                    _logger.LogInformation($"Successfully fetched {sourceTableResponse.AsNtripSourceTable()?.Streams?.Count ?? 0} sources.");

                    await source.InsertSourceTable(sourceTableResponse.AsNtripSourceTable());

                    // handle preconnection to sources
                    if (source.PreconnectMountpoints?.Count > 0)
                    {
                        foreach (var item in source.PreconnectMountpoints)
                        {
                            // Connect the stream
                            _logger.LogInformation($"Connecting to preconfigured source: {serverEndPoint.Address}:{serverEndPoint.Port}/{item}");

                            var client = new NtripStreamClientSession(serverEndPoint.Address.ToString(), serverEndPoint.Port, source, item);
                            var connectionResult = await client.SendConnectionRequest(true);

                            if (connectionResult)
                            {
                                _logger.LogInformation($"Successfully connected to {serverEndPoint.Address}:{serverEndPoint.Port}/{item}");
                            }

                            caster.AddStreamClient(client);
                        }
                    }

                    //bool connected = client.Connect();

                    //if (connected)
                    //    _logger.LogInformation("Successfully connected.");
                    //else
                    //    _logger.LogWarning("Error connecting.");

                    //string ntripRequest = source.GetSourceInfoRequestString();

                    //var t = client.SendAsync(Encoding.ASCII.GetBytes(ntripRequest));

                    //client.ReceiveAsync();
                    //client.DisconnectAsync();
                }
                else
                {
                    
                }
            }

            // Start the server
            await caster.StartServer();

            // client

            //// TCP server address
            //string serverAddress = "3.143.243.81";
            //// TCP server port
            //int serverPort = 2101;

            //Console.WriteLine($"TCP server address: {serverAddress}");
            //Console.WriteLine($"TCP server port: {port}");

            //Console.WriteLine();

            //// Create a new TCP chat client
            //var client = new NtripServerClient(serverAddress, port);

            //// Connect the client
            //Console.Write("Client connecting...");
            //client.ConnectAsync();
            //Console.WriteLine("Done!");

            //string username = "bouskdav@hotmail.com";
            //string password = "password";

            //// Create the Basic Authentication string
            //string auth = Convert.ToBase64String(Encoding.ASCII.GetBytes(username + ":" + password));

            //// Send NTRIP request
            //string ntripRequest = "GET /MSTAS HTTP/1.1\r\n" +
            //                       "Host: " + "3.143.243.81" + "\r\n" +
            //                       "Ntrip-Version: Ntrip/2.0\r\n" +
            //                       "User-Agent: NTRIP Client/1.0\r\n" +
            //                       "Connection: close\r\n" +
            //                       "Authorization: Basic " + auth + "\r\n\r\n";

            //client.SendAsync(ntripRequest);
            //// client

            //Console.WriteLine("Press Enter to stop the server or '!' to restart the server...");

            // Perform text input
            for (;;)
            {
                string line = Console.ReadLine();
                if (string.IsNullOrEmpty(line))
                    break;

                // Restart the server
                if (line == "!")
                {
                    Console.Write("Server restarting...");
                    await caster.RestartServer();
                    Console.WriteLine("Done!");
                    continue;
                }

                //// Multicast admin message to all sessions
                //line = "(admin) " + line;
                //server.Multicast(line);
            }

            // Stop the server
            Console.Write("Server stopping...");
            caster.StopServer();
            Console.WriteLine("Done!");
        }

        static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((hostingContext, configuration) =>
                {
                    configuration.Sources.Clear();

                    IHostEnvironment env = hostingContext.HostingEnvironment;

                    _hostingEnvironment = env;

                    string environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? env.EnvironmentName;

                    configuration
                        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                        .AddJsonFile($"appsettings.{environmentName}.json", optional: false, reloadOnChange: true)
                        .AddJsonFile(ApplicationConstants.ManagerRuntimeConfigName, optional: true, reloadOnChange: true);

                    IConfigurationRoot configurationRoot = configuration.Build();
                })
                .ConfigureServices((context, services) =>
                {
                    IPAddress serverAddress = IPAddress.Any;
                    int port = context.Configuration.GetValue<int>("ServerPort");

                    // if on linux, run management socket
                    if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                    {
                        string socketName = context.Configuration.GetValue<string>("SocketName", "ntripcore.caster.sock");
                        string socketPath = Path.Combine(context.Configuration.GetValue<string>("SocketPath", Path.GetTempPath()));

                        services.AddSingleton<INtripCoreInteropServer, NtripCoreUdsInteropServer>(services => new NtripCoreUdsInteropServer(socketPath));
                    }
                    else
                    {
                        services.AddSingleton<INtripCoreInteropServer, NtripCoreTcpInteropServer>(services => new NtripCoreTcpInteropServer(IPAddress.Any, 2100));
                    }

                    services.AddSingleton<NtripCasterServer>(services => new NtripCasterServer(serverAddress, port));

                    services.AddSingleton<NtripCaster>();

                    services.AddHttpClient();
                });
    }
}
