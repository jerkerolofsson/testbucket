using TestBucket.CodeCoverage.Parsers;

namespace TestBucket.CodeCoverage.Tests.CSharp;

/// <summary>
/// Tests related to parsing of code coverage report with C# async methods
/// </summary>
[Feature("Code Coverage")]
[Component("Code Coverage")]
[UnitTest]
[EnrichedTest]
[FunctionalTest]
public class CSharpAsyncCoverageTests
{
    /// <summary>
    /// Verifies that async classes are merged with the real class (as defining it in the code)
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task ParseTextAsync_WithCoberturaParser_AndAsyncMoveNextMethodInXml_AsyncClassMergedWithOtherClass()
    {
        string xml = """
            <coverage line-rate="0.5" branch-rate="0" complexity="10485" version="1.9" timestamp="1757424625" lines-covered="7612" lines-valid="24908" branches-covered="1737" branches-valid="6670">
                <packages>
                    <package line-rate="0.5" branch-rate="0" complexity="10462" name="TestBucket.Domain">
                        <classes>
                            <class line-rate="0.5" branch-rate="0" complexity="2" name="TestBucket.Domain.Fields.FieldManager.&lt;UpsertRequirementFieldAsync&gt;d__10" filename="src\TestBucket.Domain\Fields\FieldManager.cs">
                                <methods>
                                    <method line-rate="0.5" branch-rate="0" complexity="2" name="MoveNext" signature="()">
                                        <lines>
                                            <line number="173" hits="1" branch="False" />
                                            <line number="174" hits="0" branch="False" />
                                        </lines>
                                    </method>
                                </methods>
                                <lines>
                                    <line number="173" hits="1" branch="False" />
                                    <line number="174" hits="0" branch="False" />
                                </lines>
                            </class>
                        </classes>
                    </package>
                </packages>
            </coverage>
            """;

        var parser = new CoberturaParser();
        var report = await parser.ParseTextAsync(xml, TestContext.Current.CancellationToken);

        Assert.Single(report.Packages);
        Assert.Single(report.Packages[0].Classes);
        Assert.Single(report.Packages[0].Classes[0].Methods);
        Assert.Equal("TestBucket.Domain.Fields.FieldManager", report.Packages[0].Classes[0].Name);
        var method = report.Packages[0].Classes[0].Methods[0];
        Assert.Equal("UpsertRequirementFieldAsync", method.Name);
        Assert.Equal(2, method.LineCount.Value);
        Assert.Equal(1, method.CoveredLineCount.Value);
        Assert.Equal(50.0, method.CoveragePercent.Value);
    }
}
