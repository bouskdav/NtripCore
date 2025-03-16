using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using Ghostware.GPS.NET.Models.ConnectionInfo;
using Ghostware.GPS.NET.Models.ConnectionInfo.Credentials;

namespace Ghostware.GPS.NET.Handlers
{
    public static class ProxyClientHandler
    {
        public static TcpClient GetTcpClient(GpsdInfo data)
        {
            var uriBuilder = new UriBuilder
            {
                Scheme = Uri.UriSchemeHttp,
                Host = data.ProxyAddress,
                Port = data.ProxyPort
            };

            var proxyUri = uriBuilder.Uri;
#pragma warning disable SYSLIB0014 // Type or member is obsolete
            var request = WebRequest.Create("http://" + data.Address + ":" + data.Port);
#pragma warning restore SYSLIB0014 // Type or member is obsolete
            var webProxy = new WebProxy(proxyUri);

            request.Proxy = webProxy;
            request.Method = "CONNECT";

            if (data.ProxyCredentials != null)
            {
                var credentials = data.ProxyCredentials;
                if (credentials.GetType() == typeof(ProxyCredentials))
                {
                    webProxy.Credentials = new NetworkCredential(credentials.ProxyUsername, ((ProxyCredentials)credentials).ProxyPassword);
                }
                else if (credentials.GetType() == typeof(SecureProxyCredentials))
                {
                    webProxy.Credentials = new NetworkCredential(data.ProxyCredentials.ProxyUsername, ((SecureProxyCredentials)credentials).ProxyPassword);
                }
            }
            else
            {
                webProxy.UseDefaultCredentials = true;
            }

            var response = request.GetResponse();
            var responseStream = response.GetResponseStream();
            Debug.Assert(responseStream != null);

            const BindingFlags flags = BindingFlags.NonPublic | BindingFlags.Instance;

            var rsType = responseStream.GetType();
            var connectionProperty = rsType.GetProperty("Connection", flags);

            var connection = connectionProperty.GetValue(responseStream, null);
            var connectionType = connection.GetType();
            var networkStreamProperty = connectionType.GetProperty("NetworkStream", flags);

            var networkStream = networkStreamProperty.GetValue(connection, null);
            var nsType = networkStream.GetType();
            var socketProperty = nsType.GetProperty("Socket", flags);
            var socket = (Socket)socketProperty.GetValue(networkStream, null);

            return new TcpClient { Client = socket };
        }
    }
}
