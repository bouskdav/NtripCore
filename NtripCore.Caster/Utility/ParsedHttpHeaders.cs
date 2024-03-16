using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtripCore.Caster.Utility
{
    public class ParsedHttpHeaders
    {
        public string Mountpoint;
        public string Username;
        public string Password;
        public bool IsSource;
        public string Agent;

        public bool IsNTRIP => Agent.Contains("NTRIP");

        public static ParsedHttpHeaders Parse(string headers)
        {
            var parsedHeaders = new ParsedHttpHeaders();

            foreach (var header in headers.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries))
            {
                // GET is first header
                if (header.StartsWith("GET ", StringComparison.Ordinal))
                {
                    // HTTP GET
                    parsedHeaders.Mountpoint = NormalizeMountpoint(header.Split(new char[] { ' ' })[1]);
                }
                // Basic auth header
                else if (header.StartsWith("Authorization:", StringComparison.Ordinal))
                {
                    // Basic Auth
                    var authorizationParts = header.Split(new char[] { ' ' });
                    if (authorizationParts.Length == 2 && authorizationParts[0].Equals("Basic", StringComparison.OrdinalIgnoreCase))
                    {
                        var usernamePassword = DecodeBase64(authorizationParts[1]).Split(new char[] { ':' });
                        if (usernamePassword.Length == 2)
                        {
                            parsedHeaders.Username = usernamePassword[0];
                            parsedHeaders.Password = usernamePassword[1];
                        }
                    }
                }
                // 
                else if (header.StartsWith("SOURCE", StringComparison.Ordinal))
                {
                    // Source
                    parsedHeaders.IsSource = true;
                    var sourceParts = header.Split(new char[] { ' ' });
                    if (sourceParts.Length == 3)
                    {
                        parsedHeaders.Password = sourceParts[1];
                        parsedHeaders.Mountpoint = NormalizeMountpoint(sourceParts[2]);
                    }
                }
                else if (header.StartsWith("User-Agent:", StringComparison.Ordinal))
                {
                    parsedHeaders.Agent = header.Substring("User-Agent:".Length).TrimStart();
                }
                else if (header.StartsWith("Source-Agent:", StringComparison.Ordinal))
                {
                    parsedHeaders.Agent = header.Substring("Source-Agent:".Length).TrimStart();
                }
                //else
                //{
                //    Console.WriteLine("ignore header: " + header);
                //}
            }

            return parsedHeaders;
        }

        private static string DecodeBase64(string base64String)
        {
            var bytes = System.Convert.FromBase64String(base64String);
            return Encoding.UTF8.GetString(bytes);
        }

        private static string NormalizeMountpoint(string mountpoint)
        {
            if (mountpoint == null)
            {
                return String.Empty;
            }
            return mountpoint.ToLowerInvariant().TrimStart(new char[] { '/' });
        }
    }
}
