using TestBucket.Domain.Automation.Helpers;
using TestBucket.Formats;

using TestResult = TestBucket.Contracts.Testing.Models.TestResult;

namespace TestBucket.Domain.UnitTests.Automation.Helpers;

/// <summary>
/// Unit tests for the <see cref="TestRunnerResultParser"/> class.
/// </summary>
[UnitTest]
[EnrichedTest]
[FunctionalTest]
[Component("Automation")]
[Feature("AI Runner")]
public class TestRunnerResultParserTests
{
    /// <summary>
    /// Tests that ParseSingleResult returns a valid TestCaseRunDto when provided with valid content and format.
    /// </summary>
    [Fact]
    public void ParseSingleResult_ValidContentAndFormat_ReturnsTestCaseRunDto()
    {
        // Arrange
        var content = """
            <testsuites time="15.682687">
            	<testsuite name="Tests.Registration" time="6.605871">
            		<testcase name="testCase1" classname="Tests.Registration" time="2.113871" />
            	</testsuite>
            </testsuites>
            """;
        var format = TestResultFormat.JUnitXml;

        // Act
        var result = TestRunnerResultParser.ParseSingleResult(content, format);

        // Assert
        Assert.NotNull(result?.Result);
        Assert.Equal(TestResult.Passed, result.Result.Value);
    }

    /// <summary>
    /// Tests that ParseSingleResult returns null when provided with null content.
    /// </summary>
    [Fact]
    public void ParseSingleResult_NullContent_ReturnsNull()
    {
        // Act
        var result = TestRunnerResultParser.ParseSingleResult(null, TestResultFormat.JUnitXml);

        // Assert
        Assert.Null(result);
    }

    /// <summary>
    /// Tests that ParseSingleResult returns null when provided with an unknown format.
    /// </summary>
    [Fact]
    public void ParseSingleResult_UnknownFormat_ReturnsNull()
    {
        // Act
        var result = TestRunnerResultParser.ParseSingleResult("content", TestResultFormat.UnknownFormat);

        // Assert
        Assert.Null(result);
    }

    /// <summary>
    /// Tests that ParseSingleResult returns null when no test suites are present in the content.
    /// </summary>
    [Fact]
    public void ParseSingleResult_NoSuites_ReturnsNull()
    {
        // Arrange
        var junitXml = """
            <testsuites time="15.682687">
            	<testsuite name="Tests.Registration" time="6.605871">
            	</testsuite>
            </testsuites>
            """;
        var format = TestResultFormat.JUnitXml;

        // Act
        var result = TestRunnerResultParser.ParseSingleResult(junitXml, format);

        // Assert
        Assert.Null(result);
    }

    /// <summary>
    /// Tests that ParseSingleResult returns null when the first test suite contains no test cases.
    /// </summary>
    [Fact]
    public void ParseSingleResult_NoTestsInFirstSuite_ReturnsNull()
    {
        // Arrange
        var content = """
            <testsuites time="15.682687">
            	<testsuite name="Tests.Registration" time="6.605871">
            	</testsuite>
            </testsuites>
            """;
        var format = TestResultFormat.JUnitXml;

        // Act
        var result = TestRunnerResultParser.ParseSingleResult(content, format);

        // Assert
        Assert.Null(result);
    }
}
