using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtripCore.Caster.Connections.DataPushers.Events
{
    public class StreamDataReceivedEventArgs : EventArgs
    {
        public string MountpointName { get; set; }

        public byte[] Data { get; set; }
    }

    public delegate void StreamDataReceivedEventHandler(object sender, StreamDataReceivedEventArgs e);
}
