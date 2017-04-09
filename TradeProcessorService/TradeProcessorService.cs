using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using TradeData;
using NLog;

namespace TradeProcessorServices
{
    public class TradeProcessorService : ITradeProcessorService
    {
        private const float LotSize = 100000f;
        
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
       
        public IList<string> ReadStream(Stream stream)
        {
            var lines = new List<string>();
            if (stream != null)
            {
                using (var reader = new StreamReader(stream))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        lines.Add(line);
                    }
                }
            }
            return lines;
        }

        public IList<TradeRecord> ProcessLines(IList<string> lines)
        {
            if (lines == null)
            {
                Logger.Error("lines should not be null");
                return new List<TradeRecord>();
            }
            return lines.Select(ParseLine).Where(x => x != null).ToList();
        }

        private TradeRecord ParseLine(string line, int index)
        {
            var fields = line.Split(new char[] { ',' });
            int tradeAmount;
            decimal tradePrice;
            if (ValidateLine(fields, index, out tradeAmount, out tradePrice))
            {
                var sourceCurrencyCode = fields[0].Substring(0, 3);
                var destinationCurrencyCode = fields[0].Substring(3, 3);

                // Calculate values
                var trade = new TradeRecord
                {
                    SourceCurrency = sourceCurrencyCode,
                    DestinationCurrency = destinationCurrencyCode,
                    Lots = tradeAmount / LotSize,
                    Price = tradePrice
                };
                return trade;
            }
            return null;
           
        }

        private bool ValidateLine(string[] fields, int index, out int tradeAmount, out decimal tradePrice)
        {
            tradeAmount = 0;
            tradePrice = 0;

            if (fields.Length != 3)
            {
                Logger.Warn("Line {0} malformed. Only {1} field(s) found.", index, fields.Length);
                return false;
            }

            if (fields[0].Length != 6)
            {
                Logger.Warn("Trade currencies on line {0} malformed: [{1}]", index, fields[0]);
                return false;
            }

            if (!int.TryParse(fields[1], out tradeAmount))
            {
                Logger.Warn("Trade amount on line {0} is not a valid integer: [{1}]", index, fields[1]);
                return false;
            }
            if (tradeAmount < 0)
            {
                Logger.Warn("Trade amount on line {0} should not be negative: [{1}]", index, fields[1]);
                return false;
            }

            if (!decimal.TryParse(fields[2], out tradePrice))
            {
                Logger.Warn("Trade price on line {0} is not a valid decimal: [{1}]", index, fields[1]);
                return false;
            }
            if (tradePrice < 0)
            {
                //Console.WriteLine($"WARN: Trade price on line {0} should not be negative: [{fields[1]}]");
                Logger.Warn("Trade price on line {0} should not be negative: [{1}]", index, fields[1]);
                return false;
            }
            return true;
        }

        
        
    }
}
