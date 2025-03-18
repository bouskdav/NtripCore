using Ghostware.GPS.NET.Models.GpsdModels.Abstraction.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Ghostware.GPS.NET.Models.GpsdModels
{
    [DataContract]
    public class GpsRaw : IGpsdMessage
    {
        public DateTime Time { get; set; }
    }
}
