using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Formats.Csv;
using TestBucket.Traits.Xunit;

namespace TestBucket.Formats.UnitTests.Csv;

/// <summary>
/// Verifies mapping beween input columns from a spreadsheet/CSV to a well-defined key that can be mapped to entities
/// </summary>
[UnitTest]
[EnrichedTest]
[Component("Test Formats")]
[Feature("Import Tests")]
[FunctionalTest]
public class TranslateHeaderTests
{
    /// <summary>
    /// Verifies that CSV/spreadsheet headers are translated to "expected-results" when expressed in different ways
    /// </summary>
    /// <param name="input"></param>
    /// <param name="expectedResult"></param>
    /// <returns></returns>
    [Theory]
    [InlineData("expected-results", "expected-results")]
    [InlineData("expected-result", "expected-results")]
    [InlineData("steps_result", "expected-results")]
    [InlineData("Steps (Expected Result)", "expected-results")]
    [InlineData("expectations", "expected-results")]
    public async Task TranslateHeader_WithDifferentWaysToExpressExpectedResults_Success(string input, string expectedResult)
    {
        var output = await HeaderTranslator.TranslateHeaderAsync(input);
        Assert.Equal(expectedResult, output);
    }

    /// <summary>
    /// Verifies that CSV/spreadsheet headers are translated to "steps" when expressed in different ways
    /// </summary>
    /// <param name="input"></param>
    /// <param name="expectedResult"></param>
    /// <returns></returns>
    [Theory]
    [InlineData("steps", "steps")]
    [InlineData("Execution steps", "steps")]
    [InlineData("test steps", "steps")]
    [InlineData("Test Steps", "steps")]
    [InlineData("Test-steps", "steps")]
    public async Task TranslateHeader_WithDifferentWaysToExpressSteps_Success(string input, string expectedResult)
    {
        var output = await HeaderTranslator.TranslateHeaderAsync(input);
        Assert.Equal(expectedResult, output);
    }

    /// <summary>
    /// Verifies that CSV/spreadsheet headers are translated to "suite" when expressed in different ways
    /// </summary>
    /// <param name="input"></param>
    /// <param name="expectedResult"></param>
    /// <returns></returns>
    [Theory]
    [InlineData("suite", "suite")]
    [InlineData("test-suite", "suite")]
    [InlineData("Test Suite", "suite")]
    public async Task TranslateHeader_WithDifferentWaysToExpressTestSuite_Success(string input, string expectedResult)
    {
        var output = await HeaderTranslator.TranslateHeaderAsync(input);
        Assert.Equal(expectedResult, output);
    }

    /// <summary>
    /// Verifies that CSV/spreadsheet headers are translated to "priority" when expressed in different ways
    /// </summary>
    /// <param name="input"></param>
    /// <param name="expectedResult"></param>
    /// <returns></returns>
    [Theory]
    [InlineData("Prio", "priority")]
    [InlineData("Test Priority", "priority")]
    [InlineData("Test Prio", "priority")]
    public async Task TranslateHeader_WithDifferentWaysToExpressPriority_Success(string input, string expectedResult)
    {
        var output = await HeaderTranslator.TranslateHeaderAsync(input);
        Assert.Equal(expectedResult, output);
    }


    /// <summary>
    /// Verifies that CSV/spreadsheet headers are translated to "category" when expressed in different ways
    /// </summary>
    /// <param name="input"></param>
    /// <param name="expectedResult"></param>
    /// <returns></returns>
    [Theory]
    [InlineData("Test Category", "category")]
    [InlineData("layer", "category")]
    public async Task TranslateHeader_WithDifferentWaysToExpressTestCategory_Success(string input, string expectedResult)
    {
        var output = await HeaderTranslator.TranslateHeaderAsync(input);
        Assert.Equal(expectedResult, output);
    }

    /// <summary>
    /// Verifies that CSV/spreadsheet headers are translated to "component" when expressed in different ways
    /// </summary>
    /// <param name="input"></param>
    /// <param name="expectedResult"></param>
    /// <returns></returns>
    [Theory]
    [InlineData("Component", "component")]
    [InlineData("SW module", "component")]
    public async Task TranslateHeader_WithDifferentWaysToExpressComponent_Success(string input, string expectedResult)
    {
        var output = await HeaderTranslator.TranslateHeaderAsync(input);
        Assert.Equal(expectedResult, output);
    }

    /// <summary>
    /// Verifies that CSV/spreadsheet headers are translated to "path" when expressed in different ways
    /// </summary>
    /// <param name="input"></param>
    /// <param name="expectedResult"></param>
    /// <returns></returns>
    [Theory]
    [InlineData("Folder path", "path")]
    [InlineData("Directory path", "path")]
    [InlineData("Folder", "path")]
    [InlineData("directory", "path")]
    [InlineData("section hierarchy", "path")]
    public async Task TranslateHeader_WithDifferentWaysToExpressPath_Success(string input, string expectedResult)
    {
        var output = await HeaderTranslator.TranslateHeaderAsync(input);
        Assert.Equal(expectedResult, output);
    }


    /// <summary>
    /// Verifies that CSV/spreadsheet headers are translated to "created-by" when expressed in different ways
    /// </summary>
    /// <param name="input"></param>
    /// <param name="expectedResult"></param>
    /// <returns></returns>
    [Theory]
    [InlineData("Created by", "created-by")]
    [InlineData("Author", "created-by")]
    [InlineData("Creator", "created-by")]
    public async Task TranslateHeader_WithDifferentWaysToExpressCreatedBy_Success(string input, string expectedResult)
    {
        var output = await HeaderTranslator.TranslateHeaderAsync(input);
        Assert.Equal(expectedResult, output);
    }
}
