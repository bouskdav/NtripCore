using Ghostware.GPS.NET.Models.GpsdModels.Abstraction.Contracts;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Ghostware.GPS.NET.Models.GpsdModels
{
    public class GpsDevices : IGpsdMessage
    {
        public List<GpsDevice> Devices { get; set; }

        [JsonIgnore]
        public DateTime Time { get; set; }
    }
}
