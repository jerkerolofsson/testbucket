using TestBucket.Traits.Xunit;

namespace TestBucket.Traits.Core.UnitTests.Xunit;

/// <summary>
/// Unit tests for test trait attributes, verifying correct key and value assignment for various test category attributes.
/// </summary>
[Feature("Import Test Results")]
[Component("Traits")]
[EnrichedTest]
[FunctionalTest]
[UnitTest]
public class TestTraitAttributeTests
{
    /// <summary>
    /// Verifies that <see cref="TestCategoryAttribute"/> assigns the correct key and value for a custom category.
    /// </summary>
    [Fact]
    public void CustomCategoryTest()
    {
        var attribute = new TestCategoryAttribute("CustomCategory");
        Assert.Equal(TestTraitNames.TestCategory, attribute.Key);
        Assert.Equal("CustomCategory", attribute.Value);
    }

    /// <summary>
    /// Verifies that ApiTestAttribute assigns the correct key and value for API tests.
    /// </summary>
    [Fact]
    public void ApiTest()
    {
        var attribute = new ApiTestAttribute();
        Assert.Equal(TestTraitNames.TestCategory, attribute.Key);
        Assert.Equal("API", attribute.Value);
    }

    /// <summary>
    /// Verifies that <see cref="UnitTestAttribute"/> assigns the correct key and value for unit tests.
    /// </summary>
    [Fact]
    public void UnitTest()
    {
        var attribute = new UnitTestAttribute();
        Assert.Equal(TestTraitNames.TestCategory, attribute.Key);
        Assert.Equal("Unit", attribute.Value);
    }

    /// <summary>
    /// Verifies that <see cref="IntegrationTestAttribute"/> assigns the correct key and value for integration tests.
    /// </summary>
    [Fact]
    public void IntegrationTest()
    {
        var attribute = new IntegrationTestAttribute();
        Assert.Equal(TestTraitNames.TestCategory, attribute.Key);
        Assert.Equal("Integration", attribute.Value);
    }

    /// <summary>
    /// Verifies that <see cref="EndToEndTestAttribute"/> assigns the correct key and value for end-to-end tests.
    /// </summary>
    [Fact]
    public void End2EndTest()
    {
        var attribute = new EndToEndTestAttribute();
        Assert.Equal(TestTraitNames.TestCategory, attribute.Key);
        Assert.Equal("E2E", attribute.Value);
    }
}