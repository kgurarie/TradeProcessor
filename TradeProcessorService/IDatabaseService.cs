using System.Collections.Generic;
using TradeData;

namespace TradeProcessorServices
{
    public interface IDatabaseService
    {
        void WriteData(IList<TradeRecord> trades);
    }
}