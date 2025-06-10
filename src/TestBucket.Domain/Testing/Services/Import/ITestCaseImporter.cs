using TestBucket.Formats;

namespace TestBucket.Domain.Testing;
public interface ITestCaseImporter
{
    Task ImportAsync(ClaimsPrincipal principal, long teamId, long projectId, byte[] bytes, string mediaType, ImportHandlingOptions options);
}