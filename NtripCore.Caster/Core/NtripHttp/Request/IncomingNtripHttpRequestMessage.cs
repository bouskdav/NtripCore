using NtripCore.Caster.Extensions;
using NtripCore.Caster.Utility.Sources;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;

namespace NtripCore.Caster.Core.NtripHttp.Request
{
    public class IncomingNtripHttpRequestMessage
    {
        private readonly string _content;

        private readonly ReadOnlyDictionary<string, string> _headers;

        private readonly string _method;
        private readonly string _path;

        private readonly bool _authenticated;
        private readonly string _authenticationType;
        private readonly string _username;
        private readonly string _password;

        private int _version;

        public IncomingNtripHttpRequestMessage(string content) 
        {
            _content = content;

            StringReader reader = new StringReader(_content);

            // at first read headers
            bool readHeaders = true;
            // then switch to body
            bool readBody = false;

            // containers
            Dictionary<string, string> headers = new Dictionary<string, string>();

            int lineNumber = 0;

            do
            {
                lineNumber++;
                string line = reader.ReadLine();

                // if end of string, break
                if (line == null)
                    break;

                // first line should contain status code
                if (lineNumber == 1)
                {
                    var fields = line.Split(' ');

                    // first field would contain GET
                    // second field should contain path
                    // third field should contain http protocol version

                    _method = fields[0];
                    _path = fields[1];

                    continue;
                }

                // if line is empty, switch reading mode and continue
                if (String.IsNullOrEmpty(line) && readHeaders && !readBody)
                {
                    readHeaders = false;
                    readBody = true;

                    continue;
                }

                // if read headers
                if (readHeaders)
                {
                    string name = null;
                    string value = null;

                    if (line.Contains(": "))
                    {
                        int delimiter = line.IndexOf(": ");

                        name = line.Substring(0, delimiter);
                        value = line.Substring(delimiter + 2);
                    }
                    else if (line.Contains(":"))
                    {
                        name = line.Substring(0, line.Length - 2);
                    }
                    else
                    {
                        name = line.Trim();
                    }

                    headers.Add(name, value);
                }

                // if read body
                if (readBody)
                {
                    // TODO: can body contain something?
                }
            }
            while (true);

            // assign headers
            _headers = new ReadOnlyDictionary<string, string>(headers);

            // check authentication
            _authenticated = _headers.ContainsKey("Authorization");

            string authorizationString = _headers["Authorization"];

            var authorizationParts = authorizationString.Split(new char[] { ' ' });
            // Basic Auth
            if (authorizationParts.Length == 2 && authorizationParts[0].Equals("Basic", StringComparison.OrdinalIgnoreCase))
            {
                _authenticationType = authorizationParts[0];

                var usernamePassword = authorizationParts[1].DecodeBase64().Split(new char[] { ':' });
                if (usernamePassword.Length == 2)
                {
                    _username = usernamePassword[0];
                    _password = usernamePassword[1];
                }
            }

            // check ntrip version
            _version = 1;
            if (_headers.ContainsKey("Ntrip-Version"))
            {
                var versionHeader = _headers["Ntrip-Version"];
                if (versionHeader == "Ntrip/2.0")
                    _version = 2;
            }
        }

        public bool IsNtripClient => _headers["User-Agent"]?.Contains("NTRIP", StringComparison.InvariantCultureIgnoreCase) ?? false;

        public bool IsSourceTableRequested => _path == "/";

        public string RequestedMountpointName => _path?.Replace("/", "");

        public bool Authenticated => _authenticated;

        public string Username => _username;

        public string Password => _password;

        public bool IsVersion2 => _version == 2;
    }
}
