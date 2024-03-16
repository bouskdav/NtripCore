using System.Collections.Generic;

namespace NtripCore.Caster.Configs
{

    public class User
    {
        public string Name { get; set; }
        public string Password { get; set; }
        public IList<string> Mountpoints { get; set; }

        public User()
        {
            Mountpoints = new List<string>();
        }
    }
}
