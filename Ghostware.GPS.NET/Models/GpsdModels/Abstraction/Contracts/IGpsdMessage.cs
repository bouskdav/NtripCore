using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ghostware.GPS.NET.Models.GpsdModels.Abstraction.Contracts
{
    public interface IGpsdMessage
    {
        public DateTime Time { get; set; }
    }
}
