using NtripCore.Caster.Core.NtripHttp.Request.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtripCore.Caster.Core.NtripHttp.Request
{
    public class NtripConnectMountpointHttpRequestMessage : NtripHttpRequestMessage
    {
        public NtripConnectMountpointHttpRequestMessage(string host, int port, string mountpoint)
            : base(host, port)
        {
            Mountpoint = mountpoint;
        }

        public NtripConnectMountpointHttpRequestMessage(string host, int port, string username, string password, string mountpoint)
            : base(host, port, username, password)
        {
            Mountpoint = mountpoint;
        }

        public string Mountpoint { get; set; }

        public override byte[] GetBytes()
        {
            // Create the Basic Authentication string
            string auth = Convert.ToBase64String(Encoding.ASCII.GetBytes(Username + ":" + Password));

            // Send NTRIP request
            string ntripRequest = "GET /" + Mountpoint + " HTTP/1.1\r\n" +
                                  "Host: " + Host + "\r\n" +
                                  "Ntrip-Version: Ntrip/2.0\r\n" +
                                  "User-Agent: NTRIP Client/1.0\r\n" +
                                  "Connection: close\r\n";

            if (!String.IsNullOrEmpty(Username) && !String.IsNullOrEmpty(Password))
                ntripRequest += "Authorization: Basic " + auth + "\r\n";

            ntripRequest += "\r\n";

            return Encoding.ASCII.GetBytes(ntripRequest);
        }
    }
}
