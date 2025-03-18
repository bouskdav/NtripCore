using Ghostware.GPS.NET;
using Ghostware.GPS.NET.Models.ConnectionInfo;
using Ghostware.GPS.NET.Models.Events;
using NtripCore.Manager.Hubs;
using NtripCore.Manager.Shared.Interfaces.Services.System;
using System.Diagnostics;
using System.Text.Json;

namespace NtripCore.Manager.Services.System.Linux
{
    public class LinuxGpsdManager : IGpsServiceManager
    {
        private readonly IConfiguration _configuration;
        private readonly ApplicationHub _applicationHub;
        private readonly ILogger<LinuxGpsdManager> _logger;
        private readonly GpsService _gpsService;

        private bool _connected;

        public event EventHandler<GpsDataEventArgs>? GpsDataReceived;

        public LinuxGpsdManager(
            IConfiguration configuration,
            ApplicationHub applicationHub,
            ILogger<LinuxGpsdManager> logger)
        {
            _configuration = configuration;
            _applicationHub = applicationHub;
            _logger = logger;

            int gpsdListenPort = _configuration.GetValue<int>("InternalGps:Gpsd:ListenPort", 2947);
            string gpsdListenAddress = _configuration.GetValue<string>("InternalGps:Gpsd:ListenAddress", "127.0.0.1");

            var info = new GpsdInfo()
            {
                Address = gpsdListenAddress,
                Port = gpsdListenPort,
                IsProxyEnabled = false,
            };

            _gpsService = new GpsService(info);

            _gpsService.RegisterDataEvent(GpsdServiceOnLocationChanged);
            //_gpsService.Connect();

            _logger.LogInformation("Initialized LinuxGpsdManager");
        }

        private void GpsdServiceOnLocationChanged(object sender, GpsDataEventArgs args)
        {
            _applicationHub.SendGpsData(args);

            _logger.LogInformation($"Location received: {JsonSerializer.Serialize(args)}");

            GpsDataReceived?.Invoke(this, args);
        }

        public Task<bool> IsGpsServiceRunning()
        {
            string command = "pgrep gpsd";
            string result = "";
            using (Process proc = new Process())
            {
                proc.StartInfo.FileName = "/bin/bash";
                proc.StartInfo.Arguments = "-c \" " + command + " \"";
                proc.StartInfo.UseShellExecute = false;
                proc.StartInfo.RedirectStandardOutput = true;
                proc.StartInfo.RedirectStandardError = true;
                proc.Start();

                result += proc.StandardOutput.ReadToEnd();
                result += proc.StandardError.ReadToEnd();

                proc.WaitForExit();
            }

            if (!String.IsNullOrEmpty(result) && !_connected)
            {
                _gpsService.Connect();
                _connected = true;
            }

            _logger.LogInformation($"Checked GPSD running: {!String.IsNullOrEmpty(result)}");

            return Task.FromResult(!String.IsNullOrEmpty(result));
        }

        public async Task StartGpsService() => await StartGpsService(null, null, null, 0, null);

        public Task StartGpsService(string? userName, string? password, string? ntripHost, int ntripPort, string? mountpoint)
        {
            int baudRate = _configuration.GetValue<int>("InternalGps:Gpsd:BaudRate", 115200);
            string comPort = _configuration.GetValue<string>("InternalGps:Gpsd:Com", "/dev/ttyACM0");

            string command = !String.IsNullOrEmpty(ntripHost) ?
                $"gpsd -nG ntrip://{userName}:{password}@{ntripHost}:{ntripPort}/{mountpoint} -s {baudRate} {comPort}" :
                $"gpsd -nG -s {baudRate} {comPort}";
            string result = "";
            using (Process proc = new Process())
            {
                proc.StartInfo.FileName = "/bin/bash";
                proc.StartInfo.Arguments = "-c \" " + command + " \"";
                proc.StartInfo.UseShellExecute = false;
                proc.StartInfo.RedirectStandardOutput = true;
                proc.StartInfo.RedirectStandardError = true;
                proc.Start();

                result += proc.StandardOutput.ReadToEnd();
                result += proc.StandardError.ReadToEnd();

                proc.WaitForExit();
            }

            _gpsService.Connect();
            _connected = true;

            return Task.CompletedTask;
        }

        public Task StopGpsService()
        {
            string command = "pkill gpsd";
            string result = "";
            using (Process proc = new Process())
            {
                proc.StartInfo.FileName = "/bin/bash";
                proc.StartInfo.Arguments = "-c \" " + command + " \"";
                proc.StartInfo.UseShellExecute = false;
                proc.StartInfo.RedirectStandardOutput = true;
                proc.StartInfo.RedirectStandardError = true;
                proc.Start();

                result += proc.StandardOutput.ReadToEnd();
                result += proc.StandardError.ReadToEnd();

                proc.WaitForExit();
            }

            _gpsService.Disconnect();
            _connected = false;

            return Task.CompletedTask;
        }
    }
}
