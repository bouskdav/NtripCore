using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtripCore.Caster.Utility.Sources
{
    public class NtripStrRecord
    {
        private readonly string _record;

        private readonly string _type;
        private readonly string _mountpoint;
        private readonly string _identifier;
        private readonly string _format;
        private readonly string _formatDetails;
        private readonly string _carrier;
        private readonly string _navSystem;
        private readonly string _network;
        private readonly string _country;
        private readonly string _latitude;
        private readonly string _longitude;
        private readonly string _nmea;
        private readonly string _solution;
        private readonly string _generator;
        private readonly string _compression;
        private readonly string _authentication;
        private readonly string _fee;
        private readonly string _bitrate;
        private readonly string _misc;

        public NtripStrRecord(string record) 
        {
            _record = record;

            var fields = record.Split(";");

            _type = fields[0];
            _mountpoint = fields[1];
            _identifier = fields[2];
            _format = fields[3];
            _formatDetails = fields[4];
            _carrier = fields[5];
            _navSystem = fields[6];
            _country = fields[7];
            _latitude = fields[8];
            _longitude = fields[9];
            _nmea = fields[10];
            _solution = fields[11];
            _generator = fields[12];
            _compression = fields[13];
            _authentication = fields[14];
            _fee = fields[15];
            _bitrate = fields[16];
            _misc = fields[17];
        }

        public string Mountpoint => _mountpoint;
    }
}
