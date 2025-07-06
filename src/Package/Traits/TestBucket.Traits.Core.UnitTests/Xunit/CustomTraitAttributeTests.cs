using TestBucket.Traits.Xunit;

namespace TestBucket.Traits.Core.UnitTests.Xunit;

/// <summary>
/// Unit tests for custom trait attributes, verifying correct trait key/value handling and integration with test trait names.
/// </summary>
[UnitTest]
[Component("Traits")]
[Feature("Import Test Results")]
[EnrichedTest]
[FunctionalTest]
public class CustomTraitAttributeTests
{
    /// <summary>
    /// Verifies that <see cref="CustomTraitAttribute.GetTraits"/> returns a single trait when constructed with one key/value pair.
    /// </summary>
    [Fact]
    public void GetTraits_SingleItem()
    {
        var customTraitAttribute = new CustomTraitAttribute("KEY", "VALUE");

        var traits = customTraitAttribute.GetTraits();

        Assert.Single(traits);
    }

    /// <summary>
    /// Verifies that <see cref="TestIdAttribute"/> creates a trait with the correct name and value.
    /// </summary>
    [Fact]
    public void CreateTestIdTrait_WithCustomId_IdIsCorrect()
    {
        var customTraitAttribute = new TestIdAttribute("something");

        Assert.Equal(TestTraitNames.TestId, customTraitAttribute.Name);
        Assert.Equal("something", customTraitAttribute.Value);
    }

    /// <summary>
    /// Verifies that <see cref="CustomTraitAttribute.GetTraits"/> returns a single trait with the correct key and value.
    /// </summary>
    [Fact]
    public void GetTraits_FromTag()
    {
        var customTraitAttribute = new TagAttribute("something");

        var traits = customTraitAttribute.GetTraits();

        Assert.Single(traits);
        Assert.Equal(TestTraitNames.Tag, traits.First().Key);
        Assert.Equal("something", traits.First().Value);
    }
}