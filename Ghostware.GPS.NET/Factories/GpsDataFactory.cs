using System;
using Ghostware.GPS.NET.Enums;
using Ghostware.GPS.NET.Models.ConnectionInfo;

namespace Ghostware.GPS.NET.Factories
{
    public static class GpsDataFactory
    {
        public static BaseGpsInfo Create(GpsType type)
        {
            var dataType = GetDataType(type);
            return (BaseGpsInfo)Activator.CreateInstance(dataType);
        }

        public static Type GetDataType(GpsType type)
        {
            switch (type)
            {
                //case GpsType.File:
                //    return typeof(FileGpsInfo);
                //case GpsType.ComPort:
                //    return typeof(ComPortInfo);
                case GpsType.Gpsd:
                    return typeof(GpsdInfo);
                case GpsType.WindowsLocationApi:
                    return typeof(WindowsLocationApiInfo);
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }
    }
}
