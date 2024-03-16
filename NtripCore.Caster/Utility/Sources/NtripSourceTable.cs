using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtripCore.Caster.Utility.Sources
{
    public class NtripSourceTable
    {
        private readonly ReadOnlyDictionary<string, NtripStrRecord> _streams;

        public NtripSourceTable(ReadOnlyDictionary<string, NtripStrRecord> streams) 
        {
            _streams = streams;
        }

        public ReadOnlyDictionary<string, NtripStrRecord> Streams => _streams;

        public NtripStrRecord Get(string key) 
        {
            return _streams[key];
        }
    }
}
