using Ghostware.GPS.NET.Models.Events;
using NtripCore.Manager.Shared.Interfaces.Services.System;

namespace NtripCore.Manager.Services.System.Windows
{
    public class WindowsGpsServiceManager : IGpsServiceManager
    {
        private bool _isGpsServiceRunning = false;

        public event EventHandler<GpsDataEventArgs>? GpsDataReceived;

        public Task<bool> IsGpsServiceRunning()
        {
            return Task.FromResult(_isGpsServiceRunning);
        }

        public async Task StartGpsService() => await StartGpsService(null, null, null, 0, null);

        public Task StartGpsService(string? userName, string? password, string? ntripHost, int ntripPort, string? mountpoint)
        {
            _isGpsServiceRunning = true;

            return Task.Delay(1000);
        }

        public Task StopGpsService()
        {
            _isGpsServiceRunning = false;

            return Task.Delay(1000);
        }
    }
}
