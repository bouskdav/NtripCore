using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using Ghostware.GPS.NET.Constants;
using Ghostware.GPS.NET.Enums;
using Ghostware.GPS.NET.Exceptions;
using Ghostware.GPS.NET.Handlers;
using Ghostware.GPS.NET.Models.ConnectionInfo;
using Ghostware.GPS.NET.Models.Events;
using Ghostware.GPS.NET.Models.GpsdModels;
using Ghostware.GPS.NET.Parsers;
using Newtonsoft.Json;

namespace Ghostware.GPS.NET.GpsClients
{
    public class GpsdGpsClient : BaseGpsClient
    {
        #region Private Properties

        private TcpClient _client;
        private StreamReader _streamReader;
        private StreamWriter _streamWriter;

        private GpsdDataParser _gpsdDataParser;
        private int _retryReadCount;

        private DateTime? _previousReadTime;
        private Dictionary<string, DateTime?> _previousReadTimes = new();

        #endregion

        #region Constructors

        public GpsdGpsClient(GpsdInfo connectionData) : base(GpsType.Gpsd, connectionData)
        {
        }

        public GpsdGpsClient(BaseGpsInfo connectionData) : base(GpsType.Gpsd, connectionData)
        {
        }

        #endregion

        #region Connect and Disconnect

        public override bool Connect()
        {
            var data = (GpsdInfo)GpsInfo;

            IsRunning = true;
            OnGpsStatusChanged(GpsStatus.Connecting);

            try
            {
                _client = data.IsProxyEnabled ? ProxyClientHandler.GetTcpClient(data) : new TcpClient(data.Address, data.Port);
                _streamReader = new StreamReader(_client.GetStream());
                _streamWriter = new StreamWriter(_client.GetStream());

                _gpsdDataParser = new GpsdDataParser();

                var gpsData = "";
                while (string.IsNullOrEmpty(gpsData))
                {
                    gpsData = _streamReader.ReadLine();
                }

                var message = _gpsdDataParser.GetGpsData(gpsData, out string cls);

                var version = message as GpsdVersion;
                if (version == null) return false;

                ExecuteGpsdCommand(data.GpsOptions.GetCommand());
                OnGpsStatusChanged(GpsStatus.Connected);

                StartGpsReading(data);
            }
            catch
            {
                Disconnect();
                throw;
            }
            return true;
        }

        public void StartGpsReading(GpsdInfo data)
        {
            if (_streamReader == null || !_client.Connected) throw new NotConnectedException();

            _retryReadCount = data.RetryRead;
            IsRunning = true;
            while (IsRunning)
            {
                if (!_client.Connected)
                {
                    throw new ConnectionLostException();
                }

                try
                {
                    var gpsData = _streamReader.ReadLine();

                    OnRawGpsDataReceived(gpsData);

                    if (gpsData == null)
                    {
                        if (_retryReadCount == 0)
                        {
                            Disconnect();
                            throw new ConnectionLostException();
                        }
                        _retryReadCount--;
                        continue;
                    }

                    var message = _gpsdDataParser.GetGpsData(gpsData, out string classType);

                    var previousReadTime = _previousReadTimes[classType];

                    // discard lot of readings
                    if (previousReadTime == null || DateTime.Now.Subtract(new TimeSpan(0, 0, 0, 0, data.ReadFrequenty)) <= previousReadTime)
                        continue;

                    if (message is GpsLocation gpsLocation)
                    {
                        //if (gpsLocation == null || _previousReadTime != null && data.ReadFrequenty != 0 &&
                        //    gpsLocation.Time.Subtract(new TimeSpan(0, 0, 0, 0, data.ReadFrequenty)) <= _previousReadTime)
                        //    continue;
                        if (gpsLocation == null || previousReadTime != null && data.ReadFrequenty != 0 &&
                            gpsLocation.Time.Subtract(new TimeSpan(0, 0, 0, 0, data.ReadFrequenty)) <= previousReadTime)
                            continue;

                        //_previousReadTime = gpsLocation.Time;
                        _previousReadTimes[classType] = gpsLocation.Time;
                    }
                    else
                    {
                        _previousReadTimes[classType] = message.Time;
                    }

                    OnGpsDataReceived(new GpsDataEventArgs(message));
                }
                catch (IOException)
                {
                    Disconnect();
                    throw;
                }
            }
        }

        public override bool Disconnect()
        {
            if (!IsRunning) return true;
            IsRunning = false;
            ExecuteGpsdCommand(GpsdConstants.DisableCommand);

            _streamReader?.Close();
            _streamWriter?.Close();
            _client?.Close();

            OnGpsStatusChanged(GpsStatus.Disabled);

            return true;
        }

        #endregion

        #region GPSD Commands

        private void ExecuteGpsdCommand(string command)
        {
            if (_streamWriter == null) return;
            _streamWriter.WriteLine(command);
            _streamWriter.Flush();
        }

        #endregion
    }
}
