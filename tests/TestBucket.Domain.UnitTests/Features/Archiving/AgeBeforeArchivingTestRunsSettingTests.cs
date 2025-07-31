using System.Security.Claims;

using NSubstitute;

using TestBucket.Domain.Features.Archiving.Models;
using TestBucket.Domain.Features.Archiving.Settings;
using TestBucket.Domain.Identity;
using TestBucket.Domain.Settings;
using TestBucket.Domain.Settings.Models;

namespace TestBucket.Domain.UnitTests.Features.Archiving;

/// <summary>
/// Tests that verifies reading and writing the setting
/// </summary>
[FunctionalTest]
[EnrichedTest]
[UnitTest]
[Component("Testing")]
[Feature("Archiving")]
public class AgeBeforeArchivingTestRunsSettingTests
{
    private readonly ISettingsProvider _settingsProvider;
    private readonly AgeBeforeArchivingTestRunsSetting _setting;

    /// <summary>
    /// Creates mocks
    /// </summary>
    public AgeBeforeArchivingTestRunsSettingTests()
    {
        _settingsProvider = Substitute.For<ISettingsProvider>();
        _setting = new AgeBeforeArchivingTestRunsSetting(_settingsProvider);
    }

    /// <summary>
    /// Tests that <see cref="AgeBeforeArchivingTestRunsSetting.ReadAsync"/> returns the correct value.
    /// </summary>
    [Fact]
    public async Task ReadAsync_ReturnsCorrectValue()
    {
        // Arrange
        var context = new SettingContext
        {
            Principal = Impersonation.Impersonate("test-tenant"),
            TenantId = "test-tenant"
        };
        var archiveSettings = new ArchiveSettings { AgeBeforeArchivingTestRuns = TimeSpan.FromDays(30) };
        _settingsProvider.GetDomainSettingsAsync<ArchiveSettings>("test-tenant", Arg.Any<long?>()).Returns(archiveSettings);

        // Act
        var result = await _setting.ReadAsync(context);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(archiveSettings.AgeBeforeArchivingTestRuns, result.TimeSpanValue);
    }

    /// <summary>
    /// Tests that <see cref="AgeBeforeArchivingTestRunsSetting.WriteAsync"/> saves the correct value.
    /// </summary>
    [Fact]
    public async Task WriteAsync_SavesCorrectValue()
    {
        // Arrange
        var context = new SettingContext
        {
            Principal = Impersonation.Impersonate("test-tenant"),
            TenantId = "test-tenant"
        };
        var fieldValue = new FieldValue { TimeSpanValue = TimeSpan.FromDays(45), FieldDefinitionId = 1 };
        var archiveSettings = new ArchiveSettings();
        _settingsProvider.GetDomainSettingsAsync<ArchiveSettings>("test-tenant", Arg.Any<long?>()).Returns(archiveSettings);

        // Act
        await _setting.WriteAsync(context, fieldValue);

        // Assert
        Assert.Equal(fieldValue.TimeSpanValue, archiveSettings.AgeBeforeArchivingTestRuns);
        await _settingsProvider.Received(1).SaveDomainSettingsAsync("test-tenant", Arg.Any<long?>(), archiveSettings);
    }
}
