using Ghostware.GPS.NET.Models.Events;
using Ghostware.GPS.NET.Models.GpsdModels;
using Microsoft.Extensions.Configuration;
using NtripCore.Manager.Hubs;
using NtripCore.Manager.Shared.Interfaces.Services.System;

namespace NtripCore.Manager.Services.System.Windows
{
    public class WindowsGpsServiceManager : IGpsServiceManager
    {
        private readonly IConfiguration _configuration;
        private readonly ApplicationHub _applicationHub;
        private readonly ILogger<WindowsGpsServiceManager> _logger;
        private readonly Random _random;

        private bool _isGpsServiceRunning = false;

        private Timer? _timer;

        public event EventHandler<GpsDataEventArgs>? GpsDataReceived;

        public WindowsGpsServiceManager(
            IConfiguration configuration,
            ApplicationHub applicationHub,
            ILogger<WindowsGpsServiceManager> logger)
        {
            _configuration = configuration;
            _applicationHub = applicationHub;
            _logger = logger;
            _random = new Random();

            _timer = new Timer(async (object? stateInfo) =>
            {
                if (_isGpsServiceRunning)
                    GpsdServiceOnLocationChanged(this, new GpsDataEventArgs(new GpsLocation() { Latitude = _random.Next(-90, 90), Longitude = _random.Next(-180, 180), Alt = _random.Next(0, 1000), SpeedKnots = _random.Next(0, 50) }));
            },
            new AutoResetEvent(false), 4000, 5000); // fire every 2000 milliseconds

            _logger.LogInformation("Initialized WindowsGpsdManager");
        }

        private void GpsdServiceOnLocationChanged(object sender, GpsDataEventArgs args)
        {
            _applicationHub.SendGpsData(args);

            GpsDataReceived?.Invoke(this, args);
        }

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
