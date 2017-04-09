using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using FluentAssertions;
using TradeProcessorServices;

namespace TradeProcessorServicesTest
{
    [TestClass]
    public class DatabaseServiceTests
    {
        private Mock<IConnectionStringProvider> _connectionStringProvider;
        private IDatabaseService _databaseService;

        [TestInitialize()]
        public void Setup()
        {
            var _connectionStringProvider = new Mock<IConnectionStringProvider>();
            var connectionString = "Data Source = (local); Initial Catalog = TradeDatabase; Integrated Security = True";
            _connectionStringProvider.SetupGet(x => x.ConnectionString).Returns(connectionString);
            _databaseService = new DatabaseService(_connectionStringProvider.Object);

        }

        [TestMethod]
        public void WriteData_ShouldWork()
        {

        }

        [TestMethod]
        public void WriteData_ShouldThrowException()
        {

        }



    }
}