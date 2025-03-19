using MudBlazor;
using MudBlazor.Utilities;

using TestBucket.Components.Account;
using TestBucket.Domain.Identity;

namespace TestBucket.Components.Shared.Themeing;

internal class ThemingService : TenantBaseService, IDisposable
{
    private readonly IUserPreferencesManager _userPreferencesService;
    private bool? _isDarkMode = null;
    private bool? _highContrast = null;
    private string? _themeName = null;

    /// <summary>
    /// MudTheme
    /// </summary>
    public MudTheme Theme => GetTheme();

    public bool IsDarkMode => _isDarkMode is null ? false : _isDarkMode.Value;
    public bool HighContrast => _highContrast is null ? false : _highContrast.Value;

    /// <summary>
    /// Palette for the Field control
    /// </summary>
    public AppPalette FieldPaletteDark => _fieldPaletteDark;
    public AppPalette FieldPaletteLight => _fieldPaletteLight;

    public ThemingService(
        AuthenticationStateProvider authenticationStateProvider, 
        IUserPreferencesManager userPreferencesService) : 
        base(authenticationStateProvider)
    {
        _userPreferencesService = userPreferencesService;
        _userPreferencesService.UserPreferencesChanged += OnUserPreferencesChanged;
    }

    private void OnUserPreferencesChanged(object? sender, UserPreferences preferences)
    {
        _themeName = preferences.Theme;
        _isDarkMode = preferences.DarkMode;
        _highContrast = preferences.IncreasedContrast;
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
        _userPreferencesService.UserPreferencesChanged -= OnUserPreferencesChanged;
    }

    private readonly AppPalette _fieldPaletteDark = new AppPalette
    {
        Background = new MudColor(25, 26, 36, 255),
        Border = new MudColor(35, 36, 46, 255),
    };

    private readonly AppPalette _fieldPaletteLight = new AppPalette
    {
        Background = new MudColor(255, 255, 255, 255),
        Border = new MudColor(240, 240, 240, 255),
    };

    private MudTheme GetTheme()
    {
        if(_themeName is not null)
        {
            switch(_themeName)
            {
                case "Yellow":
                    return _yellowTheme;
                case "Hotpink":
                    return _hotPinkTheme;
            }
        }
        return _theme;
    }

    private readonly MudTheme _hotPinkTheme = new MudTheme()
    {
        PaletteLight = new PaletteLight()
        {
            Dark = MudColor.Parse("#d0d0d0"),
            Surface = new MudColor(245, 245, 245, 255),

            Primary = new MudColor(255, 105, 180, 255),
            Secondary = new MudColor(255, 105, 180, 255),
            Tertiary = new MudColor(255, 105, 180, 255),
        },
        PaletteDark = new PaletteDark()
        {
            Dark = MudColor.Parse("#202020"),
            Background = new MudColor(20, 21, 31, 255),
            Surface = new MudColor(30, 31, 41, 255),

            Primary = new MudColor(255, 105, 180, 255),
            Secondary = new MudColor(255, 105, 180, 255),
            Tertiary = new MudColor(255, 105, 180, 255),
        },
        LayoutProperties = new LayoutProperties()
        {
            DrawerWidthLeft = "260px",
            DrawerWidthRight = "300px"
        },
    };
    private readonly MudTheme _yellowTheme = new MudTheme()
    {
        PaletteLight = new PaletteLight()
        {
            Dark = MudColor.Parse("#d0d0d0"),
            Surface = new MudColor(245, 245, 245, 255),
            
            Primary = new MudColor(205, 190, 0, 255),
            Secondary = new MudColor(205, 180, 0, 255),
            Tertiary = new MudColor(205, 180, 0, 255),
        },
        PaletteDark = new PaletteDark()
        {
            Dark = MudColor.Parse("#202020"),
            Background = new MudColor(20, 21, 31, 255),
            Surface = new MudColor(30, 31, 41, 255),

            Primary = new MudColor(255, 210, 0, 255),
            Secondary = new MudColor(255, 200, 0, 255),
            Tertiary = new MudColor(255, 190, 0, 255),
        },
        LayoutProperties = new LayoutProperties()
        {
            DrawerWidthLeft = "260px",
            DrawerWidthRight = "300px"
        },
    };
    private readonly MudTheme _theme = new MudTheme()
    {
        PaletteLight = new PaletteLight()
        {
            Dark = MudColor.Parse("#d0d0d0"),
            Surface = new MudColor(245, 245, 245, 255),
        },
        PaletteDark = new PaletteDark()
        {
            Dark = MudColor.Parse("#202020"),
            Background = new MudColor(20,21,31,255),
            Surface = new MudColor(30, 31, 41, 255),
        },
        LayoutProperties = new LayoutProperties()
        {
            DrawerWidthLeft = "260px",
            DrawerWidthRight = "300px"
        },
    };
}
