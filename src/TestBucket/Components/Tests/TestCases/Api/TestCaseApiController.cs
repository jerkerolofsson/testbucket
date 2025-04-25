using System.Net;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using TestBucket.Controllers.Api;
using TestBucket.Domain.Projects;
using TestBucket.Domain.Teams;
using TestBucket.Domain.Testing.Mapping;

namespace TestBucket.Components.Tests.TestCases.Api;

[ApiController]
public class TestCaseApiController : ProjectApiControllerBase
{
    private readonly ITeamManager _teamManager;
    private readonly ITestCaseManager _testCaseManager;
    private readonly IProjectManager _projectManager;

    public TestCaseApiController(ITestCaseManager testCaseManager, ITeamManager teamManager, IProjectManager projectManager)
    {
        _testCaseManager = testCaseManager;
        _teamManager = teamManager;
        _projectManager = projectManager;
    }

    [HttpPost("/api/testcases/{testCaseId:long}/duplicate")]
    [ProducesDefaultResponseType(typeof(TestCaseDto))]
    public async Task<IActionResult> DuplicateAsync(long testCaseId)
    {
        if (!User.HasPermission(PermissionEntityType.TestCase, PermissionLevel.Write))
        {
            return Unauthorized();
        }
        var testCase = await _testCaseManager.GetTestCaseByIdAsync(User, testCaseId);
        if (testCase is null)
        {
            return NotFound();
        }
        var dbo = await _testCaseManager.DuplicateTestCaseAsync(User, testCase);
        var response = dbo.ToDto();
        return Ok(response);
    }

    [Authorize("ApiKeyOrBearer")]
    [HttpPut("/api/testcases")]
    [HttpPost("/api/testcases")]
    [ProducesDefaultResponseType(typeof(TestCaseDto))]
    public async Task<IActionResult> AddAsync([FromBody] TestCaseDto testCase)
    {
        if(!User.HasPermission(PermissionEntityType.TestCase, PermissionLevel.Write))
        {
            return Unauthorized();
        }
        if(string.IsNullOrEmpty(testCase.Team))
        {
            return BadRequest("Team is missing");
        }
        if (string.IsNullOrEmpty(testCase.Project))
        {
            return BadRequest("Project is missing");
        }

        var dbo = testCase.ToDbo();

        var team = await _teamManager.GetTeamBySlugAsync(User, testCase.Team);
        var project = await _projectManager.GetTestProjectBySlugAsync(User, testCase.Project);
        dbo.TestProjectId = project?.Id;
        dbo.TeamId = project?.Id;

        await _testCaseManager.AddTestCaseAsync(User, dbo);
        var response = dbo.ToDto();
        response.Team = team?.Slug;
        response.Project = project?.Slug;
        return Ok(response);
    }
}
