
using TestBucket.Traits.Xunit;

namespace TestBucket.Traits.Core.UnitTests;

[UnitTest]
[Feature("Traits")]
[Component("TraitTypeConverter")]
public class TraitTypeConverterTests
{
    [Theory]
    [InlineData(TargetTraitNames.Feature, TraitType.Feature)]
    [InlineData(TargetTraitNames.Commit, TraitType.Commit)]
    [InlineData(TargetTraitNames.Component, TraitType.Component)]
    [InlineData(TargetTraitNames.Release, TraitType.Release)]
    [InlineData(TestTraitNames.TestCategory, TraitType.TestCategory)]
    [InlineData(TargetTraitNames.Milestone, TraitType.Milestone)]
    [TestDescription("Verifies that converting from known trait types, the correct string is returned")]
    public void ConvertFromKnownType_IsValid(string expectedTraitName, TraitType type)
    {
        bool result = TraitTypeConverter.TryConvert(type, out var traitName);
        Assert.True(result, "Failed to convert, TryConvert returned false");
        Assert.NotNull(traitName);
        Assert.Equal(expectedTraitName, traitName);
    }
    [Fact]
    public void TryConvertFromCustom_Fails()
    {
        bool result = TraitTypeConverter.TryConvert(TraitType.Custom, out var traitName);
        Assert.False(result, "Expected conversion to fail");
        Assert.Null(traitName);
    }
    [Fact]
    public void TryConvertWithTraitNull_Fails()
    {
        TraitType? type = null;
        bool result = TraitTypeConverter.TryConvert(type, out var traitName);
        Assert.False(result, "Expected conversion to fail");
        Assert.Null(traitName);
    }
    [Fact]
    public void TryConvertWithTStringNull_Fails()
    {
        string? name = null;
        bool result = TraitTypeConverter.TryConvert(name, out var traitType);
        Assert.False(result, "Expected conversion to fail");
        Assert.Null(traitType);
    }

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

    [Fact]
    public void ConvertFromStringToKnownType_WithUnknownType_IsNull()
    {
        var traitName = "UnknownTraitName";
        bool result = TraitTypeConverter.TryConvert(traitName, out var traitType);
        Assert.False(result, "Expected TryConvert to return false");
        Assert.Null(traitType);
    }
}
