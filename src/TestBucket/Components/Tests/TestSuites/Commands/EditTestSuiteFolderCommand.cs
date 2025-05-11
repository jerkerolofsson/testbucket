using Microsoft.Extensions.Localization;
using TestBucket.Components.Tests.Services;
using TestBucket.Domain.Commands;
using TestBucket.Domain.Keyboard;
using TestBucket.Localization;

namespace TestBucket.Components.Tests.TestSuites.Commands;

internal class EditTestSuiteFolderCommand : ICommand
{
    private readonly AppNavigationManager _appNavigationManager;
    private readonly TestBrowser _browser;
    private readonly IStringLocalizer<SharedStrings> _loc;
    public EditTestSuiteFolderCommand(AppNavigationManager appNavigationManager, TestBrowser browser, IStringLocalizer<SharedStrings> loc)
    {
        _appNavigationManager = appNavigationManager;
        _browser = browser;
        _loc = loc;
    }

    public int SortOrder => 80;
    public string? Folder => null;

    public PermissionEntityType? PermissionEntityType => Domain.Identity.Permissions.PermissionEntityType.TestSuite;
    public PermissionLevel? RequiredLevel => PermissionLevel.ReadWrite;
    public bool Enabled => _appNavigationManager.State.SelectedTestSuite is not null;
    public string Id => "edit-folder";
    public string Name => _loc["edit-folder"];
    public string Description => _loc["edut-folder-description"];
    public KeyboardBinding? DefaultKeyboardBinding => null;
    public string? Icon => Icons.Material.Filled.Edit;
    public string[] ContextMenuTypes => ["TestSuiteFolder"];

    public async ValueTask ExecuteAsync()
    {
        var folder = _appNavigationManager.State.SelectedTestSuiteFolder;
        if (folder is null)
        {
            return;
        }
        await _browser.CustomizeFolderAsync(folder);
    }
}
