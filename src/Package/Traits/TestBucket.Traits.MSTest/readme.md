
# TestBucket Traits

Standardized traits and enrichment attributes for .NET unit tests for MSTest v4

# Usage
```csharp
using TestBucket.Traits.MSTest;

namespace MSTestSample;

[TestClass]
public sealed class Test1
{
    [CoveredRequirement("TB-123")]
    [UnitTest]
    [TestMethod]
    public void TestMethod1()
    {
    }
}
```