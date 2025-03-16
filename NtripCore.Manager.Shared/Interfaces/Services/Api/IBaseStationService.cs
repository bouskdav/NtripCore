using NtripCore.Manager.Shared.Models.SystemState;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtripCore.Manager.Shared.Interfaces.Services.Api
{
    public interface IBaseStationService
    {
        Task<SystemStateReport> GetSystemState();

        Task<SystemStateReport> SetOperatingMode(SetOperatingModeRequest request);

        Task<bool> SetGpsServiceState(SetGpsServiceRequest request);

        Task<bool> SetRtkBaseServiceState();
    }
}
