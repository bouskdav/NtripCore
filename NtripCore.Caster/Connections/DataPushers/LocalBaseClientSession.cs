using Asv.Gnss;
using Asv.IO;
using NtripCore.Caster.Connections.DataPushers.Abstraction;
using NtripCore.Caster.Connections.DataPushers.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace NtripCore.Caster.Connections.DataPushers
{
    public class LocalBaseClientSession : INtripCorrectionSource
    {
        private string _mountpointName;
        private UbxDevice device = null;

        public LocalBaseClientSession(string mountpointName, string connection)
        {
            Connection = connection;
            _mountpointName = mountpointName;

            //string portName = "COM8";  // Change this to match your device
            //int baudRate = 115200; // Default for ZED-F9P

            //var device = new UbxDevice($"serial:{portName}?br={baudRate}");
            //await device.SetupByDefault();
            ////await device.SetMessageRate<RtcmV3Message1004>();
            //await device.SetSurveyInMode(minDuration: 60, positionAccuracyLimit: 2);
            ////device.Connection.Filter<RtcmV3Msm4>().Subscribe(_ => { /* do something with RTCM */ });
            //device.Connection.Filter<RtcmV3Msm4>().Subscribe(i =>
            //{
            //    Console.WriteLine(i);
            //    return;
            //});
        }

        public string MountpointName => _mountpointName;

        public string Connection { get; }

        public event StreamDataReceivedEventHandler StreamDataReceived;

        public void DisconnectAndStop()
        {
            device?.Connection?.Dispose();
            device?.Dispose();
        }

        public async Task<bool> SendConnectionRequest(bool persistConnection = false)
        {
            try
            {
                device = new UbxDevice(Connection);

                await device.SetupByDefault();
                
                //await device.SetMessageRate<RtcmV3Message1004>(1);
                //await device.SetMessageRate<RtcmV3Message1004>(1);
                
                //await device.SetSurveyInMode(minDuration: 60, positionAccuracyLimit: 5);
                await device.SetFixedBaseMode(new Asv.Common.GeoPoint(49.9352235, 14.2797668, 251.701));
                //await device.SetFixedBaseMode(new );
                //device.Connection.Filter<RtcmV3Msm4>().Subscribe(_ => { /* do something with RTCM */ });
                //device.Connection.Filter<RtcmV3Msm4>().Subscribe(i =>
                //{
                //    StreamDataReceivedEventArgs args = new StreamDataReceivedEventArgs();
                //    args.MountpointName = MountpointName;
                //    //args.Data = i.Serialize(ref args.Data);
                //    OnStreamDataReceived(args);
                //});

                device.Connection.GetRtcmV3RawMessages().Subscribe(i =>
                {
                    Console.WriteLine(i.MessageId);

                    StreamDataReceivedEventArgs args = new StreamDataReceivedEventArgs();
                    args.MountpointName = MountpointName;
                    args.Data = i.RawData;
                    OnStreamDataReceived(args);
                });

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        protected virtual void OnStreamDataReceived(StreamDataReceivedEventArgs e)
        {
            StreamDataReceivedEventHandler handler = StreamDataReceived;
            if (handler != null)
            {
                handler(this, e);
            }
        }
    }
}
