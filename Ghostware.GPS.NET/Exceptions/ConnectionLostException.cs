using System;

namespace Ghostware.GPS.NET.Exceptions
{
    public class ConnectionLostException : Exception
    {
        public ConnectionLostException() : base("The connection is lost.")
        {

        }
    }
}
