using Microsoft.Extensions.Configuration;
using NetCoreServer;
using NtripCore.Caster.Configs;
using NtripCore.Caster.Utility;
using System;
using System.Collections.Generic;
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
        public NtripCasterSubscriberSession(TcpServer server) : base(server) { }

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
        }

        protected override void OnReceived(byte[] buffer, long offset, long size)
        {
            string message = Encoding.UTF8.GetString(buffer, (int)offset, (int)size);
            Console.WriteLine("Incoming: " + message);

            try
            {
                // proccess http requests
                if (message.Contains("HTTP"))
                {
                    var parsedHeaders = ParsedHttpHeaders.Parse(message);

                    if (!parsedHeaders.IsNTRIP)
                    {
                        var response = "HTTP/1.0 200 OK\r\n" +
                                       "Server: NTRIPCaster\r\n" +
                                       "Content-Type: text/plain" +
                                       "\r\n" +
                                       "\r\n" +
                                       "This is a NTRIP Caster";

                        SendAsync(response);
                        Disconnect();
                    }
                    else if (parsedHeaders.Mountpoint == "")
                    {
                        //// Send Sourcetable
                        //SendSourcetable(socket);
                        //socket.Close();

                        string sourceTableResponse = GetSourcetable(parsedHeaders);

                        SendAsync(sourceTableResponse);
                        Disconnect();
                    }
                    else if (parsedHeaders.IsSource)
                    {
                        //ProcessSource(socket, parsedHeaders);
                    }
                    else
                    {
                        ProcessClient(parsedHeaders);
                    }
                }
                // if message starts with $, dont parse headers
                else if (message.StartsWith("$"))
                {

                }
            }
            catch (Exception ex)
            {
                Disconnect();
            }

            //// Multicast message to all connected sessions
            //Server.Multicast(message);

            //// If the buffer starts with '!' the disconnect the current session
            //if (message == "!")
            //    Disconnect();
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

        private string GetSourcetable(ParsedHttpHeaders headers)
        {
            var table = new StringBuilder();

            //foreach (var source in _configuration.GetSection("Sources").Get<List<Source>>())
            //{
            //    table.Append("STR;");
            //    table.Append(source.Mountpoint); table.Append(";"); // Mountpoint
            //    table.Append(source.Identifier); table.Append(";"); // Identifier
            //    table.Append(source.Format); table.Append(";;");  // Format (No details)
            //    table.Append((int)source.Carrier); table.Append(";"); // Carrier
            //    table.Append(source.NavSystem); table.Append(";"); // NavSystem
            //    table.Append(source.Network); table.Append(";"); // Ref-Network
            //    table.Append(source.Country); table.Append(";"); // Country
            //    table.Append(source.Latitude.ToString("0.00")); table.Append(";"); // Latitude
            //    table.Append(source.Longitude.ToString("0.00")); table.Append(";"); // Longitude
            //    table.Append("0;"); // Client doesn't have to send NMEA
            //    table.Append("0;"); // Single Base Solution
            //    table.Append("Unknown;"); // Generator
            //    table.Append("none;");  // Compression/Encryption
            //    table.Append(source.AuthRequired ? "B" : "N"); table.Append(";"); // Basic Authentication
            //    table.Append("N;"); // No fee
            //    table.Append("9600;"); // Bitrate
            //    table.Append("\r\n");
            //}

            var builder = new StringBuilder();

            builder.Append("SOURCETABLE 200 OK\r\n");
            builder.Append("Server: NTRIP Caster/1.0\r\n");
            builder.Append("Conent-Type: text/plain\r\n");
            builder.Append($"Conent-Length: #{table.Length}\r\n");
            builder.Append("\r\n");
            builder.Append(table.ToString());
            builder.Append("ENDSOURCETABLE\r\n");

            return builder.ToString();
        }
    }
}
