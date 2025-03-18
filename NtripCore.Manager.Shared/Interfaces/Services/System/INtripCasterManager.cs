using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtripCore.Manager.Shared.Interfaces.Services.System
{
    public interface INtripCasterManager
    {
        Task<bool> IsNtripCasterServiceRunning();

        Task StartNtripCasterService();

        Task StopNtripCasterService();
    }
}
