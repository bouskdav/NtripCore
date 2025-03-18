using Ghostware.GPS.NET.Models.GpsdModels.Abstraction.Contracts;
using System;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Ghostware.GPS.NET.Models.GpsdModels
{
    [DataContract]
    public class GpsdVersion : IGpsdMessage
    {
        [DataMember(Name = "release")]
        public string Release { get; set; }

        [DataMember(Name = "rev")]
        public string Rev { get; set; }

        [DataMember(Name = "proto_major")]
        public int ProtoMajor { get; set; }

        [DataMember(Name = "proto_minor")]
        public int ProtoMinor { get; set; }

        [JsonIgnore]
        public DateTime Time { get; set; }

        public override string ToString()
        {
            return $"Release: {Release} - Revision: {Rev} - ProtoMajor: {ProtoMajor} - ProtoMinor: {ProtoMinor}";
        }
    }
}
