
# TestBucket Traits

Standardized traits and enrichment attributes for .NET unit tests.

- Standardize properties/traits/attachments in test result files by using a shared naming convention across projects
- Enrich output by collecting additional data from tests

## Traits, Properties, Attachments

Depending on test framework (MSTest/xUnit/TUnit/NUnit..) and output formats (xunit, TRX, junitxml, CTRF etc) the attributes added by TestBucket traits may not match 1-to-1, but a generalized usage is as follows:

- Traits: Describes traits for the test case. Often these may show up as groupings in Visual Studio, Rider or other tools. For practical reasons these should be limited.
- Properties/Attachments: If the data is not suitable as a trait, it will be exported as a property or attachment (depending on output format).

## Attributes

Attributes are named the same in both ```TestBucket.Traits.Xunit``` and ```TestBucket.Traits.TUnit``` packages to the extent possible.

### xUnit

```csharp
using TestBucket.Traits.Xunit;

namespace MyTests 
{
    [UnitTest]
    [EnrichedTest]
    public class MyTestClass
    {
        [Fact]
        public void MyTestMethod()
        {
        }
    }
}
```

### TUnit

```csharp
using TestBucket.Traits.TUnit;

namespace MyTests 
{
    [UnitTest]
    public class MyTestClass
    {
        [Test]
        public void MyTestMethod()
        {
        }
    }
}
```

## Test Category

The following attributes create a TestCategory trait:
- UnitTest
- ApiTest
- IntegrationTest
- EndToEndTest

### Junit XML output from xunit runner

```xml
<testcase name="A">
  <properties>
    <property name="trait:TestCategory" value="Unit" />
  </properties>
</testcase>
```

The xunit runner adds a trait prefix as traits are not supported in standard junitxml files

### Xunit XML

```xml
<test name="A">
  <traits>
    <trait name="TestCategory" value="Unit" />
  </traits>
</test>
```

## Enriched Tests in xUnit

The ```EnrichedTestAttribute``` adds attachments (xunit) which are key-value pairs similar to traits (but they won't show up in Visual Studio).
This enables attachment traits, such as:
- TestDescription
- TestId


