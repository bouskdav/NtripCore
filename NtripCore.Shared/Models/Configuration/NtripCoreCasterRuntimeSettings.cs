using NtripCore.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtripCore.Shared.Models.Configuration
{
    public class NtripCoreCasterRuntimeSettings
    {
        public string SocketName { get; set; }

        public string SocketPath { get; set; }

        public BaseStationInitModeType InitModeType { get; set; }

        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public double Alt { get; set; }

        public double Precision { get; set; }
    }
}
