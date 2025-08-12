using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TestBucket.AdbProxy.DeviceHandling;
using TestBucket.ResourceServer.Contracts;

namespace TestBucket.AdbProxy.Appium;
public class AppiumConnectionPool
{
    private readonly IAdbDeviceRepository _deviceRepository;
    private readonly IResourceRegistry _resourceRegistry;

    public AppiumConnectionPool(IAdbDeviceRepository deviceRepository, IResourceRegistry resourceRegistry)
    {
        _deviceRepository = deviceRepository;
        _resourceRegistry = resourceRegistry;
    }

    private readonly ConcurrentDictionary<string, AppiumConnection> _connections = [];

    public AppiumConnection GetOrCreate(string deviceId)
    {
        return _connections.GetOrAdd(deviceId, id =>
        {
            var device = _deviceRepository.Devices.FirstOrDefault(x => x.DeviceId == id);
            if (device == null)
            {
                throw new InvalidOperationException($"No device found with ID {id}");
            }
            return new AppiumConnection(device, _deviceRepository, _resourceRegistry);
        });
    }
    public void Destroy(string deviceId)
    {
        if(_connections.TryRemove(deviceId, out var connection))
        {
            connection.Dispose();
        }
    }
}
