using Microsoft.Extensions.Configuration;
using NetCoreServer;
using NtripCore.Caster.Configs;
using NtripCore.Caster.Core;
using NtripCore.Caster.Core.NMEA;
using NtripCore.Caster.Core.NtripHttp.Request;
using NtripCore.Caster.Utility;
using NtripCore.Caster.Utility.Sources;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtripCore.Caster.Connections.DataPullers
{
    /// <summary>
    /// NtripCasterSubscriberSession means (subscriber) client connection
    /// </summary>
    public class NtripCasterSubscriberSession : TcpSession
    {
        private readonly NtripCasterServer _server;
        private readonly NtripCaster _ntripCaster;

        private bool _nearestMode = false;
        private double? _latitude;
        private double? _longitude;
        private DateTime? _lastUpdateTime;
        private int? _updateFrequency;

        public double? Latitude { get => _latitude; set => _latitude = value; }
        public double? Longitude { get => _longitude; set => _longitude = value; }
        public DateTime NextUpdateTime => _lastUpdateTime.HasValue && _updateFrequency.HasValue ?
            _lastUpdateTime.Value.AddSeconds(_updateFrequency.Value) :
            DateTime.Now;

        public DateTime UpdateTime { set => _lastUpdateTime = value; }

        public NtripCasterSubscriberSession(NtripCasterServer server, NtripCaster ntripCaster) 
            : base(server)
        {
            _server = server;
            _ntripCaster = ntripCaster;
        }

        protected override void OnConnected()
        {
            Console.WriteLine($"Chat TCP session with Id {Id} connected!");

            //// Send invite message
            //string message = "Hello from TCP chat! Please send a message or '!' to disconnect the client!";
            //SendAsync(message);
        }

        protected override void OnDisconnected()
        {
            Console.WriteLine($"Chat TCP session with Id {Id} disconnected!");

            _ntripCaster.NotifyClientUnsubscribed(Id);
        }

        // BEWARE of differences between ntrip rev 1 and rev2
        // https://www.use-snip.com/kb/knowledge-base/ntrip-rev1-versus-rev2-formats/ -> especially in responses ICY vs HTTP
        protected override void OnReceived(byte[] buffer, long offset, long size)
        {
            string message = Encoding.UTF8.GetString(buffer, (int)offset, (int)size);
            Console.WriteLine("Incoming: " + message);

            try
            {
                // proccess http requests
                if (message.Contains("HTTP"))
                {
                    IncomingNtripHttpRequestMessage incomingMessage = new IncomingNtripHttpRequestMessage(message);

                    // TODO: check authorization

                    // if request comes from NTRIP client
                    if (incomingMessage.IsNtripClient)
                    {
                        // if source table is requested
                        if (incomingMessage.IsSourceTableRequested)
                        {
                            string sourceTableResponse = GetSourcetable(incomingMessage).GetAwaiter().GetResult();

                            SendAsync(sourceTableResponse);
                            Disconnect();
                        }
                        // else its a stream subscription
                        else
                        {
                            var okResponse = GetOkResponse(incomingMessage).GetAwaiter().GetResult();

                            SendAsync(okResponse);

                            _ntripCaster.SubscribeClientToMountpoint(this, incomingMessage.RequestedMountpointName);
                        }
                    }
                }
                // if message starts with $ (NMEA messages), don't parse headers
                else if (message.StartsWith("$"))
                {
                    // TODO: NMEA strings go here
                    if (message.StartsWith("$GPGGA"))
                    {
                        var gpggaMessage = GPGGAMessage.Parse(message);

                        _latitude = gpggaMessage.Latitude;
                        _longitude = gpggaMessage.Longitude;

                        DateTime now = DateTime.Now;

                        // TODO: update position an resubscribe to nearest
                        if (_nearestMode)
                        {
                            if (now <= NextUpdateTime)
                            {
                                UpdateTime = now;
                                _ntripCaster.ResubscribeClientToNearestMountpoint(this).GetAwaiter().GetResult();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: " + ex.ToString());
                Disconnect();
            }
        }

        protected override void OnError(System.Net.Sockets.SocketError error)
        {
            Console.WriteLine($"Chat TCP session caught an error with code {error}");
        }

        private void ProcessSource(ParsedHttpHeaders parsedHeaders)
        {
            //// Check Mountpoint
            //var source = Config.Sources.FirstOrDefault(x => x.Mountpoint == parsedHeaders.Mountpoint);

            //if (source == null)
            //{
            //    string sourceTableResponse = GetSourcetable(parsedHeaders);

            //    SendAsync(sourceTableResponse);
            //    Disconnect();

            //    return;
            //}

            ////// Check if there is another Server
            ////if (Servers[parsedHeaders.Mountpoint] != null) {
            ////    SendToSocket(socket, "ERROR - Mountpoint already in use"); // Check behavior
            ////    socket.Close();
            ////    return;
            ////}

            //// Check Login
            //if (!source.Password.Equals(parsedHeaders.Password, StringComparison.OrdinalIgnoreCase))
            //{
            //    SendAsync("ERROR - Bad Password");
            //    Disconnect();

            //    return;
            //}

            //// Read Data
            //socket.Send(Encoding.ASCII.GetBytes("ICY 200 OK\r\n"));

            //Console.WriteLine($"New Server: {socket.RemoteEndPoint}");
            //var server = new ServerConnection(socket, parsedHeaders.Mountpoint, this);
            //server.StartProcessing();
        }

        private void ProcessClient(ParsedHttpHeaders parsedHeaders)
        {
            //// Check Mountpoint
            //var sources = _configuration.GetSection("Sources").Get<List<Source>>();
            //var source = sources.FirstOrDefault(x => x.Mountpoint == parsedHeaders.Mountpoint);

            //if (source == null)
            //{
            //    string sourceTableResponse = GetSourcetable(parsedHeaders);

            //    SendAsync(sourceTableResponse);
            //    Disconnect();

            //    return;
            //}

            //// Check Login
            //var users = _configuration.GetSection("Users").Get<List<User>>();
            //var user = users.FirstOrDefault(x => x.Name.Equals(parsedHeaders.Username, StringComparison.OrdinalIgnoreCase));
            //if (!source.AuthRequired ||
            //    (user != null && user.Password.Equals(parsedHeaders.Password, StringComparison.OrdinalIgnoreCase) && user.Mountpoints.Contains(parsedHeaders.Mountpoint)))
            //{
            //    SendAsync(Encoding.ASCII.GetBytes("ICY 200 OK\r\n"));
            //    //socket.SendTimeout = 1000;

            //    //var client = new ClientConnection(socket, parsedHeaders.Mountpoint, parsedHeaders.Username);
            //    //client.StrartProcessing();
            //    //AddClient(client);
            //}
            //else
            //{
            //    SendAsync("ERROR - Bad Password");
            //    Disconnect();
            //}
        }

        //private string GetSourcetable(ParsedHttpHeaders headers)
        //{
        //    var table = new StringBuilder();

        //    //foreach (var source in _configuration.GetSection("Sources").Get<List<Source>>())
        //    //{
        //    //    table.Append("STR;");
        //    //    table.Append(source.Mountpoint); table.Append(";"); // Mountpoint
        //    //    table.Append(source.Identifier); table.Append(";"); // Identifier
        //    //    table.Append(source.Format); table.Append(";;");  // Format (No details)
        //    //    table.Append((int)source.Carrier); table.Append(";"); // Carrier
        //    //    table.Append(source.NavSystem); table.Append(";"); // NavSystem
        //    //    table.Append(source.Network); table.Append(";"); // Ref-Network
        //    //    table.Append(source.Country); table.Append(";"); // Country
        //    //    table.Append(source.Latitude.ToString("0.00")); table.Append(";"); // Latitude
        //    //    table.Append(source.Longitude.ToString("0.00")); table.Append(";"); // Longitude
        //    //    table.Append("0;"); // Client doesn't have to send NMEA
        //    //    table.Append("0;"); // Single Base Solution
        //    //    table.Append("Unknown;"); // Generator
        //    //    table.Append("none;");  // Compression/Encryption
        //    //    table.Append(source.AuthRequired ? "B" : "N"); table.Append(";"); // Basic Authentication
        //    //    table.Append("N;"); // No fee
        //    //    table.Append("9600;"); // Bitrate
        //    //    table.Append("\r\n");
        //    //}

        //    var builder = new StringBuilder();

        //    builder.Append("SOURCETABLE 200 OK\r\n");
        //    builder.Append("Server: NTRIP Caster/1.0\r\n");
        //    builder.Append("Conent-Type: text/plain\r\n");
        //    builder.Append($"Conent-Length: #{table.Length}\r\n");
        //    builder.Append("\r\n");
        //    builder.Append(table.ToString());
        //    builder.Append("ENDSOURCETABLE\r\n");

        //    return builder.ToString();
        //}

        private async Task<string> GetSourcetable(IncomingNtripHttpRequestMessage requestMessage)
        {
            var table = new StringBuilder();

            // if nearest source enabled, put the source on top
            if (Program._configuration.GetValue<bool>("NearestMountpoint:Enabled"))
            {
                NtripStrRecord nearestStream = new NtripStrRecord(
                    Program._configuration.GetValue<string>("NearestMountpoint:MountpointName"),
                    "NtripCore nearest mountpoint function",
                    "RTCM 3",
                    "",
                    "2",
                    "GPS+GLO+GAL+BDS+QZS",
                    "NtripCore network",
                    "XXX",
                    "0",
                    "0",
                    "1",
                    "1",
                    "NtripCore",
                    "none",
                    "B",
                    "N",
                    "",
                    "");

                table.Append(nearestStream.GenerateSourceTableString() + "\r\n");
            }

            foreach (var source in _ntripCaster.SourceList)
            {
                var sourceTable = await source.GetSourceTableAsync();

                foreach (var stream in sourceTable.Streams)
                {
                    // TODO: change from original string
                    table.Append(stream.Value.OriginalString + "\r\n");
                }
            }

            var builder = new StringBuilder();

            if (requestMessage.IsVersion2)
            {
                builder.Append("HTTP/1.1 200 OK\r\n");
                builder.Append("Ntrip-Version: Ntrip/2.0\r\n");
                builder.Append("Ntrip-Flags:\r\n");
            }
            else
            {
                builder.Append("SOURCETABLE 200 OK\r\n");
            }
            
            builder.Append("Server: NtripCore\r\n");
            builder.Append($"Date: {DateTime.UtcNow.ToString("ddd,' 'dd' 'MMMM' 'yyyy' 'HH':'mm':'ss' UTC'", CultureInfo.InvariantCulture)}\r\n");
            
            if (requestMessage.IsVersion2)
            {
                builder.Append("Connection: close\r\n");
                builder.Append("Content-Type: gnss/sourcetable\r\n");
            }

            builder.Append($"Content-Length: {table.Length + 15}\r\n");
            builder.Append("\r\n");
            builder.Append(table.ToString());
            builder.Append("ENDSOURCETABLE\r\n");

            return builder.ToString();
        }

        private async Task<string> GetOkResponse(IncomingNtripHttpRequestMessage requestMessage)
        {
            var builder = new StringBuilder();

            if (requestMessage.IsVersion2)
            {
                builder.Append("HTTP/1.1 200 OK\r\n");
                builder.Append($"Date: {DateTime.UtcNow.ToString("ddd,' 'dd' 'MMMM' 'yyyy' 'HH':'mm':'ss' UTC'", CultureInfo.InvariantCulture)}\r\n");
                builder.Append("Server: NtripCore\r\n");
                builder.Append("Ntrip-Version: Ntrip/2.0\r\n");
                builder.Append("Cache-Control: no-store, no-cache, max-age=0\r\n");
                builder.Append("Pragma: no-cache\r\n");
                builder.Append("Connection: close\r\n");
                builder.Append("Content-Type: gnss/data\r\n");
            }
            else
            {
                builder.Append("ICY 200 OK\r\n");
            }

            return builder.ToString();
        }

        public void SetNearestMode(int updateLimitSeconds)
        {
            _nearestMode = true;
            _updateFrequency = updateLimitSeconds;
        }
    }
}
