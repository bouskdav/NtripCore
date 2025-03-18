using Ghostware.GPS.NET.Models.GpsdModels;
using Ghostware.GPS.NET.Models.GpsdModels.Abstraction.Contracts;
using Newtonsoft.Json;

namespace Ghostware.GPS.NET.Parsers
{
    public class GpsdDataParser
    {
        public IGpsdMessage GetGpsData(string gpsData, out string cls)
        {
            cls = null;

            try
            {
                var classType = JsonConvert.DeserializeObject<DataClassType>(gpsData);

                cls = classType.Class;

                return (IGpsdMessage)JsonConvert.DeserializeObject(gpsData, classType.GetClassType());
            }
            catch (JsonReaderException)
            {
                return null;
            }
            catch (JsonSerializationException)
            {
                return null;
            }
        }
    }
}
