using System.Text;
using System.Threading;

using TestBucket.AdbProxy.Host;
using TestBucket.AdbProxy.Models;
using TestBucket.AdbProxy.Proxy;
using TestBucket.Contracts.TestResources;
using TestBucket.ResourceServer.Contracts;

namespace TestBucket.AdbProxy.DeviceHandling;
internal class AdbResource(AdbDevice Device, Proxy.AdbProxyOptions AdbProxyOptions) : IResource
{
    public string ResourceId => Device.DeviceId;

    public async Task<ScreenshotDto?> CaptureScreenshot(CancellationToken cancellationToken = default)
    {
        var screenshot = new CaptureScreenshot(AdbProxyOptions);
        var bytes = await screenshot.RunAsync(ResourceId, cancellationToken);
        return new ScreenshotDto { Bytes = bytes, ContentType = "image/png" };
    }

    public async Task<string[]> ListPackagesAsync(CancellationToken cancellationToken = default)
    {
        var text = await ExecShellGetStringAsync("shell:cmd package list packages", cancellationToken);

        return text.Split('\n')
            .Select(line => line.Replace("package:", "").Trim())
            .ToArray();
    }

    internal async Task<string> ExecShellGetStringAsync(string command, CancellationToken cancellationToken = default)
    {
        var adb = new AdbHostClient(AdbProxyOptions);
        string deviceId = ResourceId;

        uint localId = 1;
        uint remoteId = 1;
        using var adbStream = await adb.CreateStreamAsync(localId, remoteId, [$"host:transport:{deviceId}", command], cancellationToken);
        var bytes = await adbStream.ReadByteArrayAsync(cancellationToken);
        return Encoding.UTF8.GetString(bytes);
    }
}
