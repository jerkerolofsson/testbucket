using Humanizer;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using TestBucket.Contracts.TestAccounts;
using TestBucket.Controllers.Api;
using TestBucket.Domain.Shared;
using TestBucket.Domain.TestAccounts;
using TestBucket.Domain.TestAccounts.Mapping;

namespace TestBucket.Components.TestAccounts.ApiControllers;

[ApiController]
public class TestAccountApiController : ProjectApiControllerBase
{
    private readonly ITestAccountManager _testAccountManager;

    public TestAccountApiController(ITestAccountManager testAccountManager)
    {
        _testAccountManager = testAccountManager;
    }

    /// <summary>
    /// This API is called from a resource server, and contains a list of resources (e.g. devices or similar)
    /// </summary>
    /// <returns></returns>
    [Authorize("ApiKeyOrBearer")]
    [HttpPut("/api/test-accounts")]
    [HttpPost("/api/test-accounts")]
    public async Task<IActionResult> PutTestAccountAsync(TestAccountDto account)
    {
        try
        {
            var dbo = account.ToDbo();
            await _testAccountManager.AddAsync(User, dbo);
            var result = dbo.ToDto();
            return new ObjectResult(result) { StatusCode = StatusCodes.Status201Created };
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized();
        }
    }
}
