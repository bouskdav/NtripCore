using NtripCore.Manager.Shared.Constants;
using NtripCore.Manager.Shared.Interfaces.Services.Api;
using NtripCore.Manager.Shared.Models.SystemState;
using NtripCore.Manager.Shared.Models.Weather;
using System.Net.Http.Json;

namespace NtripCore.Manager.Client.Services.BaseStation
{
    public class ClientBaseStationService(HttpClient httpClient) : IBaseStationService
    {
        public Task<SystemStateReport> GetSystemState() =>
            httpClient.GetFromJsonAsync<SystemStateReport>(RouteConstants.BaseStation.GetSystemState)!;

        public async Task<SystemStateReport> SetOperatingMode(SetOperatingModeRequest request)
        {
            var response = await httpClient.PostAsJsonAsync<SetOperatingModeRequest>(RouteConstants.BaseStation.SetOperatingMode, request);

            if (!response.IsSuccessStatusCode)
                return null;

            return await response.Content.ReadFromJsonAsync<SystemStateReport>();
        }

        public async Task<bool> SetGpsServiceState(SetGpsServiceRequest request)
        {
            var response = await httpClient.PostAsJsonAsync<SetGpsServiceRequest>(RouteConstants.BaseStation.SetGpsServiceState, request);

            if (!response.IsSuccessStatusCode)
                return false;

            var data = await response.Content.ReadAsStringAsync();

            return true;
        }

        public Task<bool> SetRtkBaseServiceState()
        {
            throw new NotImplementedException();
        }
    }
}
