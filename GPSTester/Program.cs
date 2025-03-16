using Asv.Gnss;
using Ghostware.GPS.NET;
using Ghostware.GPS.NET.Models.ConnectionInfo;
using Ghostware.GPS.NET.Models.Events;

namespace GPSTester
{
    internal class Program
    {
        private static GpsService? _gpsService;

        static async Task Main(string[] args)
        {
            Console.WriteLine("GPSTester");

            var gpsdClient = new GpsdClient();

            //if (!gpsdClient.CheckGpsdRunning())
            //{
            //    Console.WriteLine("starting gpsd...");

            //    gpsdClient.StartGpsd();

            //    Console.WriteLine("started gpsd!");
            //}
            //else
            //{
            //    Console.WriteLine("gpsd already running");
            //}

            //var info = new GpsdInfo()
            //{
            //    Address = "127.0.0.1",
            //    Port = 2947,
            //    IsProxyEnabled = false,
            //};
            //_gpsService = new GpsService(info);

            //_gpsService.RegisterDataEvent(GpsdServiceOnLocationChanged);
            //_gpsService.Connect();

            string portName = "/dev/ttyACM0";  // Change this to match your device
            int baudRate = 115200; // Default for ZED-F9P

            Console.WriteLine($"Connecting to UBX:");

            var device = new UbxDevice($"serial:{portName}?br={baudRate}");

            await device.SetupByDefault();
            //await device.TurnOffNmea();
            //await device.HardwareReset();
            //await device.GetNavPvt();

            //await device.SetMessageRate(41, 7, 1, CancellationToken.None);

            //await device.SetSurveyInMode(minDuration: 60, positionAccuracyLimit: 5);
            await device.SetFixedBaseMode(new Asv.Common.GeoPoint(49.9352235, 14.2797668, 251.701));

            //device.Connection.Filter<RtcmV3Msm4>().Subscribe(_ => { /* do something with RTCM */ });
            //device.Connection.Filter<RtcmV3Msm4>().Subscribe(i =>
            device.Connection.Filter<RtcmV3MessageBase>().Subscribe(i =>
            {
                Console.WriteLine($"{i.MessageId} - {i.Name}");
            });
            device.Connection.Filter<RtcmV3Message1005and1006>().Subscribe(i =>
            {
                Console.WriteLine($"{i.MessageId} - {i.Name} - {i.ToString()}");
            });
            device.Connection.GetRtcmV3RawMessages().Subscribe(i =>
            {
                Console.WriteLine($"{i.MessageId}");
            });
            //device.Connection.Filter<UbxAckBase>().Subscribe(i =>
            //{
            //    Console.WriteLine($"{i.MessageId} - {i.Name}");
            //});
            device.Connection.OnMessage.Subscribe(i =>
            {
                Console.WriteLine($"{i.MessageStringId} - {i.Name}: {i.ToString()}");
            });

            Console.WriteLine($"Getting CFG3 mode:");
            Console.WriteLine(await device.GetCfgTMode3());

            while (true)
            {
                string? input = Console.ReadLine();

                if (String.IsNullOrEmpty(input))
                    continue;

                //if (input == "start")
                //    await gpsdClient.RunAsync();

                //if (input == "start")
                //    await gpsdClient.RunAsync();

                if (input == "exit")
                    break;
            }

            gpsdClient.StopGpsd();
        }

        private static void GpsdServiceOnLocationChanged(object sender, GpsDataEventArgs e)
        {
            //Console.WriteLine(e.Location.ToString());
        }
    }
}
