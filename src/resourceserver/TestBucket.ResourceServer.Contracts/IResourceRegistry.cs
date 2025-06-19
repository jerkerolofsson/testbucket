using System.Threading;

using TestBucket.Contracts.TestResources;

namespace TestBucket.ResourceServer.Contracts;
public interface IResourceRegistry
{
    void AddResource(IResource resource);
    void RemoveResource(IResource resource);
    Task<ScreenshotDto?> CaptureScreenshotAsync(string resourceId, CancellationToken cancellationToken);
}
