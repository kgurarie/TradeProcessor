using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using NLog;
using TradeData;

namespace TradeProcessorServices
{
    public class DatabaseService : IDatabaseService
    {
        //private const int batchSize = 100;

        private readonly IConnectionStringProvider _connectionStringProvider;
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public DatabaseService(IConnectionStringProvider connectionStringProvider)
        {
            this._connectionStringProvider = connectionStringProvider;
        }

        public void WriteData(IList<TradeRecord> trades)
        {
            // TODO: For the future: If file contains lots of records, write records in batches 
            // Define batch size as a constant (for example, 100 records)
            // Loop on batches, write each batch as one transaction
            // Also it would be good to add check if the record already exists in the database, so the stream can be processed again
            // in case of errors
            //var count = trades.Count / batchSize;
            //if (trades.Count % batchSize > 0)
            //{
            //    count++;
            //}
            //for(int i = 0; i < count; i++)
            //{
            //    var tradesBatch = trades.Skip(i * batchSize).Take(batchSize);
            //    using (var connection = new SqlConnection(_connectionStringProvider.ConnectionString))
            //    {
            //        try
            //        {
            //            connection.Open();
            //            using (var transaction = connection.BeginTransaction())
            //            {
            //                foreach (var trade in tradesBatch)
            //                {
            //                    var command = connection.CreateCommand();
            //                    command.Transaction = transaction;
            //                    command.CommandType = CommandType.StoredProcedure;
            //                    command.CommandText = "dbo.insert_trade";
            //                    command.Parameters.AddWithValue("@sourceCurrency", trade.SourceCurrency);
            //                    command.Parameters.AddWithValue("@destinationCurrency", trade.DestinationCurrency);
            //                    command.Parameters.AddWithValue("@lots", trade.Lots);
            //                    command.Parameters.AddWithValue("@price", trade.Price);

            //                    command.ExecuteNonQuery();
            //                }
            //                transaction.Commit();
            //            }
            //        }
            //        catch (Exception ex)
            //        {
            //            logger.Error("Error saving records to the database");
            //        }
            //        finally
            //        {
            //            connection.Close();
            //        }
            //    }
            //}

            using (var connection = new SqlConnection(_connectionStringProvider.ConnectionString))
            {
                try
                {
                    connection.Open();
                    using (var transaction = connection.BeginTransaction())
                    {
                        foreach (var trade in trades)
                        {
                            var command = connection.CreateCommand();
                            command.Transaction = transaction;
                            command.CommandType = CommandType.StoredProcedure;
                            command.CommandText = "dbo.insert_trade";
                            command.Parameters.AddWithValue("@sourceCurrency", trade.SourceCurrency);
                            command.Parameters.AddWithValue("@destinationCurrency", trade.DestinationCurrency);
                            command.Parameters.AddWithValue("@lots", trade.Lots);
                            command.Parameters.AddWithValue("@price", trade.Price);

                            command.ExecuteNonQuery();
                        }

                        transaction.Commit();
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error("Error saving records to the database");
                    throw;
                }
                finally
                {
                    connection.Close();
                }

            }
        }
    }
}