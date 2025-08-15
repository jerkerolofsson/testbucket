using System.Net;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using TestBucket.Contracts.Projects;
using TestBucket.Controllers.Api;
using TestBucket.Domain.Export;
using TestBucket.Domain.Export.Handlers.Project;
using TestBucket.Domain.Projects;
using TestBucket.Domain.Projects.Mapping;
using TestBucket.Domain.Teams;

namespace TestBucket.Components.Projects.Api;

[ApiController]
public class ProjectApiController : ProjectApiControllerBase
{
    private readonly IProjectManager _manager;
    private readonly ITeamManager _teamManager;
    private readonly ILogger<ProjectApiController> _logger;
    private readonly IBackupManager _backupManager;

    public ProjectApiController(IProjectManager manager, ITeamManager teamManager, ILogger<ProjectApiController> logger, IBackupManager backupManager)
    {
        _manager = manager;
        _teamManager = teamManager;
        _logger = logger;
        _backupManager = backupManager;
    }

    [Authorize("ApiKeyOrBearer")]
    [HttpPut("/api/projects")]
    [HttpPost("/api/projects")]
    [ProducesDefaultResponseType(typeof(ProjectDto))]
    public async Task<IActionResult> AddAsync([FromBody] ProjectDto project)
    {
        if(!User.HasPermission(PermissionEntityType.Project, PermissionLevel.Write))
        {
            return Unauthorized();
        }
        if (string.IsNullOrEmpty(project.Name))
        {
            return BadRequest("Project name cannot be empty");
        }
        if (string.IsNullOrEmpty(project.Team))
        {
            return BadRequest("Project team cannot be empty");
        }
        var team = await _teamManager.GetTeamBySlugAsync(User, project.Team);
        if(team is null)
        {
            return NotFound("Team not found");
        }

        var dbo = project.ToDbo();
        dbo.TeamId = team.Id;

        _logger.LogInformation("Creating project: {ProjectName}", dbo.Name);

        var creationResult = await _manager.AddAsync(User, dbo);
        return creationResult.Match<IActionResult>(
            success => Ok(success.ToDto(false)),
            alreadyExists => BadRequest("Project already exists!")
            );
    }

    [Authorize("ApiKeyOrBearer")]
    [HttpDelete("/api/projects/{slug}")]
    public async Task<IActionResult> DeleteAsync([FromRoute] string slug)
    {
        if (!User.HasPermission(PermissionEntityType.Project, PermissionLevel.Delete))
        {
            return Unauthorized();
        }
        if (string.IsNullOrEmpty(slug))
        {
            return BadRequest("Project slug cannot be empty");
        }

        var project = await _manager.GetTestProjectBySlugAsync(User, slug);
        if(project is null)
        {
            return NotFound();
        }

        _logger.LogInformation("Deleting project: {ProjectName}", project.Name);
        await _manager.DeleteAsync(User, project);

        return Ok();
    }


    [Authorize("ApiKeyOrBearer")]
    [HttpGet("/api/projects/{slug}")]
    [ProducesDefaultResponseType(typeof(ProjectDto))]
    public async Task<IActionResult> GetAsync([FromRoute] string slug)
    {
        if (!User.HasPermission(PermissionEntityType.Project, PermissionLevel.Read))
        {
            return Unauthorized();
        }
        if (string.IsNullOrEmpty(slug))
        {
            return BadRequest("Project slug cannot be empty");
        }

        var project = await _manager.GetTestProjectBySlugAsync(User, slug);
        if (project is null)
        {
            return NotFound();
        }
        if (User.HasPermission(PermissionEntityType.Project, PermissionLevel.Delete))
        {
            return Ok(project.ToDto(true));
        }
        else
        {
            return Ok(project.ToDto(false));
        }
    }


    [Authorize("ApiKeyOrBearer")]
    [HttpGet("/api/projects/{slug}/export")]
    [ProducesDefaultResponseType(typeof(ProjectDto))]
    public async Task<IActionResult> ExportProjectAsync([FromRoute] string slug)
    {
        if (!User.HasPermission(PermissionEntityType.Project, PermissionLevel.Read))
        {
            return Unauthorized();
        }
       
        if (string.IsNullOrEmpty(slug))
        {
            return BadRequest("Project slug cannot be empty");
        }

        var project = await _manager.GetTestProjectBySlugAsync(User, slug);
        if (project is null)
        {
            return NotFound();
        }
        var stream = await _backupManager.CreateBackupAsync(User, project);
        return File(stream, "application/zip", $"{project.Name.ToLower().Replace(' ', '_')}.tbz");

    }
}
