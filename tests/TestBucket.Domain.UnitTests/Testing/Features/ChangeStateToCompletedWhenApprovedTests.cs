using NSubstitute;

using TestBucket.Domain.Editor.Models;
using TestBucket.Domain.Identity;
using TestBucket.Domain.Settings;
using TestBucket.Domain.Settings.Models;
using TestBucket.Domain.Testing.TestCases.Features.ChangeStateToCompletedWhenApproved;
using TestBucket.Domain.Testing.TestCases.Features.ChangeStateToOngoingWhenEditingTests;

namespace TestBucket.Domain.UnitTests.Testing.Features;

/// <summary>
/// Unit tests for ChangeStateToCompletedWhenApprovedSetting feature.
/// </summary>
[UnitTest]
[FunctionalTest]
[Component("Testing")]
[Feature("Review")]
[EnrichedTest]
public class ChangeStateToCompletedWhenApprovedTests
{
    /// <summary>
    /// Creates a mocked ISettingsProvider with the specified setting value.
    /// </summary>
    /// <param name="enabled">Whether ChangeStateToCompletedWhenApproved is enabled.</param>
    /// <returns>Mocked ISettingsProvider.</returns>
    private static ISettingsProvider CreateMockedSettingProvider(bool enabled)
    {
        var settingsProvider = Substitute.For<ISettingsProvider>();
        settingsProvider.GetDomainSettingsAsync<EditorSettings>(Arg.Any<string>(), Arg.Any<long>())
            .Returns(Task.FromResult<EditorSettings?>(new EditorSettings { ChangeStateToCompletedWhenApproved = enabled }));
        return settingsProvider;
    }

    /// <summary>
    /// Verifies ReadAsync returns true when the setting is enabled.
    /// </summary>
    [Fact]
    [CoveredRequirement("TB-REVIEW-003")]
    public async Task ReadAsync_ReturnsTrue_WhenSettingIsTrue()
    {
        ISettingsProvider settingsProvider = CreateMockedSettingProvider(true);
        var setting = new ChangeStateToCompletedWhenApprovedSetting(settingsProvider);
        var context = CreateContext();
        var result = await setting.ReadAsync(context);
        Assert.True(result.BooleanValue);
    }

    /// <summary>
    /// Verifies ReadAsync returns false when the setting is disabled.
    /// </summary>
    [Fact]
    [CoveredRequirement("TB-REVIEW-003")]
    public async Task ReadAsync_ReturnsFalse_WhenSettingIsFalse()
    {
        ISettingsProvider settingsProvider = CreateMockedSettingProvider(false); 
        var setting = new ChangeStateToCompletedWhenApprovedSetting(settingsProvider);
        var context = CreateContext();
        var result = await setting.ReadAsync(context);
        Assert.False(result.BooleanValue);
    }

    /// <summary>
    /// Verifies WriteAsync updates the setting when the value changes.
    /// </summary>
    [Fact]
    [CoveredRequirement("TB-REVIEW-003")]
    public async Task WriteAsync_UpdatesSetting_WhenValueChanges()
    {
        var settings = new EditorSettings { ChangeStateToCompletedWhenApproved = false };
        var settingsProvider = Substitute.For<ISettingsProvider>();
        settingsProvider.GetDomainSettingsAsync<EditorSettings>(Arg.Any<string>(), Arg.Any<long>())
            .Returns(Task.FromResult<EditorSettings?>(settings));

        var setting = new ChangeStateToCompletedWhenApprovedSetting(settingsProvider);
        var context = CreateContext();
        var value = new FieldValue { BooleanValue = true, FieldDefinitionId = 1 };
        await setting.WriteAsync(context, value);
        Assert.True(settings.ChangeStateToCompletedWhenApproved);
        await settingsProvider.Received(1).SaveDomainSettingsAsync(Arg.Any<string>(), Arg.Any<long>(), settings);
    }

    /// <summary>
    /// Verifies WriteAsync does not update the setting when the value is unchanged.
    /// </summary>
    [Fact]
    [CoveredRequirement("TB-REVIEW-003")]
    public async Task WriteAsync_DoesNotUpdate_WhenValueIsSame()
    {
        var settings = new EditorSettings { ChangeStateToCompletedWhenApproved = true };
        var settingsProvider = Substitute.For<ISettingsProvider>();
        settingsProvider.GetDomainSettingsAsync<EditorSettings>(Arg.Any<string>(), Arg.Any<long>())
            .Returns(Task.FromResult<EditorSettings?>(settings));
        var setting = new ChangeStateToCompletedWhenApprovedSetting(settingsProvider);
        var context = CreateContext();
        var value = new FieldValue { BooleanValue = true, FieldDefinitionId = 1 };
        await setting.WriteAsync(context, value);
        await settingsProvider.DidNotReceive().SaveDomainSettingsAsync(Arg.Any<string>(), Arg.Any<long>(), settings);
    }

    /// <summary>
    /// Creates a SettingContext for testing purposes.
    /// </summary>
    /// <returns>SettingContext instance.</returns>
    private static SettingContext CreateContext()
    {
        // Substitute for permission checks and context
        var principal = Impersonation.Impersonate("tenant-1");
        var context = new SettingContext() { Principal = principal, TenantId = "tenant-1", ProjectId = 1 };
        return context;
    }
}
