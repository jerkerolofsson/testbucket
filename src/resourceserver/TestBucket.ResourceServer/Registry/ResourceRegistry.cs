

using System.Threading;

using TestBucket.Contracts.TestResources;

namespace TestBucket.ResosurceServer.Registry;
internal class ResourceRegistry : IResourceRegistry
{
    private readonly ConcurrentDictionary<string, IResource> _resources = [];

    public void AddResource(IResource resource)
    {
        _resources[resource.ResourceId] = resource;
    }

    public void RemoveResource(IResource resource)
    {
        _resources.TryRemove(resource.ResourceId, out _);
    }
    public async Task<ScreenshotDto?> CaptureScreenshotAsync(string resourceId, CancellationToken cancellationToken)
    {
        if(_resources.TryGetValue(resourceId, out var resource))
        {
            return await resource.CaptureScreenshot(cancellationToken);
        }
        return null;
    }

}
