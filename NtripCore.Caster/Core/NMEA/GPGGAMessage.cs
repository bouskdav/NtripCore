using NtripCore.Caster.Utility;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtripCore.Caster.Core.NMEA
{
    public class GPGGAMessage
    {
        public string MessageID { get; private set; }
        public TimeSpan UTCTime { get; private set; }
        public double Latitude { get; private set; }
        public double Longitude { get; private set; }
        public int FixQuality { get; private set; }
        public int SatellitesTracked { get; private set; }
        public double HDOP { get; private set; }
        public double Altitude { get; private set; }
        public double HeightOfGeoid { get; private set; }

        public static GPGGAMessage Parse(string sentence)
        {
            //if (sentence.StartsWith("$GPGGA") && sentence.EndsWith("*6B"))
            if (sentence.StartsWith("$GPGGA"))
            {
                string[] parts = sentence.Split(',');

                // Parse time
                double time = double.Parse(parts[1], CultureInfo.InvariantCulture);
                TimeSpan utcTime = TimeSpan.FromSeconds(time);

                // Parse latitude
                double latitude = double.Parse(parts[2], CultureInfo.InvariantCulture);
                latitude = Math.Floor(latitude / 100) + (latitude % 100) / 60;

                // Parse latitude direction
                if (parts[3] == "S")
                    latitude *= -1;

                // Parse longitude
                double longitude = double.Parse(parts[4], CultureInfo.InvariantCulture);
                longitude = Math.Floor(longitude / 100) + (longitude % 100) / 60;

                // Parse longitude direction
                if (parts[5] == "W")
                    longitude *= -1;

                // Parse fix quality
                int fixQuality = int.Parse(parts[6].SetDefaultValueIfNullOrEmpty("0"), CultureInfo.InvariantCulture);

                // Parse number of satellites tracked
                int satellitesTracked = int.Parse(parts[7].SetDefaultValueIfNullOrEmpty("0"), CultureInfo.InvariantCulture);

                // Parse HDOP
                double hdop = double.Parse(parts[8].SetDefaultValueIfNullOrEmpty("0"), CultureInfo.InvariantCulture);

                // Parse altitude
                double altitude = double.Parse(parts[9].SetDefaultValueIfNullOrEmpty("0"), CultureInfo.InvariantCulture);

                // Parse height of geoid
                double heightOfGeoid = double.Parse(parts[11].SetDefaultValueIfNullOrEmpty("0"), CultureInfo.InvariantCulture);

                return new GPGGAMessage
                {
                    MessageID = parts[0],
                    UTCTime = utcTime,
                    Latitude = latitude,
                    Longitude = longitude,
                    FixQuality = fixQuality,
                    SatellitesTracked = satellitesTracked,
                    HDOP = hdop,
                    Altitude = altitude,
                    HeightOfGeoid = heightOfGeoid
                };
            }
            else
            {
                throw new ArgumentException("Invalid NMEA sentence");
            }
        }
    }
}
