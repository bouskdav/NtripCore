using NtripCore.Manager.Shared.Interfaces.Services.Api;
using NtripCore.Manager.Shared.Interfaces.Services.System;
using NtripCore.Manager.Shared.Models.SystemState;

namespace NtripCore.Manager.Services.BaseStation
{
    public class ServerBaseStationService(
        IGpsServiceManager gpsServiceManager) : IBaseStationService
    {
        public async Task<SystemStateReport> GetSystemState()
        {
            SystemStateReport result = new SystemStateReport();

            result.IsGpsServiceRunning = await gpsServiceManager.IsGpsServiceRunning();

            if (!result.IsGpsServiceRunning && !result.IsRtkBaseServiceRunning)
                result.OperatingMode = Shared.Enums.OperatingModeType.None;
            else if (result.IsGpsServiceRunning && !result.IsRtkBaseServiceRunning)
                result.OperatingMode = Shared.Enums.OperatingModeType.Gps;
            else if (!result.IsGpsServiceRunning && result.IsRtkBaseServiceRunning)
                result.OperatingMode = Shared.Enums.OperatingModeType.FixedBase;

            return result;
        }

        public async Task<SystemStateReport> SetOperatingMode(SetOperatingModeRequest request)
        {
            switch (request.OperatingMode)
            {
                case Shared.Enums.OperatingModeType.None:
                    // turn off everything
                    await SetGpsServiceState(SetGpsServiceRequest.Disabled);
                    //await SetRtkBaseServiceState();
                    break;
                case Shared.Enums.OperatingModeType.Gps:
                    // disable rtkbase and enable gps service
                    //await SetRtkBaseServiceState();
                    await SetGpsServiceState(new SetGpsServiceRequest(true, request.NtripServiceEnabled, request.Host, request.Port, request.Username, request.Password, request.Mountpoint));
                    break;
                case Shared.Enums.OperatingModeType.FixedBase:
                    // disable gps service and enable rtkbase
                    await SetGpsServiceState(SetGpsServiceRequest.Disabled);
                    await SetRtkBaseServiceState();
                    break;
            }

            return await GetSystemState();
        }

        public async Task<bool> SetGpsServiceState(SetGpsServiceRequest request)
        {
            var isGpsRunning = await gpsServiceManager.IsGpsServiceRunning();

            // if running, always stop
            if (isGpsRunning)
                await gpsServiceManager.StopGpsService();

            // if requested to start, enable
            if (request.GpsServiceEnabled)
            {
                if (request.NtripServiceEnabled && 
                    !String.IsNullOrEmpty(request.Username) &&
                    !String.IsNullOrEmpty(request.Password) &&
                    !String.IsNullOrEmpty(request.Host) &&
                    request.Port.HasValue &&
                    !String.IsNullOrEmpty(request.Mountpoint))
                {
                    await gpsServiceManager.StartGpsService(request.Username, request.Password, request.Host, request.Port!.Value, request.Mountpoint);
                }
                else
                {
                    await gpsServiceManager.StartGpsService();
                }
            }

            return await gpsServiceManager.IsGpsServiceRunning();
        }

        public Task<bool> SetRtkBaseServiceState()
        {
            throw new NotImplementedException();
        }
    }
}
