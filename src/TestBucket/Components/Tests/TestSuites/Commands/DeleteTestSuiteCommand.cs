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
    private readonly TestSuiteController _browser;
    private readonly IStringLocalizer<SharedStrings> _loc;
    private readonly IDialogService _dialogService;
    public DeleteTestSuiteCommand(AppNavigationManager appNavigationManager, TestSuiteController browser, IStringLocalizer<SharedStrings> loc, IDialogService dialogService)
    {
        _appNavigationManager = appNavigationManager;
        _browser = browser;
        _loc = loc;
        _dialogService = dialogService;
    }

    public int SortOrder => 95;
    public string? Folder => null;

    public PermissionEntityType? PermissionEntityType => Domain.Identity.Permissions.PermissionEntityType.TestSuite;
    public PermissionLevel? RequiredLevel => PermissionLevel.Delete;
    public bool Enabled => _appNavigationManager.State.SelectedTestSuite is not null &&
        _appNavigationManager.State.SelectedTestRun is null &&
        _appNavigationManager.State.SelectedTestCase is null &&
        _appNavigationManager.State.SelectedTestSuiteFolder is null;
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
