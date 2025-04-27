using System.Net;

using Mediator;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using TestBucket.Contracts.Projects;
using TestBucket.Controllers.Api;
using TestBucket.Domain.Projects;
using TestBucket.Domain.Projects.Mapping;
using TestBucket.Domain.Teams;
using TestBucket.Domain.Testing.Services.Export;
using TestBucket.Domain.Testing.Services.Import;
using TestBucket.Domain.Testing.TestRuns;
using TestBucket.Formats;
using TestBucket.Formats.Dtos;

namespace TestBucket.Components.Projects.Api;

[ApiController]
public class TestRunApiController : ProjectApiControllerBase
{
    private readonly ITeamManager _teamManager;
    private readonly IProjectManager _projectManager;
    private readonly ITestRunManager _runManager;
    private readonly IMediator _mediator;

    public TestRunApiController(ITeamManager teamManager, IProjectManager projectManager, ITestRunManager runManager, IMediator mediator)
    {
        _teamManager = teamManager;
        _projectManager = projectManager;
        _runManager = runManager;
        _mediator = mediator;
    }


    [Authorize("ApiKeyOrBearer")]
    [HttpPut("/api/runs/{slug}/tests")]
    [HttpPost("/api/runs/{slug}/tests")]
    [ProducesDefaultResponseType(typeof(ProjectDto))]
    public async Task<IActionResult> AddTestCaseRunToTestRunAsync([FromRoute] string slug, [FromBody] TestCaseRunDto testCaseRun)
    {
        if (!User.HasPermission(PermissionEntityType.Project, PermissionLevel.Write))
        {
            return Unauthorized();
        }

        TestRun? run = await _runManager.GetTestRunBySlugAsync(User, slug);
        if(run is null)
        {
            return NotFound("Test run not found");
        }

        var createdTestCaseRun = await _mediator.Send(new ImportTestCaseRunRequest(User, run, testCaseRun, new ImportHandlingOptions()));
        var exportedRun = await _mediator.Send(new ExportTestCaseRunRequest(User, createdTestCaseRun.Id));

        return Ok(exportedRun);
    }

    [Authorize("ApiKeyOrBearer")]
    [HttpPut("/api/runs")]
    [HttpPost("/api/runs")]
    [ProducesDefaultResponseType(typeof(ProjectDto))]
    public async Task<IActionResult> AddAsync([FromBody] TestRunDto run)
    {
        if(!User.HasPermission(PermissionEntityType.Project, PermissionLevel.Write))
        {
            return Unauthorized();
        }
        if (string.IsNullOrEmpty(run.Project))
        {
            return BadRequest("Project name cannot be empty");
        }
        if (string.IsNullOrEmpty(run.Team))
        {
            return BadRequest("Project team cannot be empty");
        }
        var team = await _teamManager.GetTeamBySlugAsync(User, run.Team);
        if (team is null)
        {
            return NotFound("Team not found");
        }
        var project = await _projectManager.GetTestProjectBySlugAsync(User, run.Project);
        if (project is null)
        {
            return NotFound("Project not found");
        }

        var testRun = await _mediator.Send(new ImportRunRequest(User, run, new ImportHandlingOptions()));

        var exportedRun = await _mediator.Send(new ExportRunRequest(User, testRun.Id));
        
        return Ok(exportedRun);
    }
}
