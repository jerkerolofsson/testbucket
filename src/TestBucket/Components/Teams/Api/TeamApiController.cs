using System.Net;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using TestBucket.Contracts.Teams;
using TestBucket.Controllers.Api;
using TestBucket.Domain.Projects;
using TestBucket.Domain.Teams;
using TestBucket.Domain.Teams.Mapping;

namespace TestBucket.Components.Tests.TestCases.Api;

[ApiController]
public class TeamApiController : ProjectApiControllerBase
{
    private readonly ITeamManager _teamManager;

    public TeamApiController(ITeamManager teamManager)
    {
        _teamManager = teamManager;
    }

    [Authorize("ApiKeyOrBearer")]
    [HttpPut("/api/teams")]
    [HttpPost("/api/teams")]
    [ProducesDefaultResponseType(typeof(TestCaseDto))]
    public async Task<IActionResult> AddAsync([FromBody] TeamDto team)
    {
        if(!User.HasPermission(PermissionEntityType.Team, PermissionLevel.Write))
        {
            return Unauthorized();
        }
        if(string.IsNullOrEmpty(team.Name))
        {
            return BadRequest("Team name cannot be empty");
        }

        var dbo = team.ToDbo();

        var creationResult = await _teamManager.AddAsync(User, dbo);
        return creationResult.Match<IActionResult>(
            success => Ok(success.ToDto()),
            alreadyExists => BadRequest()
            );
    }

    [Authorize("ApiKeyOrBearer")]
    [HttpDelete("/api/teams/{slug}")]
    public async Task<IActionResult> DeleteAsync([FromRoute] string slug)
    {
        if (!User.HasPermission(PermissionEntityType.Team, PermissionLevel.Delete))
        {
            return Unauthorized();
        }
        if (string.IsNullOrEmpty(slug))
        {
            return BadRequest("Team slug cannot be empty");
        }

        var team = await _teamManager.GetTeamBySlugAsync(User, slug);
        if(team is null)
        {
            return NotFound();
        }

        await _teamManager.DeleteAsync(User, team);

        return Ok();
    }


    [Authorize("ApiKeyOrBearer")]
    [HttpGet("/api/teams/{slug}")]
    [ProducesDefaultResponseType(typeof(TestCaseDto))]
    public async Task<IActionResult> GetAsync([FromRoute] string slug)
    {
        if (!User.HasPermission(PermissionEntityType.Team, PermissionLevel.Delete))
        {
            return Unauthorized();
        }
        if (string.IsNullOrEmpty(slug))
        {
            return BadRequest("Team slug cannot be empty");
        }

        var team = await _teamManager.GetTeamBySlugAsync(User, slug);
        if (team is null)
        {
            return NotFound();
        }
        return Ok(team.ToDto());
    }
}
