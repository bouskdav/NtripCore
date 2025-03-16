using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Ghostware.GPS.NET.Exceptions;

namespace Ghostware.GPS.NET.Models.GpsdModels
{
    [DataContract]
    public class DataClassType
    {
        public static Dictionary<string, Type> TypeDictionary = new Dictionary<string, Type>
        {
            {"VERSION", typeof(GpsdVersion)},
            {"DEVICES", typeof(GpsDevices)},
            {"WATCH", typeof(GpsdOptions)},
            {"TPV", typeof(GpsLocation)},
            {"SKY", typeof(GpsSky)},
            {"RAW", typeof(GpsRaw)},
            {"SUBFRAME", typeof(GpsRaw)},
        };

        [DataMember(Name = "class")]
        public string Class { get; set; }

        public Type GetClassType()
        {
            Type result;
            TypeDictionary.TryGetValue(Class, out result);

            if (result == null)
            {
                throw new UnknownTypeException(Class);
            }

            return result;
        }
    }
}
