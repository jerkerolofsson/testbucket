using TestBucket.AdbProxy.Models;
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
}
