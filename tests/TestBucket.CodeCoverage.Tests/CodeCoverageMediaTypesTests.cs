using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using TestBucket.CodeCoverage;

namespace TestBucket.CodeCoverage.Tests;

/// <summary>
/// Tests for CodeCoverageMediaTypes
/// </summary>
[Feature("Code Coverage")]
[Component("Code Coverage")]
[UnitTest]
[EnrichedTest]
[FunctionalTest]
public class CodeCoverageMediaTypesTests
{
    /// <summary>
    /// Verifies that the default Cobertura media type is set correctly
    /// </summary>
    [Fact]
    public void Cobertura_Constant_IsCorrect()
    {
        Assert.Equal("application/x-cobertura", CodeCoverageMediaTypes.Cobertura);
    }

    /// <summary>
    /// Verfifies that IsCodeCoverageFile correctly identifies Cobertura media types, ignoring case and handling null/empty strings.
    /// </summary>
    /// <param name="mediaType"></param>
    /// <param name="expected"></param>
    [Theory]
    [InlineData("application/x-cobertura", true)]
    [InlineData("application/x-cobertura+xml", true)]
    [InlineData("APPLICATION/X-COBERTURA", true)]
    [InlineData("application/json", false)]
    [InlineData("", false)]
    [InlineData(null, false)]
    public void IsCodeCoverageFile_ReturnsExpected(string? mediaType, bool expected)
    {
        var result = CodeCoverageMediaTypes.IsCodeCoverageFile(mediaType);
        Assert.Equal(expected, result);
    }
}
