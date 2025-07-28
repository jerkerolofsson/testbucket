using TestBucket.AdbProxy.DeviceIntegrationTests.Framework;
using TestBucket.Traits.Xunit;

namespace TestBucket.AdbProxy.DeviceIntegrationTests
{
    [EnrichedTest]
    [IntegrationTest]
    [Component("ADB Device Handling")]
    [Feature("Test Resources")]
    public class AdbDeviceHandlingTests(AdbProxyTestFramework Fixture)
    {
        /// <summary>
        /// Verifies that atleast one device is found
        /// </summary>
        [Fact]
        public void GetSerials_DeviceFound()
        {
            var devices = Fixture.GetSerials();
            Assert.NotEmpty(devices);
        }

        /// <summary>
        /// Verifies that atleast one device is found
        /// </summary>
        [Fact]
        public void GetDevices_DeviceFound()
        {
            var devices = Fixture.GetDevices();
            Assert.NotEmpty(devices);
        }

        /// <summary>
        /// Verifies that the devices has a valid connection url
        /// </summary>
        [Fact]
        public void GetDevices_DeviceHasConnectionUrl()
        {
            var devices = Fixture.GetDevices();
            Assert.NotEmpty(devices);
            foreach (var device in devices)
            {
                Assert.NotNull(device.Url);
                Assert.NotEmpty(device.Url);

                Assert.True(device.Url.Contains(':'), "Expected connection url to contain :");
            }
        }
    }
}
