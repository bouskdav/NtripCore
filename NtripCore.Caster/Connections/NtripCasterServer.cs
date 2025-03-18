using NetCoreServer;
using NtripCore.Caster.Connections.DataPullers;
using NtripCore.Caster.Core;
using Serilog.Settings.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace NtripCore.Caster.Connections
{
    /// <summary>
    /// Server - that listens on specific IP address and port for client connection
    /// </summary>
    public class NtripCasterServer : TcpServer
    {
        private NtripCaster _ntripCaster;

        public NtripCasterServer(IPAddress address, int port) : base(address, port) { }

        protected override TcpSession CreateSession() 
        {
            return new NtripCasterSubscriberSession(this, _ntripCaster);
        }

        protected override void OnError(System.Net.Sockets.SocketError error)
        {
            Console.WriteLine($"Chat TCP server caught an error with code {error}");
        }

        protected override void OnConnected(TcpSession session)
        {
            base.OnConnected(session);
        }

        internal void RegisterCaster(NtripCaster ntripCaster)
        {
            _ntripCaster = ntripCaster;
        }
    }
}
