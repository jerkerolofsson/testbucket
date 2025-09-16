using TestBucket.Domain.Appearance.Models;

namespace TestBucket.Components.Appearance.Controls;

public partial class ThemePicker
{
    [Parameter] public string? Theme { get; set; }
    [Parameter] public EventCallback<string> ThemeChanged { get; set; }

    private IReadOnlyList<TestBucketBaseTheme> _themes = [];

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (_themes.Count == 0)
        {
            _themes = await themeManager.GetThemesAsync();
        }
    }

    private async Task OnThemeClicked(TestBucketBaseTheme theme)
    {
        var themeName = theme.ToString();
        await ThemeChanged.InvokeAsync(themeName);
    }

    private MarkupString GetSurface(ColorScheme scheme)
    {
        return new MarkupString($"""
            <div class="surface cell" style="background-color:{scheme.Base.Surface}">
                <div class="color" style="background-color:{scheme.Base.Primary}"></div>
                <div class="color" style="background-color:{scheme.Base.Secondary}"></div>
                <div class="color" style="background-color:{scheme.Base.Tertiary}"></div>
            </div>
            """);
    }

    private MarkupString GetColorSchemePreview(ColorScheme scheme)
    {
        return new MarkupString($"""
            <div class="color-scheme cell" style="background-color:{scheme.Base.Background}">
                <div class="color" style="background-color:{scheme.Base.Primary}"></div>
                <div class="color" style="background-color:{scheme.Base.Secondary}"></div>
                <div class="color" style="background-color:{scheme.Base.Tertiary}"></div>
            </div>
            """);
    }
}

