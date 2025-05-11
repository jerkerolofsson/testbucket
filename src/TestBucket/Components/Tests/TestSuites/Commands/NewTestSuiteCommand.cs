using Microsoft.Extensions.Localization;

using TestBucket.Components.Tests.Services;
using TestBucket.Domain.Commands;
using TestBucket.Domain.Identity.Permissions;
using TestBucket.Domain.Keyboard;
using TestBucket.Localization;

namespace TestBucket.Components.Tests.TestSuites.Commands;

internal class NewTestSuiteCommand : ICommand
{
    private readonly AppNavigationManager _appNavigationManager;
    private readonly TestBrowser _browser;
    private readonly IStringLocalizer<SharedStrings> _loc;
    public NewTestSuiteCommand(AppNavigationManager appNavigationManager, TestBrowser browser, IStringLocalizer<SharedStrings> loc)
    {
        _appNavigationManager = appNavigationManager;
        _browser = browser;
        _loc = loc;
    }

    public int SortOrder => 10;
    public string? Folder => _loc["add"];

    public PermissionEntityType? PermissionEntityType => Domain.Identity.Permissions.PermissionEntityType.TestSuite;
    public PermissionLevel? RequiredLevel => PermissionLevel.ReadWrite;
    public bool Enabled => true;
    public string Id => "new-test-suite";
    public string Name => _loc["new-test-suite"];
    public string Description => _loc["new-test-suite-description"];
    public KeyboardBinding? DefaultKeyboardBinding => null;
    public string? Icon => Icons.Material.Filled.Add;
    public string[] ContextMenuTypes => ["TestSuite", "TestSuiteFolder", "menu-new", "menu-test"];

    public async ValueTask ExecuteAsync()
    {
        if (_appNavigationManager.State.SelectedTeam is null)
        {
            return;
        }
        if (_appNavigationManager.State.SelectedProject is null)
        {
            return;
        }
        await _browser.AddTestSuiteAsync(_appNavigationManager.State.SelectedTeam, _appNavigationManager.State.SelectedProject);
    }
}
