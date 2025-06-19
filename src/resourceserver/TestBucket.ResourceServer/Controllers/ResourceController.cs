using Microsoft.AspNetCore.Mvc;

namespace TestBucket.Servers.AdbProxy.Controllers;

[ApiController]
public class ResourceController : ControllerBase
{
    private readonly IResourceRegistry _resourceRegistry;

    public ResourceController(IResourceRegistry resourceRegistry)
    {
        _resourceRegistry = resourceRegistry;
    }

    [HttpGet("/resources/{resourceId}/screenshot")]
    public async Task<IActionResult> CaptureScreenshot([FromRoute] string resourceId, CancellationToken cancellationToken)
    {
        var screenshot = await _resourceRegistry.CaptureScreenshotAsync(resourceId, cancellationToken);
        if(screenshot is null)
        {
            return NotFound();
        }
        return File(screenshot.Bytes, screenshot.ContentType);
    }
}
