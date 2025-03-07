using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using TestBucket.Domain.Files;

namespace TestBucket.Controllers.Api;

[ApiController]
public class ResourceController : ControllerBase
{
    private readonly IFileRepository _repo;

    public ResourceController(IFileRepository repo)
    {
        _repo = repo;
    }

    [Authorize("ApiKeyOrBearer")]
    [HttpGet("/api/resources/{resourceId:long}")]
    public async Task<IActionResult> GetApiResourceAsync([FromRoute] long resourceId)
    {
        var tenantId = User.Claims.Where(x => x.Type == "tenant").Select(x=>x.Value).FirstOrDefault();
        if(tenantId is null)
        {
            return Unauthorized();
        }

        var file = await _repo.GetResourceByIdAsync(tenantId, resourceId);
        if(file is null)
        {
            return NotFound();
        }
        return File(file.Data, file.ContentType);
    }
}
