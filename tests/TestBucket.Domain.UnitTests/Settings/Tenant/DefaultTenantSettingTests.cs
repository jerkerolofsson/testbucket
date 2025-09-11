using NSubstitute;

using TestBucket.Domain.Identity;
using TestBucket.Domain.Settings;
using TestBucket.Domain.Settings.Models;
using TestBucket.Domain.Settings.Server;

namespace TestBucket.Domain.UnitTests.Settings.Tenant;

/// <summary>
/// Contains unit tests for <see cref="DefaultTenantSetting"/> to verify reading and writing the default tenant value.
/// </summary>
[UnitTest]
[Component("Settings")]
[EnrichedTest]
public class DefaultTenantSettingTests
{
    /// <summary>
    /// Verifies that <see cref="DefaultTenantSetting.ReadAsync"/> returns the default tenant value from global settings.
    /// </summary>
    [Fact]
    [FunctionalTest]
    public async Task ReadAsync_ReturnsDefaultTenantValue()
    {
        // Arrange
        var principal = Impersonation.Impersonate("tenant-1");

        var settingsProvider = Substitute.For<ISettingsProvider>();
        var globalSettings = new GlobalSettings { DefaultTenant = "TenantA" };
        settingsProvider.LoadGlobalSettingsAsync().Returns(Task.FromResult(globalSettings));
        var setting = new DefaultTenantSetting(settingsProvider);
        var context = new SettingContext { Principal = principal, TenantId = "tenant-1" };

        // Act
        var result = await setting.ReadAsync(context);

        // Assert
        Assert.Equal("TenantA", result.StringValue);
        Assert.Equal(0, result.FieldDefinitionId);
    }

    /// <summary>
    /// Verifies that <see cref="DefaultTenantSetting.WriteAsync"/> updates the default tenant value in global settings.
    /// </summary>
    [Fact]
    [FunctionalTest]
    public async Task WriteAsync_UpdatesDefaultTenantValue()
    {
        // Arrange
        var principal = Impersonation.Impersonate("tenant-1");

        var settingsProvider = Substitute.For<ISettingsProvider>();
        var globalSettings = new GlobalSettings { DefaultTenant = "OldTenant" };
        settingsProvider.LoadGlobalSettingsAsync().Returns(Task.FromResult(globalSettings));
        var setting = new DefaultTenantSetting(settingsProvider);
        var context = new SettingContext { Principal = principal, TenantId = "tenant-1" };
        var value = new FieldValue { StringValue = "NewTenant", FieldDefinitionId = 1 };

        // Act
        await setting.WriteAsync(context, value);

        // Assert
        Assert.Equal("NewTenant", globalSettings.DefaultTenant);
        await settingsProvider.Received(1).SaveGlobalSettingsAsync(globalSettings);
    }

    /// <summary>
    /// Verifies that <see cref="DefaultTenantSetting.WriteAsync"/> does nothing when the string value is null.
    /// </summary>
    [Fact]
    [FunctionalTest]
    public async Task WriteAsync_DoesNothing_WhenStringValueIsNull()
    {
        // Arrange
        var principal = Impersonation.Impersonate("tenant-1");

        var settingsProvider = Substitute.For<ISettingsProvider>();
        var globalSettings = new GlobalSettings { DefaultTenant = "OldTenant" };
        settingsProvider.LoadGlobalSettingsAsync().Returns(Task.FromResult(globalSettings));
        var setting = new DefaultTenantSetting(settingsProvider);
        var context = new SettingContext { Principal = principal, TenantId = "tenant-1" };
        var value = new FieldValue { StringValue = null, FieldDefinitionId = 1 };

        // Act
        await setting.WriteAsync(context, value);

        // Assert
        await settingsProvider.DidNotReceive().SaveGlobalSettingsAsync(Arg.Any<GlobalSettings>());
        Assert.Equal("OldTenant", globalSettings.DefaultTenant);
    }
}