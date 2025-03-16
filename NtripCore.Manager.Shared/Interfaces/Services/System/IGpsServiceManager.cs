using Ghostware.GPS.NET.Models.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtripCore.Manager.Shared.Interfaces.Services.System
{
    public interface IGpsServiceManager
    {
        Task<bool> IsGpsServiceRunning();

        Task StartGpsService();

        Task StartGpsService(string? userName, string? password, string? ntripHost, int ntripPort, string? mountpoint);

        Task StopGpsService();

        event EventHandler<GpsDataEventArgs>? GpsDataReceived;
    }
}
