
using TestBucket.Domain.Editor.Models;
using TestBucket.Domain.Settings;
using TestBucket.Domain.Shared;

namespace TestBucket.Components.Settings.Controllers;

internal class SettingsController : TenantBaseService
{
    private readonly ISettingsProvider _settingsProvider;
    private readonly AppNavigationManager _appNavigationManager;
    public SettingsController(AuthenticationStateProvider authenticationStateProvider, ISettingsProvider settingsProvider, AppNavigationManager appNavigationManager) : base(authenticationStateProvider)
    {
        _settingsProvider = settingsProvider;
        _appNavigationManager = appNavigationManager;
    }

    public async Task<EditorSettings> GetEditorSettingsAsync()
    {
        var projectId = _appNavigationManager.State.SelectedProject?.Id;
        if(projectId is null)
        {
            return new EditorSettings();
        }

        var principal = await GetUserClaimsPrincipalAsync();
        return (await _settingsProvider.GetDomainSettingsAsync<EditorSettings>(principal.GetTenantIdOrThrow(), projectId.Value)) ?? new EditorSettings();
    }
}
