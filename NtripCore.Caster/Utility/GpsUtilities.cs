using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtripCore.Caster.Utility
{
    public class GpsUtilities
    {
        public static double CalculateDistanceBetweenCoordinates(double fromLatitude, double fromLongitude, double toLatitude, double toLongitude)
        {
            double x = 69.1 * (toLatitude - fromLatitude);
            double y = 69.1 * (toLongitude - fromLongitude) * Math.Cos(fromLatitude / 57.3);

            // Convert to KM by multiplying 1.609344
            return (Math.Sqrt(x * x + y * y) * 1.609344);
        }
    }
}
