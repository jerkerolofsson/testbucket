using TestBucket.AdbProxy.DeviceIntegrationTests.Framework;
using TestBucket.Traits.Xunit;

namespace TestBucket.AdbProxy.DeviceIntegrationTests
{
    [EnrichedTest]
    [IntegrationTest]
    [Component("Appium")]
    [Feature("Test Resources")]
    public class AppiumTests(AdbProxyTestFramework Fixture)
    {
        /// <summary>
        /// Verifies that the ApppiumUrl is assigned
        /// </summary>
        [Fact]
        public void GetDevices_DeviceHasAppiumUrl()
        {
            var devices = Fixture.GetDevices();
            Assert.NotEmpty(devices);
            foreach (var device in devices)
            {
                Assert.NotNull(device.AppiumUrl);
                Assert.NotEmpty(device.AppiumUrl);
            }
        }
    }
}
