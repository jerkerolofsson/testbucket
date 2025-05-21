using MudBlazor;
using MudBlazor.Utilities;

using TestBucket.Components.Account;
using TestBucket.Components.Shared.Themeing.Models;
using TestBucket.Contracts.Appearance.Models;
using TestBucket.Domain.Appearance;
using TestBucket.Domain.Identity;

namespace TestBucket.Components.Shared.Themeing;

internal class ThemingService : TenantBaseService, IDisposable
{
    private readonly ITestBucketThemeManager _testBucketThemeManager;
    private readonly IUserPreferencesManager _userPreferencesService;
    private bool? _isDarkMode = null;
    private bool? _highContrast = null;
    private string? _themeName = null;

    /// <summary>
    /// MudTheme
    /// </summary>
    //public MudTheme Theme => GetTheme();

    public bool IsDarkMode => _isDarkMode is null ? false : _isDarkMode.Value;
    public bool HighContrast => _highContrast is null ? false : _highContrast.Value;

    public ThemingService(
        AuthenticationStateProvider authenticationStateProvider,
        IUserPreferencesManager userPreferencesService,
        ITestBucketThemeManager testBucketThemeManager) :
        base(authenticationStateProvider)
    {
        _userPreferencesService = userPreferencesService;
        //_userPreferencesService.UserPreferencesChanged += OnUserPreferencesChanged;
        _testBucketThemeManager = testBucketThemeManager;
    }

    private void OnUserPreferencesChanged(object? sender, UserPreferences preferences)
    {
        _themeName = preferences.Theme;
        _isDarkMode = preferences.DarkMode;
        _highContrast = preferences.IncreasedContrast;
    }

    public async Task<ApplicationState> GetApplicationStateAsync()
    {
        var principal = await GetUserClaimsPrincipalAsync();
        var userPreferences = await _userPreferencesService.LoadUserPreferencesAsync(principal);
        var theme = await _testBucketThemeManager.GetCurrentThemeAsync(principal);
        return new ApplicationState(principal)
        {
            IsDarkMode = userPreferences.DarkMode,
            Theme = theme
        };
    }

    public async Task SetDarkModeAsync(bool darkMode)
    {
        _isDarkMode = darkMode;
        var principal = await GetUserClaimsPrincipalAsync();

        var userPreferences = await _userPreferencesService.LoadUserPreferencesAsync(principal);
        if (userPreferences is not null)
        {
            userPreferences.DarkMode = darkMode;
            await _userPreferencesService.SaveUserPreferencesAsync(principal, userPreferences);
        }
    }

    /// <summary>
    /// Returns true if high contrast mode is enabled
    /// </summary>
    /// <returns></returns>
    public async Task<bool> IsHighContrastAsync()
    {
        if (_highContrast is null)
        {
            _highContrast = false;
            var principal = await GetUserClaimsPrincipalAsync();
            var userPreferences = await _userPreferencesService.LoadUserPreferencesAsync(principal);
            if (userPreferences is not null)
            {
                _isDarkMode = userPreferences.DarkMode;
                _themeName = userPreferences.Theme;
                _highContrast = userPreferences.IncreasedContrast;
            }
        }
        return _highContrast.Value;
    }

    /// <summary>
    /// Returns true if dark mode is enabled
    /// </summary>
    /// <returns></returns>
    public async Task<bool> IsDarkModeAsync()
    {
        if (_isDarkMode is null)
        {
            _isDarkMode = true;
            var principal = await GetUserClaimsPrincipalAsync();
            var userPreferences = await _userPreferencesService.LoadUserPreferencesAsync(principal);
            if (userPreferences is not null)
            {
                _isDarkMode = userPreferences.DarkMode;
                _themeName = userPreferences.Theme;
                _highContrast = userPreferences.IncreasedContrast;
            }
        }
        return _isDarkMode.Value;
    }

    public void Dispose()
    {
        //_userPreferencesService.UserPreferencesChanged -= OnUserPreferencesChanged;
    }
}
