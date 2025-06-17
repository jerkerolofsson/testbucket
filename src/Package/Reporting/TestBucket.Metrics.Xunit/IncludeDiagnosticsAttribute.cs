using System.Collections.Concurrent;
using System.Reflection;

using TestBucket.Traits.Xunit;

using Xunit;
using Xunit.v3;

namespace TestBucket.Metrics.Xunit;
public class IncludeDiagnosticsAttribute : BeforeAfterTestAttribute
{
    private static readonly ConcurrentDictionary<IXunitTest, DiagnosticsEventListener> _diagnosticsEventListeners = [];

    [ThreadStatic]
    internal static DiagnosticsEventListener? Current;

    public override void Before(MethodInfo methodUnderTest, IXunitTest test)
    {
        DiagnosticsEventListener listener = new DiagnosticsEventListener();
        Current = listener;
        _diagnosticsEventListeners[test] = listener;
    }

    public override void After(MethodInfo methodUnderTest, IXunitTest test)
    {
        if (_diagnosticsEventListeners.TryGetValue(test, out var listener))
        {
            foreach (var metric in listener.CreateMetrics())
            {
                TestContext.Current.AddMetric(metric);
            }
        }
    }
}
