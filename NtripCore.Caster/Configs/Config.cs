using System;
using System.Collections.Generic;

namespace NtripCore.Caster.Configs
{
    public class Config
    {
        public string ServerAddress;
        public int ServerPort;
        public int MaxClients;
        public IList<NtripSource> Sources;
        public IList<User> Users;

        bool CheckConfig()
        {
            // TODO
            return true;
        }
    }
}
