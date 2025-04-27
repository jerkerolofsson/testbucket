using TestBucket.Components.Requirements.Services;
using TestBucket.Components.Shared;
using TestBucket.Components.Tests.Controls;
using TestBucket.Components.Tests.Services;
using TestBucket.Domain.Commands;
using TestBucket.Domain.Identity.Permissions;
using TestBucket.Domain.Keyboard;

namespace TestBucket.Components.Tests.Requirements.Commands;

internal class SyncWithActiveRequirementCommand : ICommand
{
    private readonly AppNavigationManager _appNavigationManager;
    private readonly RequirementBrowser _browser;

    public SyncWithActiveRequirementCommand(AppNavigationManager appNavigationManager, RequirementBrowser browser)
    {
        _appNavigationManager = appNavigationManager;
        _browser = browser;
    }

    public int SortOrder => 10;

    public string? Folder => null;

    public PermissionEntityType? PermissionEntityType => Domain.Identity.Permissions.PermissionEntityType.Requirement;
    public PermissionLevel? RequiredLevel => PermissionLevel.Read;
    public bool Enabled => _appNavigationManager.State.SelectedRequirement is not null;
    public string Id => "sync-with-active-req";
    public string Name => "Sync with active requirement";
    public string Description => "";
    public KeyboardBinding? DefaultKeyboardBinding => null;
    public string? Icon => Icons.Material.Filled.CompareArrows;
    public string[] ContextMenuTypes => ["Requirement"];

    public async ValueTask ExecuteAsync()
    {
        if (_appNavigationManager.State.SelectedRequirement is null)
        {
            return;
        }

        await _browser.SyncWithActiveDocumentAsync(_appNavigationManager.State.SelectedRequirement);
    }
}
