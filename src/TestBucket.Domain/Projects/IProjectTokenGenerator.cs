
namespace TestBucket.Domain.Projects;

internal interface IProjectTokenGenerator
{
    /// <summary>
    /// Generates a short lived access token for the specific project and tenant
    /// 
    /// The token has write access to test cases and test runs
    /// </summary>
    /// <param name="tenantId"></param>
    /// <param name="projectId"></param>
    /// <returns></returns>
    Task<string> GenerateCiCdAccessTokenAsync(string tenantId, long projectId, long testRunId, long? testSuiteId);
}