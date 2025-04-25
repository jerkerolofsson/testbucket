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
    private readonly ITestSuiteManager _testSuiteManager;
    private readonly IProjectManager _projectManager;

    public TestCaseApiController(ITestCaseManager testCaseManager, ITeamManager teamManager, IProjectManager projectManager, ITestSuiteManager testSuiteManager)
    {
        _testCaseManager = testCaseManager;
        _teamManager = teamManager;
        _projectManager = projectManager;
        _testSuiteManager = testSuiteManager;
    }

    [Authorize("ApiKeyOrBearer")]
    [EndpointDescription("Gets an existing test case")]
    [HttpGet("/api/testcases/{slug}")]
    public async Task<IActionResult> GetAsync(string slug)
    {
        if (!User.HasPermission(PermissionEntityType.TestCase, PermissionLevel.Read))
        {
            return Unauthorized();
        }
        var testCase = await _testCaseManager.GetTestCaseBySlugAsync(User, slug);
        if(testCase is null)
        {
            return NotFound();
        }
        return Ok(testCase.ToDto());
    }

    [Authorize("ApiKeyOrBearer")]
    [EndpointDescription("Deletes the test case")]
    [HttpDelete("/api/testcases/{slug}")]
    public async Task<IActionResult> DeleteAsync(string slug)
    {
        if (!User.HasPermission(PermissionEntityType.TestCase, PermissionLevel.Delete))
        {
            return Unauthorized();
        }
        var testCase = await _testCaseManager.GetTestCaseBySlugAsync(User, slug);
        if (testCase is null)
        {
            return NotFound();
        }
        await _testCaseManager.DeleteTestCaseAsync(User, testCase);
        return Ok();
    }

    [Authorize("ApiKeyOrBearer")]
    [EndpointDescription("Creates a copy of the test case")]
    [HttpPost("/api/testcases/{slug}/duplicate")]
    [ProducesDefaultResponseType(typeof(TestCaseDto))]
    public async Task<IActionResult> DuplicateAsync(string slug)
    {
        if (!User.HasPermission(PermissionEntityType.TestCase, PermissionLevel.Write))
        {
            return Unauthorized();
        }
        var testCase = await _testCaseManager.GetTestCaseBySlugAsync(User, slug);
        if (testCase is null)
        {
            return NotFound($"Test case with slug '{slug}' was not found");
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
        if(string.IsNullOrEmpty(testCase.TeamSlug))
        {
            return BadRequest("Team is missing");
        }
        if (string.IsNullOrEmpty(testCase.ProjectSlug))
        {
            return BadRequest("Project is missing");
        }
        if (string.IsNullOrEmpty(testCase.TestSuiteSlug))
        {
            return BadRequest("Test suite is missing");
        }
        var dbo = testCase.ToDbo();

        var team = await _teamManager.GetTeamBySlugAsync(User, testCase.TeamSlug);
        var project = await _projectManager.GetTestProjectBySlugAsync(User, testCase.ProjectSlug);
        var testSuite = await _testSuiteManager.GetTestSuiteBySlugAsync(User, testCase.TestSuiteSlug);

        if (team is null) return NotFound("Team not found");
        if (project is null) return NotFound("Project not found");
        if (testSuite is null) return NotFound("Test Suite not found");

        dbo.TestProjectId = project.Id;
        dbo.TeamId = team.Id;
        dbo.TestSuiteId = testSuite.Id;

        await _testCaseManager.AddTestCaseAsync(User, dbo);
        var response = dbo.ToDto();
        response.TestSuiteSlug = 
        response.TeamSlug = team?.Slug;
        response.ProjectSlug = project?.Slug;
        response.TestSuiteSlug = testSuite?.Slug;
        return Ok(response);
    }
}
