using System.Security.Claims;

using NSubstitute;

using TestBucket.Contracts.Localization;
using TestBucket.Domain.Settings;
using TestBucket.Domain.Settings.Models;
using TestBucket.Domain.Identity.Models;


namespace TestBucket.Domain.UnitTests.Settings;

/// <summary>
/// Unit tests for <see cref="SettingsManager"/> behaviour: searching links and resolving settings by name
/// with regard to access levels and visibility.
/// </summary>
[UnitTest]
[Component("Settings")]
[Feature("Settings 1.0")]
[EnrichedTest]
[FunctionalTest]
public class SettingsManagerTests
{
    /// <summary>
    /// A test implementation of <see cref="SettingAdapter"/> for testing purposes.
    /// </summary>
    private class TestSettingAdapter : SettingAdapter
    {
        /// <summary>
        /// The last value written via <see cref="WriteAsync(SettingContext, FieldValue)"/>.
        /// </summary>
        public FieldValue? LastWrittenValue { get; private set; }

        /// <summary>
        /// Reads a dummy value for testing.
        /// </summary>
        /// <param name="context">The setting context (not used).</param>
        /// <returns>A completed task containing a <see cref="FieldValue"/> with sample data.</returns>
        public override Task<FieldValue> ReadAsync(SettingContext context)
        {
            // Return a dummy value for testing
            return Task.FromResult(new FieldValue { StringValue = "TestValue", FieldDefinitionId = 1 });
        }

        /// <summary>
        /// Stores the provided <see cref="FieldValue"/> to <see cref="LastWrittenValue"/>.
        /// </summary>
        /// <param name="context">The setting context (not used).</param>
        /// <param name="value">The value written by the caller.</param>
        /// <returns>A completed <see cref="Task"/>.</returns>
        public override Task WriteAsync(SettingContext context, FieldValue value)
        {
            LastWrittenValue = value;
            return Task.CompletedTask;
        }
    }

    /// <summary>
    /// Creates a minimal <see cref="SettingContext"/> for the tests.
    /// </summary>
    /// <returns>A <see cref="SettingContext"/> with an empty principal and a sample tenant id.</returns>
    private SettingContext CreateContext() =>
        new SettingContext { Principal = new ClaimsPrincipal(), TenantId = "tenant-1" };

    /// <summary>
    /// When the search phrase is null, all available links are returned.
    /// </summary>
    [Fact]
    public void SearchLinks_NullSearchPhrase_ReturnsAllLinks()
    {
        var loc = Substitute.For<IAppLocalization>();
        var links = new[]
        {
            new SettingsLink { Title = "LinkA", Keywords = "alpha", RelativeUrl = "/a" },
            new SettingsLink { Title = "LinkB", Keywords = "beta", RelativeUrl = "/b" },
            new SettingsLink { Title = "LinkC", Keywords = "gamma", RelativeUrl = "/c" }
        };

        var manager = new SettingsManager(Array.Empty<ISetting>(), links, loc);
        var result = manager.SearchLinks(CreateContext(), null);

        Assert.Equal(3, result.Count);
    }

    /// <summary>
    /// An empty search string is treated as a normal phrase (string.Contains(empty) matches all),
    /// therefore all links are returned.
    /// </summary>
    [Fact]
    public void SearchLinks_EmptyString_ReturnsAllLinks()
    {
        var loc = Substitute.For<IAppLocalization>();
        var links = new[]
        {
            new SettingsLink { Title = "LinkA", Keywords = "alpha", RelativeUrl = "/a" },
            new SettingsLink { Title = "LinkB", Keywords = "beta", RelativeUrl = "/b" }
        };

        var manager = new SettingsManager(Array.Empty<ISetting>(), links, loc);
        var result = manager.SearchLinks(CreateContext(), string.Empty);

        // Note: implementation treats empty string as a normal search phrase;
        // string.Contains(empty) is true for all strings, so all links are returned.
        Assert.Equal(2, result.Count);
    }

    /// <summary>
    /// Partial, case-insensitive matches against the link title should return the matching link.
    /// </summary>
    [Fact]
    public void SearchLinks_MatchesTitle_PartialCaseInsensitive()
    {
        var loc = Substitute.For<IAppLocalization>();
        var links = new[]
        {
            new SettingsLink { Title = "MySpecialLink", Keywords = "kw", RelativeUrl = "/s" },
            new SettingsLink { Title = "Other", Keywords = "kw", RelativeUrl = "/o" }
        };

        var manager = new SettingsManager(Array.Empty<ISetting>(), links, loc);
        var result = manager.SearchLinks(CreateContext(), "special");

        Assert.Single(result);
        Assert.Equal("MySpecialLink", result[0].Title);
    }

    /// <summary>
    /// Partial matches against the description should return the matching link.
    /// </summary>
    [Fact]
    public void SearchLinks_MatchesDescription_Partial()
    {
        var loc = Substitute.For<IAppLocalization>();
        var links = new[]
        {
            new SettingsLink { Title = "L1", Description = "The weather is fine today", Keywords = "x", RelativeUrl = "/1" },
            new SettingsLink { Title = "L2", Description = "No match here", Keywords = "y", RelativeUrl = "/2" }
        };

        var manager = new SettingsManager(Array.Empty<ISetting>(), links, loc);
        var result = manager.SearchLinks(CreateContext(), "weather");

        Assert.Single(result);
        Assert.Equal("L1", result[0].Title);
    }

    /// <summary>
    /// Keyword matching is case-insensitive and should match individual words in the keywords string.
    /// </summary>
    [Fact]
    public void SearchLinks_MatchesKeywords_CaseInsensitive()
    {
        var loc = Substitute.For<IAppLocalization>();
        var links = new[]
        {
            new SettingsLink { Title = "K1", Keywords = "one two three", RelativeUrl = "/k1" },
            new SettingsLink { Title = "K2", Keywords = "four five", RelativeUrl = "/k2" }
        };

        var manager = new SettingsManager(Array.Empty<ISetting>(), links, loc);
        var result = manager.SearchLinks(CreateContext(), "Two");

        Assert.Single(result);
        Assert.Equal("K1", result[0].Title);
    }

    /// <summary>
    /// When there are no matches for the search phrase the result should be empty.
    /// </summary>
    [Fact]
    public void SearchLinks_NoMatches_ReturnsEmptyList()
    {
        var loc = Substitute.For<IAppLocalization>();
        var links = new[]
        {
            new SettingsLink { Title = "A", Keywords = "alpha", RelativeUrl = "/a" },
            new SettingsLink { Title = "B", Description = "bravo", Keywords = "beta", RelativeUrl = "/b" }
        };

        var manager = new SettingsManager(Array.Empty<ISetting>(), links, loc);
        var result = manager.SearchLinks(CreateContext(), "zulu");

        Assert.Empty(result);
    }

    /// <summary>
    /// Verifies that a setting with a matching label is returned and the exact adapter instance is returned.
    /// </summary>
    [Fact]
    public void GetSettingByName_LabelMatches_ReturnsSetting()
    {
        var loc = Substitute.For<IAppLocalization>();

        var setting = new TestSettingAdapter();
        setting.Metadata.Label = "my.custom.setting";
        setting.Metadata.AccessLevel = AccessLevel.User;

        var manager = new SettingsManager(new ISetting[] { setting }, Array.Empty<SettingsLink>(), loc);

        var result = manager.GetSettingByName(CreateContext(), "my.custom.setting");

        Assert.NotNull(result);
        Assert.Equal("my.custom.setting", result.Metadata.Label);
        Assert.Same(setting, result);
    }

    /// <summary>
    /// Ensures access levels are respected: a setting requiring admin access is not visible to a normal user
    /// but is visible when the principal has the admin role.
    /// </summary>
    [Fact]
    public void GetSettingByName_AdminAccess_NotVisibleToNormalUser_ButVisibleToAdmin()
    {
        var loc = Substitute.For<IAppLocalization>();

        var adminSetting = new TestSettingAdapter();
        adminSetting.Metadata.Label = "admin.setting";
        adminSetting.Metadata.AccessLevel = AccessLevel.Admin;

        var manager = new SettingsManager(new ISetting[] { adminSetting }, Array.Empty<SettingsLink>(), loc);

        // Normal user context - should not see admin setting
        var userContext = CreateContext();
        var userResult = manager.GetSettingByName(userContext, "admin.setting");
        Assert.Null(userResult);

        // Admin context - should see admin setting
        var adminPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(ClaimTypes.Role, "ADMIN") }));
        var adminContext = new SettingContext { Principal = adminPrincipal, TenantId = "tenant-1" };
        var adminResult = manager.GetSettingByName(adminContext, "admin.setting");
        Assert.NotNull(adminResult);
        Assert.Same(adminSetting, adminResult);
    }
}