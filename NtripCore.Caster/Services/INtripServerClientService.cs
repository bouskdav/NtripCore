using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtripCore.Caster.Services
{
    public interface INtripServerClientService
    {
        public Task InitializeAsync();

        public Task SubscribeClientToServerAsync(string client, string server);
    }
}
