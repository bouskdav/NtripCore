using System;

namespace Ghostware.GPS.NET.Exceptions
{
    public class UnknownTypeException : Exception
    {
        public UnknownTypeException(Type type) : base($"Unknown Class Type: {type}")
        {
            
        }

        public UnknownTypeException(string type) : base($"Unknown Class Type: {type}")
        {

        }
    }
}