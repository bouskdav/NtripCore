using Ghostware.GPS.NET.Models.GpsdModels.Abstraction.Contracts;
using System;
using System.Runtime.Serialization;

namespace Ghostware.GPS.NET.Models.GpsdModels
{
    [DataContract]
    public class GpsDevice
    {
        [DataMember(Name = "path")]
        public string Path { get; set; }

        [DataMember(Name = "driver")]
        public string Driver { get; set; }

        [DataMember(Name = "activated")]
        public DateTime Activated { get; set; }

        [DataMember(Name = "flags")]
        public int Flags { get; set; }

        [DataMember(Name = "native")]
        public int Native { get; set; }

        [DataMember(Name = "bps")]
        public int Bps { get; set; }

        [DataMember(Name = "parity")]
        public string Parity { get; set; }

        [DataMember(Name = "stopbits")]
        public int Stopbits { get; set; }

        [DataMember(Name = "cycle")]
        public float Cycle { get; set; }

        public override string ToString()
        {
            return $"Path: {Path} - Driver: {Driver} - Activated: {Activated} - Flags: {Flags} - Native: {Native} - Bps: {Bps} - Parity: {Parity} - Stopbits: {Stopbits} - Cycle: {Cycle}";
        }
    }
}
