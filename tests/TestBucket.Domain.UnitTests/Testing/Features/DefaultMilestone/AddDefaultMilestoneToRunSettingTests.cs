using System.Security.Claims;

using NSubstitute;

using TestBucket.Contracts.Localization;
using TestBucket.Domain.Identity;
using TestBucket.Domain.Settings;
using TestBucket.Domain.Settings.Models;
using TestBucket.Domain.Testing.Features.DefaultMilestones;
using TestBucket.Domain.Testing.Settings;

namespace TestBucket.Domain.UnitTests.Testing.Features.DefaultMilestone;

/// <summary>
/// Unit tests for <see cref="AddDefaultMilestoneToRunSetting"/>.
/// Validates reading and writing the AddDefaultMilestoneFieldToImportedTestRuns setting.
/// </summary>
[EnrichedTest]
[UnitTest]
[FunctionalTest]
[Component("Testing")]
[Feature("Default Milestone")]
public class AddDefaultMilestoneToRunSettingTests
{
    /// <summary>
    /// Creates a <see cref="SettingContext"/> for testing with optional tenant and project IDs.
    /// </summary>
    /// <param name="tenantId">The tenant identifier. Defaults to "tenant-1".</param>
    /// <param name="projectId">The project identifier. Defaults to 1.</param>
    /// <returns>A new <see cref="SettingContext"/> instance.</returns>
    private static SettingContext CreateContext(string tenantId = "tenant-1", long? projectId = 1)
    {
        var principal = Impersonation.Impersonate(tenantId, projectId);
        return new SettingContext
        {
            Principal = principal,
            TenantId = tenantId,
            ProjectId = projectId
        };
    }

    /// <summary>
    /// Verifies that <see cref="AddDefaultMilestoneToRunSetting.ReadAsync"/> returns true when the setting is enabled.
    /// </summary>
    [Fact]
    public async Task ReadAsync_Returns_True_When_Setting_Enabled()
    {
        // Arrange
        var settingsProvider = Substitute.For<ISettingsProvider>();
        var loc = Substitute.For<IAppLocalization>();

        settingsProvider
            .GetDomainSettingsAsync<ImportTestsProjectSettings>("tenant-1", 1)
            .Returns(new ImportTestsProjectSettings { AddDefaultMilestoneFieldToImportedTestRuns = true });

        var setting = new AddDefaultMilestoneToRunSetting(settingsProvider, loc);
        var context = CreateContext();

        // Act
        var value = await setting.ReadAsync(context);

        // Assert
        Assert.True(value.BooleanValue);
    }

    /// <summary>
    /// Verifies that <see cref="AddDefaultMilestoneToRunSetting.ReadAsync"/> returns false when the setting is disabled.
    /// </summary>
    [Fact]
    public async Task ReadAsync_Returns_False_When_Setting_Disabled()
    {
        // Arrange
        var settingsProvider = Substitute.For<ISettingsProvider>();
        var loc = Substitute.For<IAppLocalization>();

        settingsProvider
            .GetDomainSettingsAsync<ImportTestsProjectSettings>("tenant-1", 1)
            .Returns(new ImportTestsProjectSettings { AddDefaultMilestoneFieldToImportedTestRuns = false });

        var setting = new AddDefaultMilestoneToRunSetting(settingsProvider, loc);
        var context = CreateContext();

        // Act
        var value = await setting.ReadAsync(context);

        // Assert
        Assert.False(value.BooleanValue);
    }

    /// <summary>
    /// Verifies that <see cref="AddDefaultMilestoneToRunSetting.WriteAsync"/> saves a true value to the setting.
    /// </summary>
    [Fact]
    public async Task WriteAsync_Saves_True_Value()
    {
        // Arrange
        var settingsProvider = Substitute.For<ISettingsProvider>();
        var loc = Substitute.For<IAppLocalization>();

        var preferences = new ImportTestsProjectSettings { AddDefaultMilestoneFieldToImportedTestRuns = false };
        settingsProvider
            .GetDomainSettingsAsync<ImportTestsProjectSettings>("tenant-1", 1)
            .Returns(preferences);

        var setting = new AddDefaultMilestoneToRunSetting(settingsProvider, loc);
        var context = CreateContext();

        var value = new FieldValue { BooleanValue = true, FieldDefinitionId = 0 };

        // Act
        await setting.WriteAsync(context, value);

        // Assert
        Assert.True(preferences.AddDefaultMilestoneFieldToImportedTestRuns);
        await settingsProvider.Received(1)
            .SaveDomainSettingsAsync("tenant-1", 1, preferences);
    }

    /// <summary>
    /// Verifies that <see cref="AddDefaultMilestoneToRunSetting.WriteAsync"/> does nothing when <see cref="FieldValue.BooleanValue"/> is null.
    /// </summary>
    [Fact]
    public async Task WriteAsync_DoesNothing_When_BooleanValue_IsNull()
    {
        // Arrange
        var settingsProvider = Substitute.For<ISettingsProvider>();
        var loc = Substitute.For<IAppLocalization>();

        var setting = new AddDefaultMilestoneToRunSetting(settingsProvider, loc);
        var context = CreateContext();

        var value = new FieldValue { BooleanValue = null, FieldDefinitionId = 0 };

        // Act
        await setting.WriteAsync(context, value);

        // Assert
        await settingsProvider.DidNotReceiveWithAnyArgs().SaveDomainSettingsAsync<ImportTestsProjectSettings>(default!, default, default!);
    }
}