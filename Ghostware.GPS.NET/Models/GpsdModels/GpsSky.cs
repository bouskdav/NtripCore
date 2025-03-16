using Ghostware.GPS.NET.Models.GpsdModels.Abstraction.Contracts;
using System;
using System.Runtime.Serialization;

namespace Ghostware.GPS.NET.Models.GpsdModels
{


    [DataContract]
    public class GpsSkySatellite
    {
        public string PRN { get; set; }

        public string el { get; set; }

        public string az { get; set; }

        public string ss { get; set; }

        public bool used { get; set; }

    }
    

    [DataContract]
    public class GpsSky : IGpsdMessage
    {

        [DataMember(Name = "device")]
        public string Device { get; set; }

        [DataMember(Name = "time")]
        public DateTime Time { get; set; }

        [DataMember(Name = "xdop")]
        public string xdop { get; set; }

        [DataMember(Name = "ydop")]
        public string ydop { get; set; }

        [DataMember(Name = "vdop")]
        public string vdop { get; set; }

        [DataMember(Name = "tdop")]
        public string tdop { get; set; }

        [DataMember(Name = "hdop")]
        public string hdop { get; set; }

        [DataMember(Name = "gdop")]
        public string gdop { get; set; }

        [DataMember(Name = "pdop")]
        public string pdop { get; set; }

        [DataMember(Name = "satellites")]
        public GpsSkySatellite[] Satellites { get; set; }



    }
}
