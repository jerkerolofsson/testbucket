using System.Net;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using TestBucket.Contracts.Projects;
using TestBucket.Controllers.Api;
using TestBucket.Domain.Projects;
using TestBucket.Domain.Projects.Mapping;
using TestBucket.Domain.Teams;

namespace TestBucket.Components.Projects.Api;

[ApiController]
public class ProjectApiController : ProjectApiControllerBase
{
    private readonly IProjectManager _manager;
    private readonly ITeamManager _teamManager;

    public ProjectApiController(IProjectManager manager, ITeamManager teamManager)
    {
        _manager = manager;
        _teamManager = teamManager;
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

        var creationResult = await _manager.AddAsync(User, dbo);
        return creationResult.Match<IActionResult>(
            success => Ok(success.ToDto()),
            alreadyExists => BadRequest()
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

        await _manager.DeleteAsync(User, project);

        return Ok();
    }


    [Authorize("ApiKeyOrBearer")]
    [HttpGet("/api/projects/{slug}")]
    [ProducesDefaultResponseType(typeof(TestCaseDto))]
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
        return Ok(project.ToDto());
    }
}
