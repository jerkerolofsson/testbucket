using MudBlazor;
using MudBlazor.Utilities;

using TestBucket.Components.Account;

namespace TestBucket.Components.Shared.Themeing;

internal class ThemingService
{
    private readonly UserPreferencesService _userPreferencesService;
    private bool? _isDarkMode = null;

    public MudTheme Theme => _theme;
    public ThemingService(UserPreferencesService userPreferencesService)
    {
        _userPreferencesService = userPreferencesService;
    }

    public async Task SetDarkModeAsync(bool darkMode)
    {
        _isDarkMode = darkMode;

        var userPreferences = await _userPreferencesService.GetUserPreferencesAsync();
        if (userPreferences is not null)
        {
            userPreferences.DarkMode = darkMode;
            await _userPreferencesService.SaveUserPreferencesAsync(userPreferences);
        }
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
            var userPreferences = await _userPreferencesService.GetUserPreferencesAsync();
            if (userPreferences is not null)
            {
                _isDarkMode = userPreferences.DarkMode;
            }
        }
        return _isDarkMode.Value;
    }

    private readonly MudTheme _theme = new MudTheme()
    {
        PaletteLight = new PaletteLight()
        {
            Dark = MudColor.Parse("#d0d0d0")
        },
        PaletteDark = new PaletteDark()
        {
            Dark = MudColor.Parse("#202020"),
        },
        LayoutProperties = new LayoutProperties()
        {
            DrawerWidthLeft = "260px",
            DrawerWidthRight = "300px"
        },
    };
}
