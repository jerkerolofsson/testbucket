using TestBucket.Contracts.Testing.Models;
using TestBucket.Domain.Testing.TestCases.Templates;

namespace TestBucket.Domain.UnitTests.Testing.Templates;

/// <summary>
/// Unit tests for various test case templates.
/// </summary>
[Feature("Templates")]
[Component("Testing")]
[UnitTest]
[EnrichedTest]
[FunctionalTest]
public class TemplateTests
{
    /// <summary>
    /// Verifies that the PowershellTemplate correctly applies values to a test case.
    /// </summary>
    [Fact]
    public async Task PowershellTemplate_ApplyAsync_SetsCorrectValues()
    {
        // Arrange
        var template = new PowershellTemplate();
        var testCase = new TestCase { Name = "TestCase1" };

        // Act
        await template.ApplyAsync(testCase);

        // Assert
        Assert.Equal("pwsh", testCase.RunnerLanguage);
        Assert.Equal(TestExecutionType.Automated, testCase.ExecutionType);
        Assert.Equal(ScriptType.ScriptedDefault, testCase.ScriptType);
        Assert.Contains("Write-Host \"Hello, World\"", testCase.Description);
    }

    /// <summary>
    /// Verifies that the PythonTemplate correctly applies values to a test case.
    /// </summary>
    [Fact]
    public async Task PythonTemplate_ApplyAsync_SetsCorrectValues()
    {
        // Arrange
        var template = new PythonTemplate();
        var testCase = new TestCase { Name = "TestCase2" };

        // Act
        await template.ApplyAsync(testCase);

        // Assert
        Assert.Equal("python", testCase.RunnerLanguage);
        Assert.Equal(TestExecutionType.Automated, testCase.ExecutionType);
        Assert.Equal(ScriptType.ScriptedDefault, testCase.ScriptType);
        Assert.Contains("print(\"Hello, World\")", testCase.Description);
    }

    /// <summary>
    /// Verifies that the BlankTemplate correctly applies values to a test case.
    /// </summary>
    [Fact]
    public async Task BlankTemplate_ApplyAsync_SetsCorrectValues()
    {
        // Arrange
        var template = new BlankTemplate();
        var testCase = new TestCase { Name = "TestCase3" };

        // Act
        await template.ApplyAsync(testCase);

        // Assert
        Assert.Null(testCase.RunnerLanguage);
        Assert.Equal(TestExecutionType.Manual, testCase.ExecutionType);
        Assert.Equal(ScriptType.ScriptedDefault, testCase.ScriptType);
    }

    /// <summary>
    /// Verifies that the DotHttpTemplate correctly applies values to a test case.
    /// </summary>
    [Fact]
    public async Task DotHttpTemplate_ApplyAsync_SetsCorrectValues()
    {
        // Arrange
        var template = new DotHttpTemplate();
        var testCase = new TestCase { Name = "TestCase4" };

        // Act
        await template.ApplyAsync(testCase);

        // Assert
        Assert.Equal("http", testCase.RunnerLanguage);
        Assert.Equal(TestExecutionType.Automated, testCase.ExecutionType);
        Assert.Equal(ScriptType.ScriptedDefault, testCase.ScriptType);
        Assert.Contains("# @name TestCase4", testCase.Description);
    }

    /// <summary>
    /// Verifies that the DotHttpMcpTemplate correctly applies values to a test case.
    /// </summary>
    [Fact]
    public async Task DotHttpMcpTemplate_ApplyAsync_SetsCorrectValues()
    {
        // Arrange
        var template = new DotHttpMcpTemplate();
        var testCase = new TestCase { Name = "TestCase5" };

        // Act
        await template.ApplyAsync(testCase);

        // Assert
        Assert.Equal("http", testCase.RunnerLanguage);
        Assert.Equal(TestExecutionType.Automated, testCase.ExecutionType);
        Assert.Equal(ScriptType.ScriptedDefault, testCase.ScriptType);
        Assert.Contains("# @name TestCase5", testCase.Description);
        Assert.Contains("# @verify mcp success", testCase.Description);
    }
}
