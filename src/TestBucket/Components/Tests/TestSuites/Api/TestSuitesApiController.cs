using System.Net;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using TestBucket.Controllers.Api;
using TestBucket.Domain.Projects;
using TestBucket.Domain.Shared;
using TestBucket.Domain.Teams;
using TestBucket.Domain.Testing.Mapping;
using TestBucket.Domain.Testing.TestSuites;
using TestBucket.Formats.Dtos;

namespace TestBucket.Components.Tests.TestSuites.Api;

[ApiController]
public class TestSuitesApiController : ProjectApiControllerBase
{
    private readonly ITeamManager _teamManager;
    private readonly ITestSuiteManager _testSuiteManager;
    private readonly IProjectManager _projectManager;

    public TestSuitesApiController(ITestSuiteManager testSuiteManager, ITeamManager teamManager, IProjectManager projectManager)
    {
        _testSuiteManager = testSuiteManager;
        _teamManager = teamManager;
        _projectManager = projectManager;
    }

    [Authorize("ApiKeyOrBearer")]
    [HttpPut("/api/testsuites")]
    [HttpPost("/api/testsuites")]
    [ProducesDefaultResponseType(typeof(TestSuiteDto))]
    public async Task<IActionResult> AddAsync([FromBody] TestSuiteDto testSuite)
    {
        if(!User.HasPermission(PermissionEntityType.TestCase, PermissionLevel.Write))
        {
            return Unauthorized();
        }
        if(string.IsNullOrEmpty(testSuite.TeamSlug))
        {
            return BadRequest("Team is missing");
        }
        if (string.IsNullOrEmpty(testSuite.ProjectSlug))
        {
            return BadRequest("Project is missing");
        }

        var dbo = testSuite.ToDbo();

        // Resolve team/project from slug
        var team = await _teamManager.GetTeamBySlugAsync(User, testSuite.TeamSlug);
        var project = await _projectManager.GetTestProjectBySlugAsync(User, testSuite.ProjectSlug);
        dbo.TestProjectId = project?.Id;
        dbo.TeamId = team?.Id;

        try
        {
            await _testSuiteManager.AddTestSuiteAsync(User, dbo);
            var response = dbo.ToDto();
            response.TeamSlug = team?.Slug;
            response.ProjectSlug = project?.Slug;
            return Ok(response);
        }
        catch(Exception ex)
        {
            throw new Exception($"Error adding suite '{dbo.Slug}' with team={testSuite.TeamSlug}, project={testSuite.ProjectSlug}", ex);
        }
    }

    [Authorize("ApiKeyOrBearer")]
    [EndpointDescription("Gets an existing test suite")]
    [HttpGet("/api/testsuites/{slug}")]
    public async Task<IActionResult> GetAsync(string slug)
    {
        if (!User.HasPermission(PermissionEntityType.TestCase, PermissionLevel.Read))
        {
            return Unauthorized();
        }
        var projectId = User.GetProjectId();
        var suite = await _testSuiteManager.GetTestSuiteBySlugAsync(User, projectId, slug);
        if (suite is null)
        {
            return NotFound();
        }
        return Ok(suite.ToDto());
    }

    [Authorize("ApiKeyOrBearer")]
    [HttpDelete("/api/testsuites/{slug}")]
    [ProducesDefaultResponseType(typeof(TestSuiteDto))]
    public async Task<IActionResult> DeleteAsync([FromRoute] string slug)
    {
        if (!User.HasPermission(PermissionEntityType.TestCase, PermissionLevel.Write))
        {
            return Unauthorized();
        }

        var testSuite = await _testSuiteManager.GetTestSuiteBySlugAsync(User, User.GetProjectId(), slug);
        if(testSuite is null)
        {
            return NotFound();
        }

        await _testSuiteManager.DeleteTestSuiteByIdAsync(User, testSuite.Id);
        return Ok();
    }
}
