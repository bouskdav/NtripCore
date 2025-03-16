using NtripCore.Manager.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtripCore.Manager.Shared.Models.SystemState
{
    public class SetOperatingModeRequest
    {
        public OperatingModeType OperatingMode { get; set; }

        public bool IsGpsServiceRunning { get; set; }

        public bool NtripServiceEnabled { get; set; }

        public string Host { get; set; }

        public int? Port { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

        public string Mountpoint { get; set; }
    }
}
