using NtripCore.Manager.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtripCore.Manager.Shared.Models.SystemState
{
    public class SystemStateReport
    {
        public OperatingModeType OperatingMode { get; set; }

        public bool IsGpsServiceRunning { get; set; }
        
        public bool IsRtkBaseServiceRunning { get; set; }

        public bool IsError { get; set; }

        public List<string> Errors { get; set; }
    }
}
