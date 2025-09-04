using TestBucket.Domain.Files;
using TestBucket.Domain.Files.Models;
using TestBucket.Domain.Resources;
using TestBucket.Domain.Testing.Models;

namespace TestBucket.Domain.AI.Runner;
public static class IFileResourceManagerArtifactExtensions
{
    /// <summary>
    /// Saves artifacts as resources associated with the test case run
    /// </summary>
    /// <param name="fileResourceManager"></param>
    /// <param name="principal"></param>
    /// <param name="testCaseRun"></param>
    /// <param name="artifacts"></param>
    /// <returns></returns>
    public static async Task SaveArtifactsAsync(this IFileResourceManager fileResourceManager, ClaimsPrincipal principal, TestCaseRun testCaseRun, Dictionary<string, byte[]> artifacts)
    {
        foreach (var artifactContent in artifacts)
        {
            var name = artifactContent.Key;
            var artifactBytes = artifactContent.Value;

            var contentType = MediaTypeDetector.DetectType(name, null, artifactBytes);
            var resource = new FileResource
            {
                ContentType = contentType,
                Data = artifactBytes,
                TenantId = principal.GetTenantIdOrThrow(),
                TestCaseRunId = testCaseRun.Id,
            };
            await fileResourceManager.AddResourceAsync(principal, resource);
        }
    }
}
