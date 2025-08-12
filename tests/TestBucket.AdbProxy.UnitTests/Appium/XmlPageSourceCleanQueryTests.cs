using TestBucket.AdbProxy.Appium.PageSources;
using TestBucket.Traits.Xunit;

namespace TestBucket.AdbProxy.UnitTests.Appium;

[FunctionalTest]
[EnrichedTest]
[UnitTest]
[Component("Appium")]
[Feature("Test Resources")]
public class XmlPageSourceCleanQueryTests
{
    [InlineData("ok button", "ok")]
    [InlineData("enabled toggle", "enabled")]
    [InlineData("enabled switch", "enabled")]
    [InlineData("name text box", "name")]
    [InlineData("email field", "email")]
    [Theory]
    public void CleanQuery_CleanedCorrectly(string input, string expectedResult)
    {
        // Arrange
        // Act
        var result = XmlPageSourceExtensions.CleanQuery(input);

        // Assert
        Assert.Equal(expectedResult, result);
    }
}
