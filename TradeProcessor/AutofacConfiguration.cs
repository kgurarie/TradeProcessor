using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using TradeData;
using TradeProcessorServices;

namespace TradeProcessor
{
    public class AutofacConfiguration
    {
        public static IContainer ConfigureAutofacContainer()
        {
            return ConfigureAutofacContainer(new ContainerBuilder());
        }

        public static IContainer ConfigureAutofacContainer(ContainerBuilder containerBuilder)
        {
            containerBuilder.RegisterType<TradeProcessorService>().As<ITradeProcessorService>();
            containerBuilder.RegisterType<ConnectionStringProvider>().As<IConnectionStringProvider>().SingleInstance();
            containerBuilder.RegisterType<DatabaseService>().As<IDatabaseService>();
            return containerBuilder.Build();
        }



    }
}
