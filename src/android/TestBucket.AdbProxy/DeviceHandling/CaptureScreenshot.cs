using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using TestBucket.AdbProxy.Host;
using TestBucket.AdbProxy.Proxy;

namespace TestBucket.AdbProxy.DeviceHandling;
internal class CaptureScreenshot
{
    private readonly AdbProxyOptions _adbProxyOptions;

    public CaptureScreenshot(AdbProxyOptions adbProxyOptions)
    {
        _adbProxyOptions = adbProxyOptions;
    }

    /// <summary>
    /// Returns the result from getprop as a string
    /// </summary>
    /// <param name="deviceId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    internal async Task<byte[]> RunAsync(string deviceId, CancellationToken cancellationToken)
    {
        var adb = new AdbHostClient(_adbProxyOptions);

        uint localId = 1;
        uint remoteId = 1;
        using var adbStream = await adb.CreateStreamAsync(localId, remoteId, [$"host:transport:{deviceId}", "shell:screencap -p"], cancellationToken);
        //using var adbStream = await adb.CreateStreamAsync(localId, remoteId, [$"host:transport:{deviceId}", "shell:getprop"], cancellationToken);

        var bytes = await adbStream.ReadByteArrayAsync(cancellationToken);
        return bytes;
    }
}
