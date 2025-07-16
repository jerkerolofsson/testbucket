using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace TestBucket.AdbProxy.DeviceHandling
{
    public class AdbDeviceIndexingService : BackgroundService
    {
        private readonly ILogger<AdbDeviceIndexingService> _logger;
        private readonly IAdbDeviceRepository _repo;
        private bool _isFirst = true;

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
                try
                {
                    await _repo.UpdateAsync(_isFirst, stoppingToken);
                    _isFirst = false;
                }
                catch(Exception ex)
                {
                    _logger.LogError(ex, "Error updating");
                }

                await Task.Delay(10_000, stoppingToken);
            }
        }
    }
}
