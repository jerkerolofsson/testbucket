using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TestBucket.AdbProxy.Host;
using TestBucket.AdbProxy.Models;
using TestBucket.AdbProxy.Proxy;

namespace TestBucket.AdbProxy.DeviceHandling;

/// <summary>
/// Runs "adb getprop" on a device, extracting model/manufacturer from a device
/// </summary>
internal class GetProp
{
    private readonly AdbProxyOptions _adbProxyOptions;

    public GetProp(AdbProxyOptions adbProxyOptions)
    {
        _adbProxyOptions = adbProxyOptions;
    }

    /// <summary>
    /// Returns the result from getprop as a string
    /// </summary>
    /// <param name="deviceId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    internal async Task<string> RunAsync(string deviceId, CancellationToken cancellationToken)
    {
        var adb = new AdbHostClient(_adbProxyOptions);

        uint localId = 1;
        uint remoteId = 1;
        using var adbStream = await adb.CreateStreamAsync(localId, remoteId, [$"host:transport:{deviceId}", "shell:getprop"], cancellationToken);

        var text = await adbStream.ReadToEndAsync(cancellationToken);
        return text;
    }
}
