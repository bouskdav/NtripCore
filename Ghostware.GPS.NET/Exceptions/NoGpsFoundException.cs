using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ghostware.GPS.NET.Exceptions
{
    public class NoGpsFoundException : Exception
    {
        public NoGpsFoundException() : base("There is not gps found in this device!")
        {
            
        }
    }
}
