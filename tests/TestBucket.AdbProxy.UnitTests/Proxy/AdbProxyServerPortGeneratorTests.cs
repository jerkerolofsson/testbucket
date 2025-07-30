using System;
using Microsoft.Extensions.Options;
using TestBucket.AdbProxy.Proxy;
using TestBucket.Traits.Xunit;
using Xunit;

namespace TestBucket.AdbProxy.UnitTests.Proxy
{
    [EnrichedTest]
    [UnitTest]
    [Component("ADB Proxy")]
    [Feature("Test Resources")]
    public class AdbProxyServerPortGeneratorTests
    {
        /// <summary>
        /// Tests that the GetNextPort method returns the correct port and cycles back to the start when exceeding MaxListenPort.
        /// </summary>
        [Fact]
        public void GetNextPort_ShouldCyclePortsCorrectly()
        {
            // Arrange
            var options = Options.Create(new AdbProxyOptions
            {
                ListenPort = 15037,
                MaxListenPort = 15039
            });
            var portGenerator = new AdbProxyServerPortGenerator(options);

            // Act & Assert
            Assert.Equal(15037, portGenerator.GetNextPort());
            Assert.Equal(15038, portGenerator.GetNextPort());
            Assert.Equal(15039, portGenerator.GetNextPort());
            Assert.Equal(15037, portGenerator.GetNextPort()); // Cycles back
        }

        /// <summary>
        /// Tests that the GetNextPort method works correctly when MaxListenPort equals ListenPort.
        /// </summary>
        [Fact]
        public void GetNextPort_ShouldHandleSinglePortRange()
        {
            // Arrange
            var options = Options.Create(new AdbProxyOptions
            {
                ListenPort = 15037,
                MaxListenPort = 15037
            });
            var portGenerator = new AdbProxyServerPortGenerator(options);

            // Act & Assert
            Assert.Equal(15037, portGenerator.GetNextPort());
            Assert.Equal(15037, portGenerator.GetNextPort()); // Always returns the same port
        }
    }
}
