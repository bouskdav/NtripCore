using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using NtripCore.Caster.Core.NtripHttp.Request;
using NtripCore.Caster.Core.NtripHttp.Response;

namespace NtripCore.Caster.Core.NtripHttp
{
    public class NtripHttpClient
    {
        public Task<NtripGetSourceTableHttpResponseMessage> GetSourceTableAsync(NtripGetSourceTableHttpRequestMessage requestMessage)
        {
            // Create a TCP client
            TcpClient client = new TcpClient();

            IPAddress[] ipAddressList = Dns.GetHostAddresses(requestMessage.Host);
            IPEndPoint serverEndPoint = new IPEndPoint(ipAddressList[0], requestMessage.Port);

            // Connect to the server
            client.Connect(serverEndPoint); // Replace "server_ip" with the actual IP address of the server and 8080 with the port number

            // Get the network stream
            NetworkStream stream = client.GetStream();

            // Send data to the server
            byte[] data = requestMessage.GetBytes();
            stream.Write(data, 0, data.Length);

            // Receive response from the server
            byte[] responseBuffer = new byte[1_000_000];
            int bytesRead = stream.Read(responseBuffer, 0, responseBuffer.Length);
            string response = Encoding.ASCII.GetString(responseBuffer, 0, bytesRead);

            // Close the connection
            client.Close();

            return Task.FromResult(new NtripGetSourceTableHttpResponseMessage(response));
        }
    }
}
