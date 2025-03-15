using NtripCore.Caster.Connections.DataPushers.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtripCore.Caster.Connections.DataPushers.Abstraction
{
    public interface INtripCorrectionSource
    {
        string MountpointName { get; }

        event StreamDataReceivedEventHandler StreamDataReceived;

        Task<bool> SendConnectionRequest(bool persistConnection = false);

        void DisconnectAndStop();
    }
}
