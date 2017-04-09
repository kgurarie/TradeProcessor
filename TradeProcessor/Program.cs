using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Autofac;
using TradeProcessorServices;
using NLog;
using NLog.Internal;
using TradeData;


namespace TradeProcessor
{
    class Program
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        static void Main(string[] args)
        {
            if (!args.Any())
            {
                logger.Error("Please provide file path as an argument;");
                return;
            }
            var filePath = args[0];
            if (string.IsNullOrWhiteSpace(filePath))
            {
                logger.Error("Please provide file path as an argument;");
                return;
            }
            if (!File.Exists(filePath))
            {
                logger.Error("File {0} does not exist", filePath);
                return;
            }

            try
            {
                Stream fs = File.OpenRead(filePath);
                var cb = new ContainerBuilder();
                var container = AutofacConfiguration.ConfigureAutofacContainer(cb);

                ProcessStream(container, fs);
            }
            catch(Exception ex)
            {
                logger.Error("Exception occurred");
            }
        }
              
        private static void ProcessStream(IContainer container, Stream fs)
        {
            var service = container.Resolve<ITradeProcessorService>();
            var lines = service.ReadStream(fs);
            if (!lines.Any())
            {
                // nothing to process
                logger.Error("File {0} does not contain any data");
            }
            var tradeRecords = service.ProcessLines(lines);
            if (tradeRecords.Any())
            {

                // Write to the database 
                var connStringProvider = container.Resolve<IConnectionStringProvider>();
                var databaseService = container.Resolve<IDatabaseService>();

                var connString = System.Configuration.ConfigurationManager.ConnectionStrings["defaultConnectionString"];
                if (string.IsNullOrWhiteSpace(connString?.ToString()))
                {
                    logger.Error("Database connection string is not specified");
                    return;
                }

                connStringProvider.ConnectionString = connString.ToString();
                databaseService.WriteData(tradeRecords);
            }


        }
    }
}
