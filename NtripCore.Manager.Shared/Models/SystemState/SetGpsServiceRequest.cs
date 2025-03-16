using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtripCore.Manager.Shared.Models.SystemState
{
    public class SetGpsServiceRequest
    {
        public SetGpsServiceRequest() { }

        public SetGpsServiceRequest(bool gpsServiceEnabled, bool ntripServiceEnabled, string host, int? port, string username, string password, string mountpoint)
        {
            GpsServiceEnabled = gpsServiceEnabled;
            NtripServiceEnabled = ntripServiceEnabled;
            Host = host;
            Port = port;
            Username = username;
            Password = password;
            Mountpoint = mountpoint;
        }

        public bool GpsServiceEnabled { get; set; }

        public bool NtripServiceEnabled { get; set; }

        public string Host { get; set; }

        public int? Port { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

        public string Mountpoint { get; set; }

        public static SetGpsServiceRequest Disabled = 
            new SetGpsServiceRequest()
            {
                GpsServiceEnabled = false,
            };
    }
}
