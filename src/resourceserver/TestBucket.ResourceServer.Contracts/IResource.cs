using TestBucket.Contracts.TestResources;

namespace TestBucket.ResourceServer.Contracts;
public interface IResource
{
    string ResourceId { get; }
    Task<ScreenshotDto?> CaptureScreenshot(CancellationToken cancellationToken);
}
