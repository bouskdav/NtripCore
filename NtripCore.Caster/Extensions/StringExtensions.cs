using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtripCore.Caster.Extensions
{
    public static class StringExtensions
    {
        public static string DecodeBase64(this string base64String)
        {
            var bytes = System.Convert.FromBase64String(base64String);
            return Encoding.UTF8.GetString(bytes);
        }
    }
}
