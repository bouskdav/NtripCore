namespace Ghostware.GPS.NET.Models.ConnectionInfo
{
    public class ComPortInfo : BaseGpsInfo
    {
        public string ComPort { get; set; } = "ComPort1";

        public ComPortInfo()
        {
            ReadFrequenty = 1000;
        }

        public ComPortInfo(string comPort, int readFrequenty = 1000)
        {
            ComPort = comPort;
            ReadFrequenty = readFrequenty;
        }
    }
}
