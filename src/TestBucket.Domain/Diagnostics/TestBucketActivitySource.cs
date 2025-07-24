using System.Diagnostics;

namespace TestBucket.Domain.Diagnostics;
internal class TestBucketActivitySource
{
    private static readonly ActivitySource s_activitySource = new ActivitySource("TestBucket");

    public static ActivitySource ActivitySource  => s_activitySource;
}
