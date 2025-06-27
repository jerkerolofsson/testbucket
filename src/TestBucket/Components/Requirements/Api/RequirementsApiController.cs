using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using TestBucket.Components.Projects.Api;
using TestBucket.Contracts.Projects;
using TestBucket.Controllers.Api;
using TestBucket.Domain.Export;
using TestBucket.Domain.Export.Handlers.Requirements;
using TestBucket.Domain.Projects;
using TestBucket.Domain.Requirements;
using TestBucket.Domain.Teams;

namespace TestBucket.Components.Requirements.Api;

[ApiController]
public class RequirementsApiController : ProjectApiControllerBase
{
    private readonly IRequirementManager _manager;
    private readonly IBackupManager _backupManager;
    private readonly ILogger _logger;

    public RequirementsApiController(IRequirementManager manager, ILogger<RequirementsApiController> logger, IBackupManager backupManager)
    {
        _manager = manager;
        _logger = logger;
        _backupManager = backupManager;
    }

    [Authorize("ApiKeyOrBearer")]
    [HttpGet("/api/requirements/collections/{collectionId:long}/export")]
    public async Task<IActionResult> ExportAsync([FromRoute] long collectionId)
    {
        if (!User.HasPermission(PermissionEntityType.RequirementSpecification, PermissionLevel.Read))
        {
            return Unauthorized();
        }

        var specification = await _manager.GetRequirementSpecificationByIdAsync(User, collectionId);
        if(specification is null)
        {
            return NotFound();
        }


        var stream = await _backupManager.CreateBackupAsync(User, specification);
        return File(stream, "application/zip");
    }
}
