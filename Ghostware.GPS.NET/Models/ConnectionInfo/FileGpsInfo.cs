using Ghostware.GPS.NET.Enums;

namespace Ghostware.GPS.NET.Models.ConnectionInfo
{
    public class FileGpsInfo : BaseGpsInfo
    {
        public string FilePath { get; set; } = "test";
        public FileType FileType { get; set; } = FileType.Nmea;

        public FileGpsInfo()
        {
            ReadFrequenty = 1000;
        }

        public FileGpsInfo(string filePath, FileType fileType = FileType.Nmea, int readFrequenty = 1000)
        {
            FilePath = filePath;
            FileType = fileType;
            ReadFrequenty = readFrequenty;
        }
    }
}
