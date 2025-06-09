using Xunit;

namespace TestBucket.Traits.Xunit.Enrichment;
public static class BrowserEnrichment
{
    /// <summary>
    /// Adds attachments to the test result describing what browser is used
    /// </summary>
    /// <param name="testContext"></param>
    public static void AddBrowser(this ITestContext testContext, string browser, string? version)
    {
        if (!string.IsNullOrWhiteSpace(browser))
        {
            testContext.AddAttachmentIfNotExists(TestTraitNames.Browser, browser);
        }
        if (!string.IsNullOrWhiteSpace(version))
        {
            testContext.AddAttachmentIfNotExists(TestTraitNames.BrowserVersion, version);
        }
    }
}
