using Xunit;

namespace TestBucket.Traits.Xunit.Enrichment;
internal static class GitlabCi
{
    /// <summary>
    /// Adds attachments to the test from GITHUB environment variables
    /// </summary>
    /// <param name="testContext"></param>
    public static void AddGitlabCiEnrichment(this ITestContext testContext)
    {
        var commit = Environment.GetEnvironmentVariable("CI_COMMIT_SHA");
        var headRef = Environment.GetEnvironmentVariable("CI_COMMIT_REF_NAME");
        if (!string.IsNullOrWhiteSpace(commit))
        {
            testContext.AddAttachment(TargetTraitNames.Commit, commit);
        }
        if (!string.IsNullOrWhiteSpace(headRef))
        {
            testContext.AddAttachment(TargetTraitNames.Branch, headRef);
        }
    }
}
