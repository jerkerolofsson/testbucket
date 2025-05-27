using Microsoft.Extensions.Localization;

using TestBucket.Domain.Commands;
using TestBucket.Domain.Keyboard;
using TestBucket.Localization;

namespace TestBucket.Components.Settings.Commands;

public class OpenAiSettingsCommand  : ICommand
{
    private readonly IStringLocalizer _loc;
    private readonly AppNavigationManager _appNavigationManager;

    public int SortOrder => 20;

    public string? Folder => null;

    public OpenAiSettingsCommand(
        IStringLocalizer<SettingStrings> loc,
        AppNavigationManager appNavigationManager)
    {
        _loc = loc;
        _appNavigationManager = appNavigationManager;
    }

    public bool Enabled => true;
    public string Id => "open-ai-settings";
    public string Name => _loc["open-ai-settings"];
    public string Description => "";
    public string? Icon => Icons.Material.Outlined.Rocket;
    public string[] ContextMenuTypes => [];
    public KeyboardBinding? DefaultKeyboardBinding => null;

    public PermissionEntityType? PermissionEntityType => Domain.Identity.Permissions.PermissionEntityType.Tenant;
    public PermissionLevel? RequiredLevel => PermissionLevel.Delete;

    public ValueTask ExecuteAsync(ClaimsPrincipal principal)
    {
        if (!principal.IsInRole(TestBucket.Domain.Identity.Roles.SUPERADMIN))
        {
            return ValueTask.CompletedTask;
        }

        _appNavigationManager.NavigateTo(_appNavigationManager.GetAISettingsUrl());
        return ValueTask.CompletedTask;
    }
}
