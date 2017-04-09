using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using TradeData;

namespace TradeProcessorServices
{
    public interface ITradeProcessorService
    {
        IList<string> ReadStream(Stream stream);
        IList<TradeRecord> ProcessLines(IList<string> lines);
      
    }
}
