using TestBucket.Domain.Automation.Artifact;
using TestBucket.Domain.Code;

namespace TestBucket.Domain.UnitTests.Code;

/// <summary>
/// Unit tests for the GlobMatcher class, which is responsible for matching file paths against glob patterns.
/// </summary>
[Component("Code")]
[UnitTest]
[EnrichedTest]
[FunctionalTest]
public class GlobMatcherTests
{
    /// <summary>
    /// Tests if GlobMatcher.IsMatch returns true when the file path matches the default coverage report pattern.
    /// </summary>
    [Fact]
    public void IsMatch_WithPath_ReturnsTrue()
    {
        // Arrange
        var filePath = @"tests/TestBucket.Domain.UnitTests/bin/Debug/net9.0/TestResults/TestBucket.Domain.UnitTests.coverage.cobertura.xml";
        string[] patterns = [DefaultGlobPatterns.DefaultCoverageReportArtifactsPattern];

        // Act
        var result = GLobMatcher.IsMatch(filePath, patterns);

        // Assert
        Assert.True(result);
    }

    /// <summary>
    /// Tests if GlobMatcher.IsMatch returns true when the file path exactly matches a single pattern.
    /// </summary>
    [Fact]
    public void IsMatch_SinglePatterWithExactMatch_ReturnsTrue()
    {
        // Arrange
        var filePath = "Program.cs";
        var patterns = new[] { "Program.cs" };

        // Act
        var result = GLobMatcher.IsMatch(filePath, patterns);

        // Assert
        Assert.True(result);
    }

    /// <summary>
    /// Tests if GlobMatcher.IsMatch returns true when the file path matches a single pattern with a filename wildcard.
    /// </summary>
    [Fact]
    public void IsMatch_SinglePatternWithFilenameWildcard_ReturnsTrue()
    {
        // Arrange
        var filePath = "Program.cs";
        var patterns = new[] { "*.cs" };

        // Act
        var result = GLobMatcher.IsMatch(filePath, patterns);

        // Assert
        Assert.True(result);
    }

    /// <summary>
    /// Tests if GlobMatcher.IsMatch returns true when the file path matches a single pattern with a path wildcard.
    /// </summary>
    [Fact]
    public void IsMatch_SinglePatternWithPathWildcard_ReturnsTrue()
    {
        // Arrange
        var filePath = "src/Program.cs";
        var patterns = new[] { "**/*.cs" };

        // Act
        var result = GLobMatcher.IsMatch(filePath, patterns);

        // Assert
        Assert.True(result);
    }

    /// <summary>
    /// Tests if GlobMatcher.IsMatch returns true when the file path matches one of multiple patterns.
    /// </summary>
    [Fact]
    public void IsMatch_MultiplePatterns_ReturnsTrue()
    {
        // Arrange
        var filePath = "src/Program.cs";
        var patterns = new[] { "**/*.txt", "**/*.cs" };

        // Act
        var result = GLobMatcher.IsMatch(filePath, patterns);

        // Assert
        Assert.True(result);
    }

    /// <summary>
    /// Tests if GlobMatcher.IsMatch returns false when the file path is excluded by a negated pattern.
    /// </summary>
    [Fact]
    public void IsMatch_NegatedPattern_ReturnsFalse()
    {
        // Arrange
        var filePath = "src/Program.cs";
        var patterns = new[] { "!**/*.cs" };

        // Act
        var result = GLobMatcher.IsMatch(filePath, patterns);

        // Assert
        Assert.False(result);
    }

    /// <summary>
    /// Tests if GlobMatcher.IsMatch returns false when a negated pattern is specified before a matching pattern.
    /// </summary>
    [Fact]
    public void IsMatch_NegatedPatternBeforeMatching_ReturnsFalse()
    {
        // Arrange
        var filePath = "src/Program.cs";
        var patterns = new[] { "!**/*.cs", "**/*.cs" };

        // Act
        var result = GLobMatcher.IsMatch(filePath, patterns);

        // Assert
        Assert.False(result);
    }

    /// <summary>
    /// Tests if GlobMatcher.IsMatch returns false when the patterns array is empty or contains null/empty strings.
    /// </summary>
    [Fact]
    public void IsMatch_EmptyPatterns_ReturnsFalse()
    {
        // Arrange
        var filePath = "src/Program.cs";
        var patterns = new[] { "", null! };

        // Act
        var result = GLobMatcher.IsMatch(filePath, patterns);

        // Assert
        Assert.False(result);
    }

    /// <summary>
    /// Tests if GlobMatcher.IsMatch returns false when the file path does not match any of the patterns.
    /// </summary>
    [Fact]
    public void IsMatch_NoMatch_ReturnsFalse()
    {
        // Arrange
        var filePath = "src/Program.cs";
        var patterns = new[] { "*.txt", "*.md" };

        // Act
        var result = GLobMatcher.IsMatch(filePath, patterns);

        // Assert
        Assert.False(result);
    }
}