using Ghostware.GPS.NET.Models.GpsdModels.Abstraction.Contracts;
using System.Collections.Generic;

namespace Ghostware.GPS.NET.Models.GpsdModels
{
    public class GpsDevices : IGpsdMessage
    {
        public List<GpsDevice> Devices { get; set; }
    }
}
