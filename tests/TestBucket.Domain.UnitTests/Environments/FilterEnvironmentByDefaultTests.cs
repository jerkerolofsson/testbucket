using TestBucket.Domain.Environments.Models;
using TestBucket.Domain.Environments.Specifications;

namespace TestBucket.Domain.UnitTests.Environments;

/// <summary>
/// Unit tests for the <see cref="FilterEnvironmentByDefault"/> specification,
/// verifying that only default environments are filtered as expected.
/// </summary>
[EnrichedTest]
[UnitTest]
[Component("Environments")]
[FunctionalTest]
public class FilterEnvironmentByDefaultTests
{
    /// <summary>
    /// Verifies that <see cref="FilterEnvironmentByDefault"/> filters only environments
    /// where <c>Default</c> is <c>true</c>.
    /// </summary>
    [Fact]
    public void GetExpression_FiltersOnlyDefaultEnvironments()
    {
        // Arrange
        var environments = new List<TestEnvironment>
        {
            new TestEnvironment { Name = "env1", Default = false },
            new TestEnvironment { Name = "env2", Default = true },
            new TestEnvironment { Name = "env3", Default = false },
            new TestEnvironment { Name = "env4", Default = true }
        };

        var spec = new FilterEnvironmentByDefault();
        var expr = spec.Expression.Compile();

        // Act
        var result = environments.Where(expr).ToList();

        // Assert
        Assert.Equal(2, result.Count);
        Assert.All(result, e => Assert.True(e.Default));
        Assert.Contains(result, e => e.Name == "env2");
        Assert.Contains(result, e => e.Name == "env4");
    }

    /// <summary>
    /// Verifies that <see cref="FilterEnvironmentByDefault"/> returns an empty result
    /// when no environments are marked as default.
    /// </summary>
    [Fact]
    public void GetExpression_ReturnsEmpty_WhenNoDefault()
    {
        // Arrange
        var environments = new List<TestEnvironment>
        {
            new TestEnvironment { Name = "env1", Default = false },
            new TestEnvironment { Name = "env2", Default = false }
        };

        var spec = new FilterEnvironmentByDefault();
        var expr = spec.Expression.Compile();

        // Act
        var result = environments.Where(expr).ToList();

        // Assert
        Assert.Empty(result);
    }
}