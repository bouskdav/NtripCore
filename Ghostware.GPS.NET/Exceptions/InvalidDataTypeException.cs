using System;

namespace Ghostware.GPS.NET.Exceptions
{
    public class InvalidDataTypeException : Exception
    {
        public InvalidDataTypeException() : base("You provided an invalid datatype for this type of client!")
        {
            
        }
    }
}
