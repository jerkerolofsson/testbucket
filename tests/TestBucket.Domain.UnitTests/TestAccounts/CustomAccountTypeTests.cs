using Microsoft.Extensions.Localization;

using NSubstitute;

using TestBucket.Domain.TestAccounts.AccountTypes;

using Xunit;

namespace TestBucket.Domain.UnitTests.TestAccounts;

/// <summary>
/// Unit tests for verifying the behavior of custom account types.
/// </summary>
[UnitTest]
[EnrichedTest]
[FunctionalTest]
[Component("Test Accounts")]
public class CustomAccountTypeTests
{
    /// <summary>
    /// Tests that the <see cref="WechatAcccount"/> returns the correct type and field definitions.
    /// </summary>
    [Fact]
    public void WechatAccount_ShouldReturnCorrectTypeAndFields()
    {
        // Arrange
        var wechatAccount = new WechatAcccount();
        var localizerMock = Substitute.For<IStringLocalizer>();

        // Act
        var type = wechatAccount.Type;
        var fields = wechatAccount.GetFieldDefinitionsForAccount(localizerMock, "", "");
                    
        // Assert
        Assert.Equal("wechat", type);
        Assert.Collection(fields,
            field => Assert.Equal("phonenumber", field.Name),
            field => Assert.Equal("password", field.Name));
    }

    /// <summary>
    /// Tests that the <see cref="GoogleAccount"/> returns the correct type and field definitions.
    /// </summary>
    [Fact]
    public void GoogleAccount_ShouldReturnCorrectTypeAndFields()
    {
        // Arrange
        var googleAccount = new GoogleAccount();
        var localizerMock = Substitute.For<IStringLocalizer>();

        // Act
        var type = googleAccount.Type;
        var fields = googleAccount.GetFieldDefinitionsForAccount(localizerMock, "", "");

        // Assert
        Assert.Equal("google", type);
        Assert.Collection(fields,
            field => Assert.Equal("email", field.Name),
            field => Assert.Equal("password", field.Name));
    }

    /// <summary>
    /// Tests that the <see cref="FacebookAccount"/> returns the correct type and field definitions.
    /// </summary>
    [Fact]
    public void FacebookAccount_ShouldReturnCorrectTypeAndFields()
    {
        // Arrange
        var facebookAccount = new FacebookAccount();
        var localizerMock = Substitute.For<IStringLocalizer>();

        // Act
        var type = facebookAccount.Type;
        var fields = facebookAccount.GetFieldDefinitionsForAccount(localizerMock, "", "");

        // Assert
        Assert.Equal("facebook", type);
        Assert.Collection(fields,
            field => Assert.Equal("email", field.Name),
            field => Assert.Equal("phonenumber", field.Name),
            field => Assert.Equal("password", field.Name));
    }

    /// <summary>
    /// Tests that the <see cref="OutlookAccount"/> returns the correct type and field definitions.
    /// </summary>
    [Fact]
    public void OutlookAccount_ShouldReturnCorrectTypeAndFields()
    {
        // Arrange
        var outlookAccount = new OutlookAccount();
        var localizerMock = Substitute.For<IStringLocalizer>();

        // Act
        var type = outlookAccount.Type;
        var fields = outlookAccount.GetFieldDefinitionsForAccount(localizerMock, "", "");

        // Assert
        Assert.Equal("outlook", type);
        Assert.Collection(fields,
            field => Assert.Equal("email", field.Name),
            field => Assert.Equal("password", field.Name));
    }

    /// <summary>
    /// Tests that the <see cref="OutlookAccount"/> returns the correct subtypes.
    /// </summary>
    [Fact]
    public void OutlookAccount_ShouldReturnCorrectSubTypes()
    {
        // Arrange
        var outlookAccount = new OutlookAccount();

        // Act
        var subTypes = outlookAccount.SubTypes;

        // Assert
        Assert.NotNull(subTypes);
        Assert.Empty(subTypes);
    }

    /// <summary>
    /// Tests that the <see cref="FacebookAccount"/> returns the correct subtypes.
    /// </summary>
    [Fact]
    public void FacebookAccount_ShouldReturnCorrectSubTypes()
    {
        // Arrange
        var facebookAccount = new FacebookAccount();

        // Act
        var subTypes = facebookAccount.SubTypes;

        // Assert
        Assert.NotNull(subTypes);
        Assert.Empty(subTypes);
    }

    /// <summary>
    /// Tests that the <see cref="GoogleAccount"/> returns the correct subtypes.
    /// </summary>
    [Fact]
    public void GoogleAccount_ShouldReturnCorrectSubTypes()
    {
        // Arrange
        var googleAccount = new GoogleAccount();

        // Act
        var subTypes = googleAccount.SubTypes;

        // Assert
        Assert.NotNull(subTypes);
        Assert.Empty(subTypes);
    }
}