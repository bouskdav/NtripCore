using System;
using Ghostware.GPS.NET.Enums;

namespace Ghostware.GPS.NET.Models.Events
{
    public class GpsStatusEventArgs : EventArgs
    {
        public GpsStatus Status { get; set; }

        public GpsStatusEventArgs()
        {
            
        }

        public GpsStatusEventArgs(GpsStatus status)
        {
            Status = status;
        }
    }
}
