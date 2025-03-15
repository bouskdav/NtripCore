using NtripCore.Caster.Utility.Sources;
using System.Collections.ObjectModel;
using System.Text;

namespace NtripCore.Caster.Configs
{
    public class NtripSource
    {
        private readonly string _id;

        private NtripSourceTable _ntripSourceTable;

        public NtripSource() 
        { 
            _id = Guid.NewGuid().ToString();
        }

        public SourceType SourceType { get; set; } = SourceType.Default;
        public string Connection { get; set; }
        public string Mountpoint { get; set; }

        public string Host { get; set; }
        public int? Port { get; set; }
        public List<string> AllowedMountpoints { get; set; }
        public List<string> PreconnectMountpoints { get; set; }
        public bool AuthRequired { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }

        public string Id => _id;

        public Task InsertSourceTable(NtripSourceTable ntripSourceTable)
        {
            _ntripSourceTable = ntripSourceTable;

            return Task.CompletedTask;
        }

        public Task<NtripSourceTable> GetSourceTableAsync()
        {
            if (AllowedMountpoints?.Count > 0 && _ntripSourceTable?.Streams != null)
            {
                // select only allowed streams
                NtripSourceTable ntripSourceTable = new NtripSourceTable(
                    new ReadOnlyDictionary<string, NtripStrRecord>(_ntripSourceTable.Streams.Where(i => AllowedMountpoints.Contains(i.Key)).ToDictionary(i => i.Key, i => i.Value))
                );
            }

            return Task.FromResult(_ntripSourceTable);
        }

        //public string Mountpoint { get; set; }
        //public string Identifier { get; set; }
        //public string Format { get; set; }
        //public Carrier Carrier { get; set; }
        //public string NavSystem { get; set; }
        //public string Network { get; set; }
        //public string Country { get; set; }
        //public double Latitude { get; set; }
        //public double Longitude { get; set; }
        //
    }

    public enum Carrier
    {
        No = 0,
        L1 = 1,
        L1L2 = 2
    }
}
