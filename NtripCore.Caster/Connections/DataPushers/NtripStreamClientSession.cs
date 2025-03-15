using NetCoreServer;
using NtripCore.Caster.Configs;
using NtripCore.Caster.Connections.DataPushers.Abstraction;
using NtripCore.Caster.Connections.DataPushers.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtripCore.Caster.Connections.DataPushers
{
    /// <summary>
    /// NtripStreamClient means subscription to an existing ntrip data stream (STR from source table)
    /// </summary>
    public class NtripStreamClientSession : TcpClient, INtripCorrectionSource
    {
        private readonly string _address;
        private readonly int _port;
        private readonly NtripSource _ntripSource;
        private readonly string _mountpointName;
        private bool _stop = false;
        private bool _persistConnection = false;

        public string MountpointName => _mountpointName;

        public NtripStreamClientSession(
            string address,
            int port,
            NtripSource ntripSource,
            string mountpointName) : base(address, port)
        {
            _address = address;
            _port = port;
            _ntripSource = ntripSource;
            _mountpointName = mountpointName;
        }

        public event StreamDataReceivedEventHandler StreamDataReceived;

        public void DisconnectAndStop()
        {
            _stop = true;

            DisconnectAsync();

            while (IsConnected)
                Thread.Yield();
        }

        protected override void OnConnected()
        {
            Console.WriteLine($"Connected to mountpoint {_mountpointName} with a new session {Id}");
        }

        protected override void OnSent(long sent, long pending)
        {
            base.OnSent(sent, pending);
        }

        protected override void OnDisconnected()
        {
            Console.WriteLine($"Disconnected from mountpoint {_mountpointName} with session {Id} with reconnect option: {!_stop}");

            // Wait for a while...
            Thread.Sleep(1000);

            // Try to connect again
            if (!_stop)
                ConnectAsync();
        }

        protected override void OnReceived(byte[] buffer, long offset, long size)
        {
            byte[] data = new byte[size];
            System.Buffer.BlockCopy(buffer, (int)offset, data, 0, (int)size);

            StreamDataReceivedEventArgs args = new StreamDataReceivedEventArgs();
            args.MountpointName = MountpointName;
            args.Data = data;
            OnStreamDataReceived(args);

            //Console.WriteLine(Encoding.UTF8.GetString(buffer, (int)offset, (int)size));
        }

        protected override void OnError(System.Net.Sockets.SocketError error)
        {
            Console.WriteLine($"An error occured with session {Id}: {error}");
        }

        public Task<bool> SendConnectionRequest(bool persistConnection = false)
        {
            bool connected = IsConnected;

            if (!connected)
            {
                Connect();
                _stop = true;
            }

            string authString = string.Empty;
            if (_ntripSource.AuthRequired)
            {
                // Create the Basic Authentication string
                authString = $"Authorization: Basic {Convert.ToBase64String(Encoding.ASCII.GetBytes(_ntripSource.Username + ":" + _ntripSource.Password))}\r\n";
            }

            // Send NTRIP request
            string ntripRequest = "GET /" + _mountpointName + " HTTP/1.1\r\n" +
                                  "Host: " + _address + "\r\n" +
                                  "Ntrip-Version: Ntrip/2.0\r\n" +
                                  "User-Agent: NTRIP Client/1.0\r\n" +
                                  "Connection: close\r\n" +
                                  authString + "\r\n";

            var requestSent = SendAsync(Encoding.ASCII.GetBytes(ntripRequest));

            ReceiveAsync();

            _persistConnection = persistConnection;

            return Task.FromResult(IsConnected && requestSent);
        }

        protected virtual void OnStreamDataReceived(StreamDataReceivedEventArgs e) 
        {
            StreamDataReceivedEventHandler handler = StreamDataReceived;
            if (handler != null) 
            {
                handler(this, e);
            }
        }
    }
}
