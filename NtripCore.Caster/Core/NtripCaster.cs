using Microsoft.Extensions.Logging;
using NtripCore.Caster.Configs;
using NtripCore.Caster.Connections;
using NtripCore.Caster.Connections.DataPullers;
using NtripCore.Caster.Connections.DataPushers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtripCore.Caster.Core
{
    /// <summary>
    /// Ntrip Caster instance - basic class holding all connections
    /// </summary>
    public class NtripCaster
    {
        private readonly string _id;

        // one server instance - listener for all clients (subscribers)
        private readonly NtripCasterServer _server;
        private readonly ILogger<NtripCaster> _logger;

        // ntrip sources
        private Dictionary<string, NtripSource> _sources = new();

        // caster client (subscriber) sessions -> 
        // - when client connects, all casters are queried for their ntrip streams
        private Dictionary<Guid, string> _clientSessions = new();

        // caster server (pusher) sessions
        // - when received data for any stream, forward it to all subscribed clients
        private Dictionary<string, NtripStreamClientSession> _streams = new();

        public string Id => _id;

        public Dictionary<string, NtripSource> Sources { get => _sources; }
        public List<NtripSource> SourceList { get => _sources.Select(i => i.Value).ToList(); }

        public NtripCaster(
            NtripCasterServer server,
            ILogger<NtripCaster> logger)
        {
            _server = server;
            _logger = logger;

            _id = Guid.NewGuid().ToString();

            _server.RegisterCaster(this);
        }

        public Task StartServer()
        {
            _logger.LogInformation($"Starting server on {_server.Address}:{_server.Port}");
            
            _server.Start();

            _logger.LogInformation($"Started successfully.");

            return Task.CompletedTask;
        }

        public Task StopServer() 
        {
            _server.Stop();

            return Task.CompletedTask;
        }

        public Task RestartServer() 
        {
            _server.Restart();

            return Task.CompletedTask;
        }

        public void SubscribeClientToMountpoint(NtripCasterSubscriberSession session, string mountpoint)
        {
            _clientSessions.Add(session.Id, mountpoint);
        }

        public void AddNtripSource(NtripSource source)
        {
            _sources.Add(source.Id, source);
        }

        public void AddStreamClient(NtripStreamClientSession client)
        {
            _streams.Add(client.MountpointName, client);

            // subscribe to data received event
            client.StreamDataReceived += Client_StreamDataReceived;
        }

        private void Client_StreamDataReceived(object sender, Connections.DataPushers.Events.StreamDataReceivedEventArgs e)
        {
            Console.WriteLine($"Received data from {e.MountpointName}");

            foreach (var item in _clientSessions.Where(i => i.Value == e.MountpointName))
            {
                // TODO: handle client disconnection
                _server.FindSession(item.Key).SendAsync(e.Data);
            }
                
        }
    }
}