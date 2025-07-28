using System.Threading.Tasks;
using TestBucket.AdbProxy.Host;
using TestBucket.Traits.Xunit;

namespace TestBucket.AdbProxy.DeviceIntegrationTests
{
    [EnrichedTest]
    [IntegrationTest]
    [Component("ADB Protocol")]
    [Feature("Test Resources")]
    public class AdbHostTests()
    {
        /// <summary>
        /// Verifies that a command can be sent to the ADB host
        /// </summary>
        [Fact]
        [FunctionalTest]
        public async Task ListDevicesAsync_WithLocalhostAdb()
        {
            var client = new AdbHostClient(new Proxy.AdbProxyOptions());
            var devices = await client.ListDevicesAsync(TestContext.Current.CancellationToken);
            Assert.NotEmpty(devices);
        }

        private async Task TestListDevices()
        {
            var client = new AdbHostClient(new Proxy.AdbProxyOptions());
            var devices = await client.ListDevicesAsync(TestContext.Current.CancellationToken);
            if (devices.Length == 0)
            {
                throw new System.Exception("Failed to list devices");
            }
        }

        /// <summary>
        /// Verifies that a command can be sent to the ADB host
        /// </summary>
        [Fact]
        [ReliabilityTest]
        public async Task ListDevicesAsync_WithMultipleConcurrentThreads()
        {
            List<Task> tasks = [];
            for(int i=0; i<10000; i++)
            {
                tasks.Add(TestListDevices());
            }

            await Task.WhenAll(tasks);

            var failedTasks = tasks.Where(x => x.IsFaulted).Count();
            if(failedTasks > 0)
            {
                Assert.Fail($"{failedTasks} failed");
            }
        }
    }
}
