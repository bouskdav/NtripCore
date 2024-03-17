﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NtripCore.Caster.Configs;
using NtripCore.Caster.Connections;
using NtripCore.Caster.Connections.DataPullers;
using NtripCore.Caster.Connections.DataPushers;
using NtripCore.Caster.Utility;
using NtripCore.Caster.Utility.Sources;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
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

        /// <summary>
        /// Handles client subscription
        /// </summary>
        /// <param name="session"></param>
        /// <param name="mountpoint"></param>
        public void SubscribeClientToMountpoint(NtripCasterSubscriberSession session, string mountpoint)
        {
            // TODO: subscription is done here, so all "nearest mountpoint" etc. logic should go here
            string nearestMountpointName = Program._configuration.GetValue<string>("NearestMountpoint:MountpointName");

            if (mountpoint == nearestMountpointName)
            {
                // if newly subscribed to nearest mountpoint, set nearest mode true for that source
                // wait for NMEA GGA message and resubscribe then
                session.SetNearestMode();

                return;
            }

            _clientSessions.Add(session.Id, mountpoint);

            EnsureSubscribedStreamIsConnected(session, mountpoint).GetAwaiter().GetResult();
        }

        public async Task ResubscribeClientToNearestMountpoint(NtripCasterSubscriberSession session)
        {
            if (!session.Latitude.HasValue || !session.Longitude.HasValue)
                return;

            var streams = new List<NtripStrRecord>();

            foreach (var source in SourceList)
            {
                var sourceTable = await source.GetSourceTableAsync();

                if (sourceTable != null)
                    streams.AddRange(sourceTable.Streams.Select(i => i.Value));
            }

            var nearestStream = streams.OrderBy(i => GpsUtilities.CalculateDistanceBetweenCoordinates(session.Latitude.Value, session.Longitude.Value, i.Latitude, i.Longitude)).FirstOrDefault();

            var distance = GpsUtilities.CalculateDistanceBetweenCoordinates(
                session.Latitude.Value, session.Longitude.Value, 
                nearestStream.Latitude, nearestStream.Longitude);

            Console.WriteLine($"Subscribing session {session.Id} to mountpoint {nearestStream.Mountpoint} with distance {distance} km.");

            if (_clientSessions.ContainsKey(session.Id))
                _clientSessions[session.Id] = nearestStream.Mountpoint;
            else
                _clientSessions.Add(session.Id, nearestStream.Mountpoint);

            EnsureSubscribedStreamIsConnected(session, nearestStream.Mountpoint).GetAwaiter().GetResult();
        }

        private async Task EnsureSubscribedStreamIsConnected(NtripCasterSubscriberSession session, string mountpoint)
        {
            // check if requested stream is present
            if (!_streams.ContainsKey(mountpoint))
            {
                // search sources
                foreach (NtripSource source in SourceList)
                {
                    var sourceTable = source.GetSourceTableAsync().GetAwaiter().GetResult();
                    if (sourceTable?.Streams?.ContainsKey(mountpoint) ?? false)
                    {
                        IPEndPoint serverEndPoint;

                        try
                        {
                            IPAddress[] ipAddressList = Dns.GetHostAddresses(source.Host);

                            // TODO: check if DNS resolved something

                            serverEndPoint = new IPEndPoint(ipAddressList[0], source.Port.Value);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogWarning($"Cannot connect to {source.Host}:{source.Port} - {ex.Message}");

                            break;
                        }

                        var client = new NtripStreamClientSession(serverEndPoint.Address.ToString(), serverEndPoint.Port, source, mountpoint);
                        var connectionResult = await client.SendConnectionRequest(true);

                        if (connectionResult)
                        {
                            _logger.LogInformation($"Successfully connected to {serverEndPoint.Address}:{serverEndPoint.Port}/{mountpoint}");
                        }

                        AddStreamClient(client);

                        break;
                    }
                }
            }
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
                _server.FindSession(item.Key)?.SendAsync(e.Data);
            }
                
        }
    }
}