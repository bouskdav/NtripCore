using Ghostware.GPS.NET.Models.GpsdModels.Abstraction.Contracts;
using System;
using System.Runtime.Serialization;

namespace Ghostware.GPS.NET.Models.GpsdModels
{
    [DataContract]
    public class GpsLocation : IGpsdMessage
    {
        [DataMember(Name = "tag")]
        public string Tag { get; set; }

        [DataMember(Name = "device")]
        public string Device { get; set; }

        [DataMember(Name = "mode")]
        public int Mode { get; set; }

        [DataMember(Name = "time")]
        public DateTime Time { get; set; }

        [DataMember(Name = "ept")]
        public float Ept { get; set; }

        [DataMember(Name = "lat")]
        public double Latitude { get; set; }

        [DataMember(Name = "lon")]
        public double Longitude { get; set; }

        [DataMember(Name = "alt")]
        public float Alt { get; set; }

        [DataMember(Name = "altHAE")]
        public float AltHae { get; set; }

        [DataMember(Name = "altMSL")]
        public float AltMsl { get; set; }

        [DataMember(Name = "track")]
        public float Track { get; set; }

        [DataMember(Name = "speed")]
        public float SpeedKnots { get; set; }

        public double Speed => SpeedKnots * 1.852;

        [DataMember(Name = "epx")]
        public float EstimatedXError { get; set; }

        [DataMember(Name = "epy")]
        public float EstimatedYError { get; set; }

        [DataMember(Name = "epv")]
        public float EstimatedVError { get; set; }

        public override string ToString()
        {
            return $"Tag: {Tag} - Device: {Device} - Mode: {Mode} - Time: {Time} - Latitude: {Latitude} - Longitude: {Longitude} - Track: {Track} - Speed: {Speed}";
        }
    }
}
