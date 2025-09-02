using TestBucket.Formats;

namespace TestBucket.Domain.Testing;
public interface ITestCaseImporter
{
    /// <summary>
    /// Imports test cases
    /// </summary>
    /// <param name="principal"></param>
    /// <param name="teamId"></param>
    /// <param name="projectId"></param>
    /// <param name="bytes"></param>
    /// <param name="mediaType"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    Task ImportAsync(ClaimsPrincipal principal, long teamId, long projectId, byte[] bytes, string mediaType, ImportHandlingOptions options);
}