using Xunit;

namespace TestBucket.Traits.Xunit.Enrichment;
internal static class GithubActions
{
    /// <summary>
    /// Adds attachments to the test from GITHUB environment variables
    /// </summary>
    /// <param name="testContext"></param>
    public static void AddGithubActionsEnrichment(this ITestContext testContext)
    {
        var commit = Environment.GetEnvironmentVariable("GITHUB_SHA");
        var headRef = Environment.GetEnvironmentVariable("GITHUB_HEAD_REF");
        if (!string.IsNullOrWhiteSpace(commit))
        {
            testContext.AddAttachmentIfNotExists(TargetTraitNames.Commit, commit);
        }
        if (!string.IsNullOrWhiteSpace(headRef))
        {
            testContext.AddAttachmentIfNotExists(TargetTraitNames.Branch, headRef);
        }
    }
}
