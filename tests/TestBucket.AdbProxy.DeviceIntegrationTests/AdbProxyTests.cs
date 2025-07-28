using CliWrap;
using System.Threading.Tasks;
using TestBucket.AdbProxy.DeviceIntegrationTests.Framework;
using TestBucket.AdbProxy.Host;
using TestBucket.Traits.Xunit;

namespace TestBucket.AdbProxy.DeviceIntegrationTests
{
    [EnrichedTest]
    [IntegrationTest]
    [Component("ADB Proxy")]
    [Feature("Test Resources")]
    public class AdbProxyTests(AdbProxyTestFramework Fixture)
    {
        /// <summary>
        /// Verifies that a device can be added to adbd and removed
        /// This tests the "adb connect" and "adb disconnect" commands
        /// </summary>
        [Fact]
        [FunctionalTest]
        public async Task CanConnectToProxy_FromAdbProcess()
        {
            var devices = Fixture.GetDevices();
            var device = devices[0];
            Assert.NotNull(device.Url);

            await Cli.Wrap("adb")
                .WithArguments(["connect", device.Url], true)
                .ExecuteAsync(TestContext.Current.CancellationToken);

            // Try a command to verify that it worked
            var adb = new Adb(device.Url);
            var result = await adb.GetPropAsync("ro.build.version.sdk");

            await Cli.Wrap("adb")
                .WithArguments(["disconnect", device.Url], true)
                .ExecuteAsync(TestContext.Current.CancellationToken);

            Assert.NotNull(result);
            Assert.NotEmpty(result);
        }

        /// <summary>
        /// Verifies that multiple commands can be executed one after each other and that the result is the expected value
        /// 
        /// This test uses the adb process on the host and is starting real processes
        /// </summary>
        [Fact]
        [ReliabilityTest]
        public async Task ExecuteMultipleCommands_InSequence()
        {
            var devices = Fixture.GetDevices();
            var device = devices[0];
            Assert.NotNull(device.Url);

            await Cli.Wrap("adb")
                .WithArguments(["connect", device.Url], true)
                .ExecuteAsync(TestContext.Current.CancellationToken);
            var adb = new Adb(device.Url);

            // Get reference result
            var result = await adb.GetPropAsync("ro.build.version.sdk");
            Assert.NotNull(result);
            Assert.NotEmpty(result);

            for (int i = 0; i < 100; i++)
            {
                var result1 = await adb.GetPropAsync("ro.build.version.sdk");
                
                Assert.NotNull(result1);
                Assert.NotEmpty(result1);
                Assert.Equal(result, result1);
            }

            await Cli.Wrap("adb")
                .WithArguments(["disconnect", device.Url], true)
                .ExecuteAsync(TestContext.Current.CancellationToken);
        }
        /// <summary>
        /// Verifies that commands can be executed in parallel, one long running task is started in the background (logcat)
        /// and then a shorter command is executed 100 times while the long running command is running.
        /// 
        /// After the long running command is stopped the short command is verified again 100 times
        /// </summary>
        [Fact]
        [ReliabilityTest]
        public async Task ExecuteMultipleCommands_InParallel()
        {
            var devices = Fixture.GetDevices();
            var device = devices[0];
            Assert.NotNull(device.Url);

            await Cli.Wrap("adb")
                .WithArguments(["connect", device.Url], true)
                .ExecuteAsync(TestContext.Current.CancellationToken);
            var adb = new Adb(device.Url);

            // Get reference result
            var result = await adb.GetPropAsync("ro.build.version.sdk");
            Assert.NotNull(result);
            Assert.NotEmpty(result);

            // Try a command to verify that it worked
            var cts = CancellationTokenSource.CreateLinkedTokenSource(TestContext.Current.CancellationToken);
            var logcat = adb.StartLogcat(cts.Token);

            // While logcat is running
            for (int i = 0; i < 100; i++)
            {
                var result1 = await adb.GetPropAsync("ro.build.version.sdk");

                Assert.NotNull(result1);
                Assert.NotEmpty(result1);
                Assert.Equal(result, result1);
            }

            // Stop logcat
            cts.Cancel();

            // After logcat is stopped
            for (int i = 0; i < 100; i++)
            {
                var result1 = await adb.GetPropAsync("ro.build.version.sdk");

                Assert.NotNull(result1);
                Assert.NotEmpty(result1);
                Assert.Equal(result, result1);
            }

            await Cli.Wrap("adb")
                .WithArguments(["disconnect", device.Url], true)
                .ExecuteAsync(TestContext.Current.CancellationToken);
        }
    }
}
