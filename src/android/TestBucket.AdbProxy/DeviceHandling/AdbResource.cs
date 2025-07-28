using System.Text;

using TestBucket.AdbProxy.Host;
using TestBucket.AdbProxy.Models;
using TestBucket.AdbProxy.Proxy;
using TestBucket.Contracts.TestResources;
using TestBucket.ResourceServer.Contracts;

namespace TestBucket.AdbProxy.DeviceHandling;
internal class AdbResource(AdbDevice Device, Proxy.AdbProxyOptions AdbProxyOptions) : IResource
{
    public string ResourceId => Device.DeviceId;

    public async Task<ScreenshotDto?> CaptureScreenshot(CancellationToken cancellationToken)
    {
        var screenshot = new CaptureScreenshot(AdbProxyOptions);
        var bytes = await screenshot.RunAsync(ResourceId, cancellationToken);
        return new ScreenshotDto { Bytes = bytes, ContentType = "image/png" };
    }

    internal async Task<string> ExecShellGetStringAsync(string deviceId, string command, CancellationToken cancellationToken)
    {
        var adb = new AdbHostClient(AdbProxyOptions);

        uint localId = 1;
        uint remoteId = 1;
        using var adbStream = await adb.CreateStreamAsync(localId, remoteId, [$"host:transport:{deviceId}", command], cancellationToken);
        var bytes = await adbStream.ReadByteArrayAsync(cancellationToken);
        return Encoding.UTF8.GetString(bytes);
    }
}
