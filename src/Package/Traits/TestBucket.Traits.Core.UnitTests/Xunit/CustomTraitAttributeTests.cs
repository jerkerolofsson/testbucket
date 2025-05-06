using TestBucket.Traits.Xunit;

namespace TestBucket.Traits.Core.UnitTests.Xunit;

[UnitTest]
public class CustomTraitAttributeTests
{
    [Fact]
    public void GetTraits_SingleItem()
    {
        var customTraitAttribute = new CustomTraitAttribute("KEY", "VALUE");

        var traits = customTraitAttribute.GetTraits();

        Assert.Single(traits);
    }


    [Fact]
    public void CreateTestIdTrait_WithCustomId_IdIsCorrect()
    {
        var customTraitAttribute = new TestIdAttribute("something");

        Assert.Equal(TestTraitNames.TestId, customTraitAttribute.Name);
        Assert.Equal("something", customTraitAttribute.Value);
    }

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
