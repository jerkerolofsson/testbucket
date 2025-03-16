using System.Collections.Concurrent;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using TestBucket.AdbProxy.Host;
using TestBucket.AdbProxy.Models;
using TestBucket.AdbProxy.Proxy;

namespace TestBucket.AdbProxy.DeviceHandling
{
    public class AdbDeviceIndexingService : BackgroundService
    {
        private readonly ILogger<AdbDeviceIndexingService> _logger;
        private readonly IAdbDeviceRepository _repo;

        public AdbDeviceIndexingService(
            ILogger<AdbDeviceIndexingService> logger,
            IAdbDeviceRepository repo)
        {
            _logger = logger;
            _repo = repo;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogDebug("Periodic scan of connected devices");
                await _repo.UpdateAsync(stoppingToken);

                await Task.Delay(10_000, stoppingToken);
            }
        }
    }
}
