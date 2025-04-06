using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using TestBucket.Contracts.TestResources;
using TestBucket.Controllers.Api;
using TestBucket.Domain.Projects;
using TestBucket.Domain.Testing;
using TestBucket.Domain.TestResources;
using TestBucket.Formats;

namespace TestBucket.Components.TestResources.ApiControllers;

[ApiController]
public class TestResourceApiController : ProjectApiControllerBase
{
    private readonly ITestResourceManager _testResourceManager;

    public TestResourceApiController(ITestResourceManager testResourceManager)
    {
        _testResourceManager = testResourceManager;
    }

    /// <summary>
    /// This API is called from a resource server, and contains a list of resources (e.g. devices or similar)
    /// </summary>
    /// <returns></returns>
    [Authorize("ApiKeyOrBearer")]
    [HttpPut("/api/test-resources")]
    [HttpPost("/api/test-resources")]
    public async Task<IActionResult> PutTestResourcesAsync(List<TestResourceDto> resources)
    {
        await _testResourceManager.UpdateResourcesFromResourceServerAsync(User,resources);
        return Ok();
    }
}
