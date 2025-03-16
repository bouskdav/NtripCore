using System;
using Ghostware.GPS.NET.Enums;
using Ghostware.GPS.NET.Models.GpsdModels;
using Ghostware.GPS.NET.Models.GpsdModels.Abstraction.Contracts;
using Ghostware.NMEAParser.NMEAMessages;

namespace Ghostware.GPS.NET.Models.Events
{
    public class GpsDataEventArgs : EventArgs
    {
        public GpsCoordinateSystem CoordinateSystem { get; set; } = GpsCoordinateSystem.GeoEtrs89;

        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public double Speed { get; set; }

        public IGpsdMessage Message { get; set; }
       
        public GpsDataEventArgs(IGpsdMessage message)
        {
            if (message is GpsLocation gpsLocation)
            {
                Latitude = gpsLocation.Latitude;
                Longitude = gpsLocation.Longitude;
                Speed = gpsLocation.Speed;
            }

            Message = message;
        }

        public GpsDataEventArgs(GprmcMessage gpsLocation)
        {
            Latitude = gpsLocation.Latitude;
            Longitude = gpsLocation.Longitude;
            Speed = gpsLocation.Speed;
        }

        public GpsDataEventArgs(double latitude, double longitude, double speed = 0.0d)
        {
            Latitude = latitude;
            Longitude = longitude;
            Speed = speed;
        }

        public override string ToString()
        {
            return $"Latitude: {Latitude} - Longitude: {Longitude} - Speed: {Speed}";
        }
    }
}
