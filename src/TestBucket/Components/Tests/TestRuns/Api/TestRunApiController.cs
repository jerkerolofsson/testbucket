using Mediator;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using TestBucket.Contracts.Projects;
using TestBucket.Controllers.Api;
using TestBucket.Domain.Projects;
using TestBucket.Domain.Teams;
using TestBucket.Domain.Testing.Duplication;
using TestBucket.Domain.Testing.Services.Export;
using TestBucket.Domain.Testing.Services.Import;
using TestBucket.Domain.Testing.TestRuns;
using TestBucket.Domain.Testing.TestRuns.Search;
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
    [ProducesDefaultResponseType(typeof(TestCaseRunDto))]
    public async Task<IActionResult> AddTestCaseRunToTestRunAsync([FromRoute] string slug, [FromBody] TestCaseRunDto testCaseRun)
    {
        if (!User.HasPermission(PermissionEntityType.TestRun, PermissionLevel.Write))
        {
            return Unauthorized();
        }
        if(string.IsNullOrEmpty(testCaseRun.TestCaseSlug))
        {
            return BadRequest("Cannot add a test case run without a test case slug!");
        }

        TestRun? run = await _runManager.GetTestRunBySlugAsync(User, slug);
        if(run is null)
        {
            return NotFound("Test run not found");
        }

        var createdTestCaseRun = await _mediator.Send(new ImportTestCaseRunRequest(User, run, testCaseRun, new ImportHandlingOptions()));
        var exportedTestCaseRun = await _mediator.Send(new ExportTestCaseRunRequest(User, createdTestCaseRun.Id));

        return Ok(exportedTestCaseRun);
    }


    [Authorize("ApiKeyOrBearer")]
    [HttpGet("/api/runs/{slug}/tests")]
    [ProducesDefaultResponseType(typeof(TestCaseRunDto[]))]
    public async Task<IActionResult> GetTestRunCasesAsync([FromRoute] string slug)
    {
        if (!User.HasPermission(PermissionEntityType.TestRun, PermissionLevel.Read) ||
            !User.HasPermission(PermissionEntityType.TestCaseRun, PermissionLevel.Read))
        {
            return Unauthorized();
        }

        TestRun? run = await _runManager.GetTestRunBySlugAsync(User, slug);
        if (run is null)
        {
            return NotFound("Test run not found");
        }

        List<TestCaseRunDto> tests = [];

        int offset = 0;
        int count = 100;
        while(true)
        {
            var result = await _runManager.SearchTestCaseRunsAsync(User, new SearchTestCaseRunQuery { TestRunId = run.Id, Offset = offset, Count = count });

            foreach(var testCaseRun in result.Items)
            {
                var exportedTestCaseRun = await _mediator.Send(new ExportTestCaseRunRequest(User, testCaseRun.Id));
                tests.Add(exportedTestCaseRun);
            }
            if(result.Items.Length != count)
            {
                break;
            }

        }

        return Ok(tests);
    }

    [Authorize("ApiKeyOrBearer")]
    [EndpointDescription("Creates a copy of the test run")]
    [HttpPost("/api/runs/{slug}/duplicate")]
    [ProducesDefaultResponseType(typeof(TestRunDto))]
    public async Task<IActionResult> DuplicateAsync(string slug)
    {
        if (!User.HasPermission(PermissionEntityType.TestRun, PermissionLevel.Write) ||
            !User.HasPermission(PermissionEntityType.TestCaseRun, PermissionLevel.Write))
        {
            return Unauthorized();
        }
        TestRun? run = await _runManager.GetTestRunBySlugAsync(User, slug);
        if (run is null)
        {
            return NotFound("Test run not found");
        }

        var createdTestRun = await _mediator.Send(new DuplicateTestRunRequest(User, run));
        var exportedRun = await _mediator.Send(new ExportRunRequest(User, createdTestRun.Id));
        return Ok(exportedRun);
    }

    [Authorize("ApiKeyOrBearer")]
    [HttpPut("/api/runs")]
    [HttpPost("/api/runs")]
    [ProducesDefaultResponseType(typeof(TestRunDto))]
    public async Task<IActionResult> AddAsync([FromBody] TestRunDto run)
    {
        if(!User.HasPermission(PermissionEntityType.TestRun, PermissionLevel.Write))
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
