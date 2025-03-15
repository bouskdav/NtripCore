using NtripCore.Caster.Utility.Sources;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace NtripCore.Caster.Core.NtripHttp.Response
{
    public class NtripGetSourceTableHttpResponseMessage
    {
        private readonly string _content;

        private readonly ReadOnlyDictionary<string, string> _headers;
        private readonly ReadOnlyDictionary<string, NtripStrRecord> _streams;
        //private readonly ReadOnlyDictionary<string, string> _networks;
        //private readonly ReadOnlyDictionary<string, string> _casters;

        private readonly int _statusCode = 0;
        private readonly string _statusMessage = null;

        public NtripGetSourceTableHttpResponseMessage(string content) 
        {
            _content = content;

            StringReader reader = new StringReader(_content);

            // at first read headers
            bool readHeaders = true;
            // then switch to body
            bool readBody = false;

            // containers
            Dictionary<string, string> headers = new Dictionary<string, string>();
            Dictionary<string, NtripStrRecord> streams = new Dictionary<string, NtripStrRecord>();
            //Dictionary<string, string> networks = new Dictionary<string, string>();
            //Dictionary<string, string> casters = new Dictionary<string, string>();

            int lineNumber = 0;

            do
            {
                lineNumber++;
                string line = reader.ReadLine();

                // if line is ENDSOURCETABLE, break
                if (line == "ENDSOURCETABLE")
                    break;

                // if end of string, break
                if (line == null)
                    break;

                // first line should contain status code
                if (lineNumber == 1)
                {
                    var fields = line.Split(' ');

                    // first field would contain HTTP or SOURCETABLE
                    // second field should contain code
                    // third field should contain message

                    _statusCode = Convert.ToInt32(fields[1]);
                    _statusMessage = fields[2];

                    // handle different status codes
                    if (_statusCode >= 300)
                        break;

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
                    if (line.StartsWith("STR"))
                    {
                        try
                        {
                            NtripStrRecord strRecord = new NtripStrRecord(line);
                            streams.Add(strRecord.Mountpoint, strRecord);
                        }
                        catch (Exception ex)
                        {

                        }
                    }
                }
            }
            while (true);

            // assign headers
            _headers = new ReadOnlyDictionary<string, string>(headers);
            _streams = new ReadOnlyDictionary<string, NtripStrRecord>(streams);
        }

        public NtripSourceTable AsNtripSourceTable()
        {
            return new NtripSourceTable(_streams);
        }
    }
}
