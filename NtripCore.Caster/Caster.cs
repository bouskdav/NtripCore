using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtripCore.Caster
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net;
    using System.Net.Sockets;
    using System.Text;
    using System.Threading.Tasks;

    public class NTRIPCaster
    {
        static Dictionary<string, TcpClient> mountPointConnections = new Dictionary<string, TcpClient>();

        static async Task Main2(string[] args)
        {
            // Configuration
            string ntripServerAddress = "ntrip.example.com";
            int ntripServerPort = 2101; // Adjust this according to your NTRIP server
            int casterPort = 2102; // Port on which the caster will listen

            // Start the caster
            await StartCaster(ntripServerAddress, ntripServerPort, casterPort);
        }

        static async Task StartCaster(string ntripServerAddress, int ntripServerPort, int casterPort)
        {
            TcpListener listener = new TcpListener(IPAddress.Any, casterPort);
            listener.Start();
            Console.WriteLine($"NTRIP Caster listening on port {casterPort}");

            try
            {
                while (true)
                {
                    TcpClient client = await listener.AcceptTcpClientAsync();
                    _ = HandleClient(client, ntripServerAddress, ntripServerPort);
                }
            }
            finally
            {
                listener.Stop();
            }
        }

        static async Task HandleClient(TcpClient client, string ntripServerAddress, int ntripServerPort)
        {
            Console.WriteLine($"New client connected: {client.Client.RemoteEndPoint}");

            try
            {
                using (client)
                using (NetworkStream clientStream = client.GetStream())
                {
                    // Read mount point request from client
                    byte[] buffer = new byte[1024];
                    int bytesRead = await clientStream.ReadAsync(buffer, 0, buffer.Length);
                    string mountPoint = Encoding.ASCII.GetString(buffer, 0, bytesRead);

                    // Check if mount point connection already exists
                    if (!mountPointConnections.ContainsKey(mountPoint))
                    {
                        // Establish connection to NTRIP server for the requested mount point
                        TcpClient server = new TcpClient(ntripServerAddress, ntripServerPort);

                        mountPointConnections[mountPoint] = server;

                        Console.WriteLine($"Connection established for mount point: {mountPoint}");
                    }

                    TcpClient serverConnection = mountPointConnections[mountPoint];
                    using (NetworkStream serverStream = serverConnection.GetStream())
                    {
                        // Forward client data to NTRIP server
                        await clientStream.CopyToAsync(serverStream);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error handling client: {ex.Message}");
            }

            Console.WriteLine($"Client disconnected: {client.Client.RemoteEndPoint}");
        }
    }
}
