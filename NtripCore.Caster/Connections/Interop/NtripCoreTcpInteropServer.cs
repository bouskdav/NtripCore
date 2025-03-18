using NetCoreServer;
using NtripCore.Caster.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace NtripCore.Caster.Connections.Interop
{
    public class NtripCoreTcpInteropServer : TcpServer, INtripCoreInteropServer
    {
        public NtripCoreTcpInteropServer(IPAddress address, int port) : base(address, port) { }

        protected override TcpSession CreateSession() { return new TcpInteropSession(this); }

        protected override void OnError(SocketError error)
        {
            Console.WriteLine($"Chat TCP server caught an error with code {error}");
        }
    }
}
