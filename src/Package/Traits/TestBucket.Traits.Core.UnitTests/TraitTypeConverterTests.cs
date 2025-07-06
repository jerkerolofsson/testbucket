using TestBucket.Traits.Xunit;

namespace TestBucket.Traits.Core.UnitTests;

/// <summary>
/// Contains unit tests for the <see cref="TraitTypeConverter"/> class, verifying correct conversions between <see cref="TraitType"/> values and their string representations.
/// </summary>
[UnitTest]
[Feature("Import Test Results")]
[Component("Traits")]
[EnrichedTest]
[FunctionalTest]
public class TraitTypeConverterTests
{
    /// <summary>
    /// Verifies that converting from known <see cref="TraitType"/> values returns the correct string representation.
    /// </summary>
    /// <param name="expectedTraitName">The expected trait name string.</param>
    /// <param name="type">The <see cref="TraitType"/> to convert.</param>
    [Theory]
    [InlineData(TargetTraitNames.Feature, TraitType.Feature)]
    [InlineData(TargetTraitNames.Branch, TraitType.Branch)]
    [InlineData(TargetTraitNames.Commit, TraitType.Commit)]
    [InlineData(TargetTraitNames.Component, TraitType.Component)]
    [InlineData(TargetTraitNames.Release, TraitType.Release)]
    [InlineData(TestTraitNames.TestCategory, TraitType.TestCategory)]
    [InlineData(TargetTraitNames.Milestone, TraitType.Milestone)]
    public void ConvertFromKnownType_IsValid(string expectedTraitName, TraitType type)
    {
        bool result = TraitTypeConverter.TryConvert(type, out var traitName);
        Assert.True(result, "Failed to convert, TryConvert returned false");
        Assert.NotNull(traitName);
        Assert.Equal(expectedTraitName, traitName);
    }

    /// <summary>
    /// Verifies that attempting to convert the <see cref="TraitType.Custom"/> value fails as expected.
    /// </summary>
    [Fact]
    public void TryConvertFromCustom_Fails()
    {
        bool result = TraitTypeConverter.TryConvert(TraitType.Custom, out var traitName);
        Assert.False(result, "Expected conversion to fail");
        Assert.Null(traitName);
    }

    /// <summary>
    /// Verifies that attempting to convert a null <see cref="TraitType"/> fails as expected.
    /// </summary>
    [Fact]
    public void TryConvertWithTraitNull_Fails()
    {
        TraitType? type = null;
        bool result = TraitTypeConverter.TryConvert(type, out var traitName);
        Assert.False(result, "Expected conversion to fail");
        Assert.Null(traitName);
    }

    /// <summary>
    /// Verifies that attempting to convert a null string fails as expected.
    /// </summary>
    [Fact]
    public void TryConvertWithTStringNull_Fails()
    {
        string? name = null;
        bool result = TraitTypeConverter.TryConvert(name, out var traitType);
        Assert.False(result, "Expected conversion to fail");
        Assert.Null(traitType);
    }

    /// <summary>
    /// Verifies that converting from known trait name strings returns the correct <see cref="TraitType"/> value.
    /// </summary>
    /// <param name="traitName">The trait name string to convert.</param>
    /// <param name="expectedType">The expected <see cref="TraitType"/> value.</param>
    [Theory]
    [InlineData("feature", TraitType.Feature)]
    [InlineData("commit", TraitType.Commit)]
    [InlineData("component", TraitType.Component)]
    [InlineData("release", TraitType.Release)]
    [InlineData("testcategory", TraitType.TestCategory)]
    [InlineData("milestone", TraitType.Milestone)]
    public void ConvertFromStringToKnownType_IsValid(string traitName, TraitType expectedType)
    {
        bool result = TraitTypeConverter.TryConvert(traitName, out var traitType);
        Assert.True(result, "Failed to convert, TryConvert returned false");
        Assert.NotNull(traitType);
        Assert.Equal(expectedType, traitType);
    }

    /// <summary>
    /// Verifies that converting from known trait name strings is case-insensitive.
    /// </summary>
    /// <param name="traitName">The trait name string to convert (with varying casing).</param>
    /// <param name="expectedType">The expected <see cref="TraitType"/> value.</param>
    [Theory]
    [InlineData("testcategory", TraitType.TestCategory)]
    [InlineData("TestCategory", TraitType.TestCategory)]
    [InlineData("testcAtegory", TraitType.TestCategory)]
    [InlineData("TestCATegory", TraitType.TestCategory)]
    public void ConvertFromStringToKnownType_IsNotCaseSensitive(string traitName, TraitType expectedType)
    {
        bool result = TraitTypeConverter.TryConvert(traitName, out var traitType);
        Assert.True(result, "Failed to convert, TryConvert returned false");
        Assert.NotNull(traitType);
        Assert.Equal(expectedType, traitType);
    }

    /// <summary>
    /// Verifies that converting from an unknown trait name string fails and returns null.
    /// </summary>
    [Fact]
    public void ConvertFromStringToKnownType_WithUnknownType_IsNull()
    {
        var traitName = "UnknownTraitName";
        bool result = TraitTypeConverter.TryConvert(traitName, out var traitType);
        Assert.False(result, "Expected TryConvert to return false");
        Assert.Null(traitType);
    }
}