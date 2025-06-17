
# TestBucket.Metrics.Xunit

Provides diagnostics metrics from to xunit reports

# Usage
```csharp
[EnrichedTest]
[IncludeDiagnostics]
public class ExampleTests
{
    [Fact]
    public async Task Example_Test_Case()
    {
    }
```