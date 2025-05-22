using Microsoft.Extensions.Localization;

using TestBucket.Components.Tests.Services;
using TestBucket.Components.Tests.TestCases.Controls;
using TestBucket.Components.Tests.TestCases.Services;
using TestBucket.Domain;
using TestBucket.Domain.Commands;
using TestBucket.Domain.Keyboard;
using TestBucket.Localization;

namespace TestBucket.Components.Tests.TestRuns.Commands;

internal class ImportTestResultsCommand : ICommand
{
    private readonly AppNavigationManager _appNavigationManager;
    private readonly TestBrowser _browser;
    private readonly IStringLocalizer<SharedStrings> _loc;

    public int SortOrder => 99;

    public string? Folder => null;

    public ImportTestResultsCommand(AppNavigationManager appNavigationManager, TestBrowser browser, IStringLocalizer<SharedStrings> loc)
    {
        _appNavigationManager = appNavigationManager;
        _browser = browser;
        _loc = loc;
    }

    public PermissionEntityType? PermissionEntityType => Domain.Identity.Permissions.PermissionEntityType.TestRun;
    public PermissionLevel? RequiredLevel => PermissionLevel.ReadWrite;
    public bool Enabled => _appNavigationManager.State.SelectedProject is not null;
    public string Id => "import-results";
    public string Name => _loc["import-test-results"];
    public string Description => _loc["import-test-results"];
    public KeyboardBinding? DefaultKeyboardBinding => null;
    public string? Icon => TbIcons.BoldDuoTone.Import;
    public string[] ContextMenuTypes => ["menu-test"];

    public async ValueTask ExecuteAsync(ClaimsPrincipal principal)
    {
        if (_appNavigationManager.State.SelectedProject is null)
        {
            return;
        }
        await _browser.ImportAsync(_appNavigationManager.State.SelectedTeam, _appNavigationManager.State.SelectedProject);
    }
}
