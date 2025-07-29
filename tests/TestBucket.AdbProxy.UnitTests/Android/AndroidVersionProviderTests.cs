using TestBucket.AdbProxy.Android;

using TestBucket.Traits.Xunit;

namespace TestBucket.AdbProxy.UnitTests.Android;

[EnrichedTest]
[UnitTest]
[Component("Android")]
[Feature("Test Resources")]
public class AndroidVersionProviderTests
{
    /// <summary>
    /// Verifies that version information can be retreived from a valid API level
    /// </summary>
    [Fact]
    public void FromApiLevel_ExistingApiLevel_ReturnsCorrectVersion()
    {
        // Arrange
        int apiLevel = 33; // Tiramisu

        // Act
        var result = AndroidVersionProvider.FromApiLevel(apiLevel);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Tiramisu", result.CodeName);
        Assert.Equal("Android 13", result.VersionName);
        Assert.Equal(33, result.ApiLevel);
    }

    /// <summary>
    /// Verifies that default value is returned from an unknown API level
    /// </summary>
    [Fact]
    public void FromApiLevel_NonExistingApiLevel_ReturnsDefaultVersion()
    {
        // Arrange
        int apiLevel = 99; // Non-existing

        // Act
        var result = AndroidVersionProvider.FromApiLevel(apiLevel);

        // Assert
        Assert.NotNull(result);
        Assert.Null(result.CodeName);
        Assert.Null(result.VersionName);
        Assert.Equal(99, result.ApiLevel);
    }
}
