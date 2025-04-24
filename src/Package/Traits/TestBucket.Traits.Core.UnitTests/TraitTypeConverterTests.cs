
using TestBucket.Traits.Xunit;

namespace TestBucket.Traits.Core.UnitTests;

[UnitTest]
[Feature("Traits")]
[Component("TraitTypeConverter")]
public class TraitTypeConverterTests
{
    [Theory]
    [InlineData("feature", TraitType.Feature)]
    [InlineData("commit", TraitType.Commit)]
    [InlineData("component", TraitType.Component)]
    [InlineData("release", TraitType.Release)]
    [InlineData("testcategory", TraitType.TestCategory)]
    [InlineData("milestone", TraitType.Milestone)]
    [TestDescription("Verifies that converting from known trait strings, the correct enum value is returned")]
    public void ConvertFromStringToKnownType_IsValid(string traitName, TraitType expectedType)
    {
        bool result = TraitTypeConverter.TryConvert(traitName, out var traitType);
        Assert.True(result, "Failed to convert, TryConvert returned false");
        Assert.NotNull(traitType);
        Assert.Equal(expectedType, traitType);
    }

    [Theory]
    [InlineData("testcategory", TraitType.TestCategory)]
    [InlineData("TestCategory", TraitType.TestCategory)]
    [InlineData("testcAtegory", TraitType.TestCategory)]
    [InlineData("TestCATegory", TraitType.TestCategory)]
    [TestDescription("Verifies that converting from known trait strings, the conversion is case-insensitive")]
    public void ConvertFromStringToKnownType_IsNotCaseSensitive(string traitName, TraitType expectedType)
    {
        bool result = TraitTypeConverter.TryConvert(traitName, out var traitType);
        Assert.True(result, "Failed to convert, TryConvert returned false");
        Assert.NotNull(traitType);
        Assert.Equal(expectedType, traitType);
    }
}
