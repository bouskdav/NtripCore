using NetCoreServer;
using NtripCore.Caster.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace NtripCore.Caster.Connections.Interop
{
    public class NtripCoreUdsInteropServer : UdsServer, INtripCoreInteropServer
    {
        public NtripCoreUdsInteropServer(string path) : base(path) { }

        protected override UdsSession CreateSession() { return new UdsInteropSession(this); }

        protected override void OnError(SocketError error)
        {
            Console.WriteLine($"Chat Unix Domain Socket server caught an error with code {error}");
        }
    }
}
