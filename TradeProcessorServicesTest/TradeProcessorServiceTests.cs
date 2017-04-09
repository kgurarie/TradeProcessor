using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TradeProcessorServices;
using FluentAssertions;

namespace TradeProcessorServicesTest
{
    [TestClass]
    public class TradeProcessorServiceTests
    {
        public ITradeProcessorService _service;

        [TestInitialize()]
        public void Setup()
        {
            _service = new TradeProcessorService();
        }

        [TestCleanup]
        public void Cleanup()
        {
        }

        [TestMethod]
        public void ProcessLines_ValidLines_ShouldReturnTheSameCount()
        {
            var lines = new List<string>
            {
                "$AU$US,16000000,1600000",
                "$AU$US,2000000,3000000"
            };
            var service = new TradeProcessorService();
            var result = service.ProcessLines(lines);
            result.Count.Should().Be(lines.Count);
        }

        [TestMethod]
        public void ProcessLines_SomeValidLines_ShouldReturnValidLinesCount()
        {
            var lines = new List<string>
            {
                "$AU$US,16000000,1600000",
                "$AU$US,10",
                "$AU$US,-500000,20000",
                "$AU$US,2000000,3000000"
            };
            var result = _service.ProcessLines(lines);
            result.Count.Should().Be(2);
   
        }

        [TestMethod]
        public void ReadStream_ValidStream_ShouldReturnLines()
        {
            var stream = GenerateStreamFromString("$AU$US,16000000,1600000\n$AU$US,10\n$AU$US,-500000,20000");
            var result = _service.ReadStream(stream);
            result.Count.Should().Be(3);
        }

        private static Stream GenerateStreamFromString(string s)
        {
            MemoryStream stream = new MemoryStream();
            StreamWriter writer = new StreamWriter(stream);
            writer.Write(s);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }
    }
}
