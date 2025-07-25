using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using RedMaple.Orchestrator.Contracts.Containers;

using TestBucket.AdbProxy.DeviceHandling;

namespace TestBucket.AdbProxy.Appium;
public class AppiumDockerCleaner : BackgroundService
{
    private readonly IAdbDeviceRepository _repository;
    private readonly ILocalContainersClient _docker;
    private readonly ILogger<AppiumDockerCleaner> _logger;
    private readonly Dictionary<string, DateTime> _offlineDates = [];
    private readonly TimeSpan _gracePeriod = TimeSpan.FromMinutes(10);

    public AppiumDockerCleaner(IAdbDeviceRepository repository, ILocalContainersClient docker, ILogger<AppiumDockerCleaner> logger)
    {
        _repository = repository;
        _docker = docker;
        _logger = logger;
    }

    public async Task DownOfflineContainersAsync(string[] onlineSerials, CancellationToken cancellationToken)
    {
        var containers = await _docker.GetContainersAsync(null, cancellationToken);
        foreach(var container in containers)
        {
            if(container.Labels is not null &&
                container.Labels.TryGetValue("testbucket.adbproxy.deviceid", out var deviceId))
            {
                if(!onlineSerials.Contains(deviceId))
                {
                    _logger.LogInformation("Found container belonging to a device which is offline: ContainerId={ContainerId}, device={DeviceId}", container.Id, deviceId);

                    // If device restarts etc.. Don't tear down the container immediately, wait for a grace period
                    if (!_offlineDates.TryGetValue(deviceId, out var offlineDate))
                    {
                        offlineDate = DateTime.Now;
                        _offlineDates[deviceId] = offlineDate;
                    }
                    var elapsed = DateTime.Now - offlineDate;

                    if (elapsed >= _gracePeriod)
                    {
                        try
                        {
                            _logger.LogInformation("Destroying appium container device is not connected");
                            _offlineDates.Remove(deviceId);

                            await _docker.StopAsync(container.Id, cancellationToken);
                            await _docker.DeleteContainerAsync(container.Id, cancellationToken);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Failed to stop/remove container {ContainerId}", container.Id);
                        }
                    }
                }
            }
        }
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while(!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);

            string[] deviceIds = [.._repository.Devices.Select(x => x.DeviceId)];

            try
            {
                await DownOfflineContainersAsync(deviceIds, stoppingToken);
            }
            catch { }
        }
    }
}
