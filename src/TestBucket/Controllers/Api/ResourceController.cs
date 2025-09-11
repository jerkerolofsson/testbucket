using System.Security.Principal;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using TestBucket.Domain.Files;
using TestBucket.Domain.Shared;

namespace TestBucket.Controllers.Api;

/// <summary>
/// File resources
/// </summary>
[ApiController]
public class ResourceController : ControllerBase
{
    private readonly IFileRepository _repo;

    public ResourceController(IFileRepository repo)
    {
        _repo = repo;
    }

    [Authorize("ApiKeyOrBearer")]
    [HttpGet("/api/resources/_health")]
    public IActionResult GetHealth()
    {
        return Ok();
    }

    [Authorize("ApiKeyOrBearer")]
    [HttpGet("/api/resources/{resourceId:long}")]
    public async Task<IActionResult> GetApiResourceAsync([FromRoute] long resourceId)
    {
        var tenantId = this.User.GetTenantIdOrThrow();
        var file = await _repo.GetResourceByIdAsync(tenantId, resourceId);
        if(file is null)
        {
            return NotFound();
        }

        try
        {
            this.User.ThrowIfNoPermission(file);
        }
        catch(UnauthorizedAccessException)
        {
            return Unauthorized();
        }

        return File(file.Data, file.ContentType);
    }
}
