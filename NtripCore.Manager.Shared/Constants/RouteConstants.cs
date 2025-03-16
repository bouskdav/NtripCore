using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtripCore.Manager.Shared.Constants
{
    public static class RouteConstants
    {
        private const string ApiBase = "/api";

        public static class Weather
        {
            public const string GetWeather = ApiBase + "/weather";
        }

        public static class BaseStation
        {
            private const string BaseStationBase = ApiBase + "/base-station";

            public const string GetSystemState = ApiBase + BaseStationBase + "/system-state";
            public const string SetOperatingMode = ApiBase + BaseStationBase + "/operating-mode";
            public const string SetGpsServiceState = ApiBase + BaseStationBase + "/gps-service-state";
        }
    }
}
