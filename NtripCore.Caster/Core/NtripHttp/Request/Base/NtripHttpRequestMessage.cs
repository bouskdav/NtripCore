using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtripCore.Caster.Core.NtripHttp.Request.Base
{
    public abstract class NtripHttpRequestMessage
    {
        public NtripHttpRequestMessage(string host, int port)
        {
            Host = host;
            Port = port;
        }

        public NtripHttpRequestMessage(string host, int port, string username, string password)
            : this(host, port)
        {
            Username = username;
            Password = password;
        }

        public string Host { get; set; }

        public int Port { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

        public abstract byte[] GetBytes();
    }
}
