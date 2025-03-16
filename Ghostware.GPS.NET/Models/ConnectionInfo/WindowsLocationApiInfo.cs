namespace Ghostware.GPS.NET.Models.ConnectionInfo
{
    public class WindowsLocationApiInfo : BaseGpsInfo
    {
        public int Timeout { get; set; } = 1000;

        public WindowsLocationApiInfo()
        {
            ReadFrequenty = 0;
        }
    }
}
