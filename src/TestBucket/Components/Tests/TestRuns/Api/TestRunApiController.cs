using Mediator;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using TestBucket.Controllers.Api;
using TestBucket.Domain.Projects;
using TestBucket.Domain.Shared;
using TestBucket.Domain.Teams;
using TestBucket.Domain.Testing.Duplication;
using TestBucket.Domain.Testing.Services.Export;
using TestBucket.Domain.Testing.Services.Import;
using TestBucket.Domain.Testing.TestRuns;
using TestBucket.Domain.Testing.TestRuns.Search;
using TestBucket.Formats;
using TestBucket.Formats.Dtos;

namespace TestBucket.Components.Projects.Api;

/// <summary>
/// API controller for managing test runs and their test cases.
/// </summary>
[ApiController]
public class TestRunApiController : ProjectApiControllerBase
{
    private readonly ITeamManager _teamManager;
    private readonly IProjectManager _projectManager;
    private readonly ITestRunManager _runManager;
    private readonly IMediator _mediator;

    /// <summary>
    /// Initializes a new instance of the <see cref="TestRunApiController"/> class.
    /// </summary>
    public TestRunApiController(ITeamManager teamManager, IProjectManager projectManager, ITestRunManager runManager, IMediator mediator)
    {
        _teamManager = teamManager;
        _projectManager = projectManager;
        _runManager = runManager;
        _mediator = mediator;
    }

    /// <summary>
    /// Adds a test case run to a test run.
    /// </summary>
    /// <param name="slug">The slug of the test run.</param>
    /// <param name="testCaseRun">The test case run to add.</param>
    /// <returns>The created test case run.</returns>
    [Authorize("ApiKeyOrBearer")]
    [HttpPut("/api/runs/{slug}/tests")]
    [HttpPost("/api/runs/{slug}/tests")]
    [ProducesDefaultResponseType(typeof(TestCaseRunDto))]
    [ProducesResponseType(typeof(TestCaseRunDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ApiExplorerSettings(GroupName = "TestRuns")]
    [EndpointDescription("Adds a test case run to the specified test run.")]
    public async Task<IActionResult> AddTestCaseRunToTestRunAsync([FromRoute] string slug, [FromBody] TestCaseRunDto testCaseRun)
    {
        if (!User.HasPermission(PermissionEntityType.TestRun, PermissionLevel.Write))
        {
            return Unauthorized();
        }
        if (string.IsNullOrEmpty(testCaseRun.TestCaseSlug))
        {
            return BadRequest("Cannot add a test case run without a test case slug!");
        }

        TestRun? run = await _runManager.GetTestRunBySlugAsync(User, User.GetProjectId(), slug);
        if (run is null)
        {
            return NotFound("Test run not found");
        }

        var createdTestCaseRun = await _mediator.Send(new ImportTestCaseRunRequest(User, run, testCaseRun, new ImportHandlingOptions()));
        var exportedTestCaseRun = await _mediator.Send(new ExportTestCaseRunRequest(User, createdTestCaseRun.Id));

        return Ok(exportedTestCaseRun);
    }

    /// <summary>
    /// Gets all test case runs for a test run.
    /// </summary>
    /// <param name="slug">The slug of the test run.</param>
    /// <returns>An array of test case runs.</returns>
    [Authorize("ApiKeyOrBearer")]
    [HttpGet("/api/runs/{slug}/tests")]
    [ProducesDefaultResponseType(typeof(TestCaseRunDto[]))]
    [ProducesResponseType(typeof(TestCaseRunDto[]), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ApiExplorerSettings(GroupName = "TestRuns")]
    [EndpointDescription("Gets all test case runs for the specified test run.")]
    public async Task<IActionResult> GetTestRunCasesAsync([FromRoute] string slug)
    {
        if (!User.HasPermission(PermissionEntityType.TestRun, PermissionLevel.Read) ||
            !User.HasPermission(PermissionEntityType.TestCaseRun, PermissionLevel.Read))
        {
            return Unauthorized();
        }

        TestRun? run = await _runManager.GetTestRunBySlugAsync(User, User.GetProjectId(), slug);
        if (run is null)
        {
            return NotFound("Test run not found");
        }

        List<TestCaseRunDto> tests = [];

        int offset = 0;
        int count = 100;
        while (true)
        {
            var result = await _runManager.SearchTestCaseRunsAsync(User, new SearchTestCaseRunQuery { TestRunId = run.Id, Offset = offset, Count = count });

            foreach (var testCaseRun in result.Items)
            {
                var exportedTestCaseRun = await _mediator.Send(new ExportTestCaseRunRequest(User, testCaseRun.Id));
                tests.Add(exportedTestCaseRun);
            }
            if (result.Items.Length != count)
            {
                break;
            }

        }

        return Ok(tests);
    }

    /// <summary>
    /// Creates a copy of the specified test run.
    /// </summary>
    /// <param name="slug">The slug of the test run to duplicate.</param>
    /// <returns>The duplicated test run.</returns>
    [Authorize("ApiKeyOrBearer")]
    [EndpointDescription("Creates a copy of the test run.")]
    [HttpPost("/api/runs/{slug}/duplicate")]
    [ProducesDefaultResponseType(typeof(TestRunDto))]
    [ProducesResponseType(typeof(TestRunDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ApiExplorerSettings(GroupName = "TestRuns")]
    public async Task<IActionResult> DuplicateAsync(string slug)
    {
        if (!User.HasPermission(PermissionEntityType.TestRun, PermissionLevel.Write) ||
            !User.HasPermission(PermissionEntityType.TestCaseRun, PermissionLevel.Write))
        {
            return Unauthorized();
        }
        TestRun? run = await _runManager.GetTestRunBySlugAsync(User, User.GetProjectId(), slug);
        if (run is null)
        {
            return NotFound("Test run not found");
        }

        var createdTestRun = await _mediator.Send(new DuplicateTestRunRequest(User, run));
        var exportedRun = await _mediator.Send(new ExportRunRequest(User, createdTestRun.Id));
        return Ok(exportedRun);
    }

    /// <summary>
    /// Adds a new test run.
    /// </summary>
    /// <param name="run">The test run to add.</param>
    /// <returns>The created test run.</returns>
    [Authorize("ApiKeyOrBearer")]
    [HttpPut("/api/runs")]
    [HttpPost("/api/runs")]
    [ProducesDefaultResponseType(typeof(TestRunDto))]
    [ProducesResponseType(typeof(TestRunDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ApiExplorerSettings(GroupName = "TestRuns")]
    [EndpointDescription("Adds a new test run.")]
    public async Task<IActionResult> AddAsync([FromBody] TestRunDto run)
    {
        if (!User.HasPermission(PermissionEntityType.TestRun, PermissionLevel.Write))
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