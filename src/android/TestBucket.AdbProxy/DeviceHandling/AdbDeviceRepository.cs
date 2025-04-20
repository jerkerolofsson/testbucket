using System.Collections.Concurrent;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using TestBucket.AdbProxy.Android;
using TestBucket.AdbProxy.Host;
using TestBucket.AdbProxy.Inform;
using TestBucket.AdbProxy.Models;
using TestBucket.AdbProxy.Proxy;

namespace TestBucket.AdbProxy.DeviceHandling;
public class AdbDeviceRepository : IAdbDeviceRepository
{
    private readonly ILogger<AdbDeviceRepository> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly ConcurrentDictionary<string, AdbProxyServer> _servers = new();
    private readonly AdbProxyOptions _adbProxyOptions;
    private readonly IDeviceInformer _deviceInformer;

    /// <summary>
    /// Returns a list of all devices
    /// </summary>
    public IReadOnlyList<AdbDevice> Devices => _servers.Select(x => x.Value.Device).ToList();

    public AdbDeviceRepository(
        ILogger<AdbDeviceRepository> logger,
        IServiceProvider serviceProvider, IOptions<AdbProxyOptions> options, IDeviceInformer deviceInformer)
    {
        _adbProxyOptions = options.Value;
        _logger = logger;
        _serviceProvider = serviceProvider;
        _deviceInformer = deviceInformer;
    }

    /// <summary>
    /// Scans for device changes and starts/stops proxies for connected devices
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task UpdateAsync(bool isFirstUpdate, CancellationToken cancellationToken)
    {
        var adb = new AdbHostClient(_adbProxyOptions);
        var devices = await adb.ListDevicesAsync(cancellationToken);
        var deviceDict = new Dictionary<string, AdbDevice>();
        foreach (var device in devices)
        {
            deviceDict[device.DeviceId] = device;

            if(_servers.TryGetValue(device.DeviceId, out var proxyServer))
            {
                device.Url = proxyServer.DeviceUrl;
                device.Port = proxyServer.Port;
            }
        }

        // Stop servers where the device has been removed
        bool changed = RemoveServersForRemovedDevices(deviceDict);

        // Start new servers if a device has been added
        if(await StartServersForAddedDevicesAsync(deviceDict, cancellationToken))
        {
            changed = true;
        }

        // Updates the device status like unauthorized, offline for a device
        if(UpdateDeviceStatus(deviceDict))
        {
            changed = true;
        }

        if(changed || isFirstUpdate)
        {
            try
            {
                await _deviceInformer.InformAsync(devices, cancellationToken);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Failed to inform");
            }
        }
    }

    private bool UpdateDeviceStatus(Dictionary<string, AdbDevice> deviceDict)
    {
        var changed = false;
        foreach (var deviceId in _servers.Keys.ToArray())
        {
            if (deviceDict.TryGetValue(deviceId, out var device))
            {
                if (_servers.TryGetValue(deviceId, out var server))
                {
                    if (server.Device.Status != device.Status)
                    {
                        changed = true;
                        server.Device.Status = device.Status;
                    }
                }
            }
        }
        return changed;
    }

    private async Task<bool> StartServersForAddedDevicesAsync(Dictionary<string, AdbDevice> deviceDict, CancellationToken cancellationToken)
    {
        bool changed = false;
        foreach (var device in deviceDict.Values)
        {
            // Read information from the device
            if(await ExtractGetpropAsync(device, cancellationToken))
            {
                changed = true;
            }

            if (!_servers.ContainsKey(device.DeviceId))
            {
                // Start server
                var server = ActivatorUtilities.CreateInstance<AdbProxyServer>(_serviceProvider, device);
                server.Start(cancellationToken);

                _logger.LogInformation("Server started for {deviceId} on {port}", device.DeviceId, server.Port);
                _servers[device.DeviceId] = server;
                changed = true;
            }
        }
        return changed;
    }

    private async Task<bool> ExtractGetpropAsync(AdbDevice device, CancellationToken cancellationToken)
    {
        bool changed = false;
        var getpropResponse = await new GetProp(_adbProxyOptions).RunAsync(device.DeviceId, cancellationToken);
        var properties = GetPropParser.Parse(getpropResponse);
        if (properties.TryGetValue("ro.product.model", out var model))
        {
            if (device.ModelInfo.Name != model)
            {
                changed = true;
                device.ModelInfo.Name = model;
            }
        }
        if (properties.TryGetValue("ro.product.manufacturer", out var manufacturer))
        {
            if(device.ModelInfo.Manufacturer != manufacturer)
            {
                changed = true;
                device.ModelInfo.Manufacturer = manufacturer;
            }
        }
        if (properties.TryGetValue("ro.product.build.version.sdk", out var sdkString) && int.TryParse(sdkString, out int sdk))
        {
            if(device.ApiLevel != sdk)
            {
                changed = true;
                device.ApiLevel = sdk;
                device.Version = AndroidVersionProvider.FromApiLevel(sdk);
            }
        }
        return changed;
    }

    private bool RemoveServersForRemovedDevices(Dictionary<string, AdbDevice> deviceDict)
    {
        bool changed = false;
        foreach (var deviceId in _servers.Keys.ToArray())
        {
            if (!deviceDict.ContainsKey(deviceId))
            {
                if (_servers.TryRemove(deviceId, out var server))
                {
                    _logger.LogInformation("Removing server for device {deviceId}", deviceId);
                    server.Dispose();
                    changed = true;
                }
            }
        }
        return changed;
    }
}
