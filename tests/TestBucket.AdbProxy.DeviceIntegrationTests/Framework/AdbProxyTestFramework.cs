using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.Extensions.DependencyInjection;
using TestBucket.AdbProxy.DeviceHandling;
using TestBucket.AdbProxy.DeviceIntegrationTests.Framework;
using TestBucket.AdbProxy.Models;
using TestBucket.AdbProxy.Proxy;

[assembly: AssemblyFixture(typeof(AdbProxyTestFramework))]

namespace TestBucket.AdbProxy.DeviceIntegrationTests.Framework
{
    public class AdbProxyTestFramework : IAsyncLifetime
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IAdbDeviceRepository _adbDeviceRepository;
        private readonly CancellationTokenSource _cts = new();

        public AdbProxyTestFramework()
        {
            Environment.SetEnvironmentVariable("TB_PUBLIC_IP", "127.0.0.1");

            IServiceCollection services = new ServiceCollection();
            services.AddAdbProxyServices();
            services.AddLogging();
            services.AddHttpClient();
            services.AddSingleton<IServer, FakeServer>();
            services.AddTestBucketResourceServer();
            _serviceProvider = services.BuildServiceProvider();
            _adbDeviceRepository = _serviceProvider.GetRequiredService<IAdbDeviceRepository>();
        }

        public ValueTask DisposeAsync()
        {
            _cts.Cancel();
            return ValueTask.CompletedTask;
        }

        public string[] GetSerials()
        {
            return _adbDeviceRepository.Devices.Select(d => d.DeviceId).ToArray();
        }

        public AdbDevice[] GetDevices()
        {
            return _adbDeviceRepository.Devices.ToArray();
        }

        public async ValueTask InitializeAsync()
        {
            await Adb.DisconnectAllAsync();

            // Perform one update so we add the currently attached device
            await _adbDeviceRepository.UpdateAsync(true, _cts.Token);
        }
    }
}
