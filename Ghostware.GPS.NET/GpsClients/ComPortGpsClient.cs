using System;
using System.IO.Ports;
using System.Threading;
using Ghostware.GPS.NET.Enums;
using Ghostware.GPS.NET.Models.ConnectionInfo;
using Ghostware.GPS.NET.Models.Events;
using Ghostware.NMEAParser;
using Ghostware.NMEAParser.Exceptions;
using Ghostware.NMEAParser.NMEAMessages;

namespace Ghostware.GPS.NET.GpsClients
{
    //public class ComPortGpsClient : BaseGpsClient
    //{
    //    #region Private Properties

    //    private readonly NmeaParser _parser = new NmeaParser();
    //    private SerialPort _serialPort;

    //    private DateTime? _previousReadTime;

    //    #endregion

    //    #region Constructors

    //    public ComPortGpsClient(ComPortInfo connectionData) : base(GpsType.ComPort, connectionData)
    //    {
    //    }

    //    public ComPortGpsClient(BaseGpsInfo connectionData) : base(GpsType.ComPort, connectionData)
    //    {
    //    }

    //    #endregion

    //    #region Connect and Disconnect

    //    public override bool Connect()
    //    {
    //        var data = (ComPortInfo)GpsInfo;

    //        IsRunning = true;
    //        OnGpsStatusChanged(GpsStatus.Connecting);
    //        _serialPort = new SerialPort(data.ComPort, 9600, Parity.None, 8, StopBits.One);

    //        // Attach a method to be called when there
    //        // is data waiting in the port's buffer
    //        _serialPort.DataReceived += port_DataReceived;
    //        try
    //        {
    //            // Begin communications
    //            _serialPort.Open();

    //            OnGpsStatusChanged(GpsStatus.Connected);
    //            // Enter an application loop to keep this thread alive
    //            while (_serialPort.IsOpen)
    //            {
    //                Thread.Sleep(data.ReadFrequenty);
    //            }
    //        }
    //        catch
    //        {
    //            Disconnect();
    //            throw;
    //        }
            
    //        return true;
    //    }

    //    public override bool Disconnect()
    //    {
    //        _serialPort.Close();
    //        IsRunning = false;
    //        OnGpsStatusChanged(GpsStatus.Disabled);
    //        return true;
    //    }

    //    #endregion

    //    #region Location Callbacks

    //    private void port_DataReceived(object sender, SerialDataReceivedEventArgs e)
    //    {
    //        try
    //        {
    //            var readString = _serialPort.ReadExisting();
    //            OnRawGpsDataReceived(readString);
    //            var result = _parser.Parse(readString);
    //            if (typeof(GprmcMessage) != result.GetType()) return;
    //            if (_previousReadTime != null && GpsInfo.ReadFrequenty != 0 && ((GprmcMessage)result).UpdateDate.Subtract(new TimeSpan(0, 0, 0, 0, GpsInfo.ReadFrequenty)) <= _previousReadTime) return;
    //            OnGpsDataReceived(new GpsDataEventArgs((GprmcMessage)result));
    //        }
    //        catch (UnknownTypeException ex)
    //        {
    //            Console.WriteLine(ex.Message);
    //        }
    //    }

    //    #endregion
    //}
}