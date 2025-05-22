using Microsoft.Extensions.Localization;
using TestBucket.Components.Tests.Services;
using TestBucket.Components.Tests.TestSuites.Services;
using TestBucket.Domain.Commands;
using TestBucket.Domain.Keyboard;
using TestBucket.Localization;

namespace TestBucket.Components.Tests.TestSuites.Commands;

internal class DeleteTestSuiteCommand : ICommand
{
    private readonly AppNavigationManager _appNavigationManager;
    private readonly TestSuiteService _browser;
    private readonly IStringLocalizer<SharedStrings> _loc;
    public DeleteTestSuiteCommand(AppNavigationManager appNavigationManager, TestSuiteService browser, IStringLocalizer<SharedStrings> loc)
    {
        _appNavigationManager = appNavigationManager;
        _browser = browser;
        _loc = loc;
    }

    public int SortOrder => 90;
    public string? Folder => null;

    public PermissionEntityType? PermissionEntityType => Domain.Identity.Permissions.PermissionEntityType.TestSuite;
    public PermissionLevel? RequiredLevel => PermissionLevel.ReadWrite;
    public bool Enabled => _appNavigationManager.State.SelectedTestSuite is not null;
    public string Id => "delete-test-suite";
    public string Name => _loc["delete-test-suite"];
    public string Description => _loc["delete-test-suite-description"];
    public KeyboardBinding? DefaultKeyboardBinding => null;
    public string? Icon => Icons.Material.Filled.Delete;
    public string[] ContextMenuTypes => ["TestSuite"];

    public async ValueTask ExecuteAsync(ClaimsPrincipal principal)
    {
        var suite = _appNavigationManager.State.SelectedTestSuite;
        if (suite is null)
        {
            return;
        }
        await _browser.DeleteTestSuiteByIdAsync(suite.Id);
    }
}
