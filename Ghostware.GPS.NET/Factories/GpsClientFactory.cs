using System;
using Ghostware.GPS.NET.Enums;
using Ghostware.GPS.NET.Exceptions;
using Ghostware.GPS.NET.GpsClients;
using Ghostware.GPS.NET.Models.ConnectionInfo;

namespace Ghostware.GPS.NET.Factories
{
    public static class GpsClientFactory
    {
        public static BaseGpsClient Create(GpsType type)
        {
            switch (type)
            {
                //case GpsType.File:
                //    return new FileGpsClient(new FileGpsInfo());
                //case GpsType.ComPort:
                //    return new ComPortGpsClient(new ComPortInfo());
                case GpsType.Gpsd:
                    return new GpsdGpsClient(new GpsdInfo());
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }

        public static BaseGpsClient Create(BaseGpsInfo baseGpsData)
        {
            //if (baseGpsData.GetType() == typeof(FileGpsInfo))
            //{
            //    return new FileGpsClient(baseGpsData);
            //}
            //if (baseGpsData.GetType() == typeof(ComPortInfo))
            //{
            //    return new ComPortGpsClient(baseGpsData);
            //}
            if (baseGpsData.GetType() == typeof(GpsdInfo))
            {
                return new GpsdGpsClient(baseGpsData);
            }
            throw new UnknownTypeException(baseGpsData.GetType());
        }
    }
}
