using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtripCore.Caster.Utility
{
    public static class StringExtensions
    {
        public static string SetDefaultValueIfNullOrEmpty(this string input, string defaultValue)
        {
            if (String.IsNullOrEmpty(input))
                return defaultValue;

            return input;
        }
    }
}
