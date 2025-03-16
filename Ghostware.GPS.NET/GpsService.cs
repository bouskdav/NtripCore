using System;
using Ghostware.GPS.NET.Enums;
using Ghostware.GPS.NET.Factories;
using Ghostware.GPS.NET.GpsClients;
using Ghostware.GPS.NET.Models.ConnectionInfo;
using Ghostware.GPS.NET.Models.Events;

namespace Ghostware.GPS.NET
{
    public class GpsService
    {
        #region Private Properties

        private readonly BaseGpsClient _client;

        #endregion

        #region Public Properties

        public bool IsRunning => _client.IsRunning;
        public GpsType GpsType => _client.GpsType;

        #endregion

        #region Constructors

        public GpsService(BaseGpsInfo baseGpsData)
        {
            _client = GpsClientFactory.Create(baseGpsData);
        }

        public GpsService(GpsType gpsServiceType)
        {
            _client = GpsClientFactory.Create(gpsServiceType);
        }

        #endregion

        #region Connect and Disconnect

        public bool Connect()
        {
            return _client.Connect();
        }

        public bool Disconnect()
        {
            return _client.Disconnect();
        }

        #endregion

        #region Register Events

        public void RegisterDataEvent(Action<object, GpsDataEventArgs> action)
        {
            _client.GpsCallbackEvent += new EventHandler<GpsDataEventArgs>(action);
        }

        public void RegisterRawDataEvent(Action<object, string> action)
        {
            _client.RawGpsCallbackEvent += new EventHandler<string>(action);
        }

        public void RegisterStatusEvent(Action<object, GpsStatus> action)
        {
            _client.GpsStatusEvent += new EventHandler<GpsStatus>(action);
        }

        public void RemoveDataEvent(Action<object, GpsDataEventArgs> action)
        {
            _client.GpsCallbackEvent -= new EventHandler<GpsDataEventArgs>(action);
        }

        public void RemoveRawDataEvent(Action<object, string> action)
        {
            _client.RawGpsCallbackEvent -= new EventHandler<string>(action);
        }

        public void RemoveStatusEvent(Action<object, GpsStatus> action)
        {
            _client.GpsStatusEvent -= new EventHandler<GpsStatus>(action);
        }
        
        #endregion
    }
}
