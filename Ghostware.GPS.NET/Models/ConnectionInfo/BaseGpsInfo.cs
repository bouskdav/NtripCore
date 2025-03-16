using Ghostware.GPS.NET.Enums;

namespace Ghostware.GPS.NET.Models.ConnectionInfo
{
    public abstract class BaseGpsInfo
    {
        public GpsCoordinateSystem CoordinateSystem { get; set; } = GpsCoordinateSystem.GeoEtrs89;

        public int ReadFrequenty { get; set; }
    }
}
