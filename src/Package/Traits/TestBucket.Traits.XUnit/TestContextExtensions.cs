using TestBucket.Traits.Core.Metrics;

using Xunit;

namespace TestBucket.Traits.Xunit;
public static class TestContextExtensions
{
    /// <summary>
    /// Adds a metric or a numerical result to the test result for the current test.
    /// </summary>
    /// <param name="testContext"></param>
    /// <param name="metric"></param>
    public static void AddMetric(this ITestContext testContext, TestResultMetric metric)
    {
        var key = MetricSerializer.SerializeName(metric);
        var value = MetricSerializer.SerializeValue(metric) + MetricSerializer.SerializeCreatedSuffix(metric);
        testContext.AddAttachmentIfNotExists(key, value);
    }

    public static void AddAttachmentIfNotExists(this ITestContext testContext, string key, string value)
    {
        if (testContext.Attachments?.Any(x => x.Key == key) == true)
        {
            return;
        }
        testContext.AddAttachment(key, value);
    }
}
