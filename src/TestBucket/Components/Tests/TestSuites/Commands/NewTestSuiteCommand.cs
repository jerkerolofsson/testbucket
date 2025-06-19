using Microsoft.Extensions.Localization;

using TestBucket.Components.Tests.Services;
using TestBucket.Components.Tests.TestSuites.Services;
using TestBucket.Domain.Commands;
using TestBucket.Domain.Identity.Permissions;
using TestBucket.Domain.Keyboard;
using TestBucket.Domain.Teams;
using TestBucket.Localization;

namespace TestBucket.Components.Tests.TestSuites.Commands;

internal class NewTestSuiteCommand : ICommand
{
    private readonly AppNavigationManager _appNavigationManager;
    private readonly TestBrowser _browser;
    private readonly TestSuiteController _controller;
    private readonly IStringLocalizer<SharedStrings> _loc;
    private readonly ITeamManager _teamManager;


    public int SortOrder => 10;
    public string? Folder => _loc["add"];

    public PermissionEntityType? PermissionEntityType => Domain.Identity.Permissions.PermissionEntityType.TestSuite;
    public PermissionLevel? RequiredLevel => PermissionLevel.ReadWrite;
    public bool Enabled => _appNavigationManager.State.SelectedProject?.TeamId is not null;
    public string Id => "new-test-suite";
    public string Name => _loc["new-test-suite"];
    public string Description => _loc["new-test-suite-description"];
    public KeyboardBinding? DefaultKeyboardBinding => null;
    public string? Icon => Icons.Material.Filled.Add;
    public string[] ContextMenuTypes => ["TestSuite", "TestSuiteFolder", "TestRepositoryFolder", "menu-new", "menu-test"];

    public NewTestSuiteCommand(
        AppNavigationManager appNavigationManager,
        TestBrowser browser,
        IStringLocalizer<SharedStrings> loc,
        ITeamManager teamManager,
        TestSuiteController controller)
    {
        _appNavigationManager = appNavigationManager;
        _browser = browser;
        _loc = loc;
        _teamManager = teamManager;
        _controller = controller;
    }

    public async ValueTask ExecuteAsync(ClaimsPrincipal principal)
    {
        var project = _appNavigationManager.State.SelectedProject;
        if (project?.TeamId is null)
        {
            return;
        }

        var team = _appNavigationManager.State.SelectedTeam ?? await _teamManager.GetTeamByIdAsync(principal, project.TeamId.Value);

        var suite = await _browser.AddTestSuiteAsync(team, project);

        if(suite is not null && _appNavigationManager.State.SelectedTestRepositoryFolder is not null)
        {
            // Move it into the folder
            suite.FolderId = _appNavigationManager.State.SelectedTestRepositoryFolder.Id;
            await _controller.SaveTestSuiteAsync(suite);
        }
    }
}
