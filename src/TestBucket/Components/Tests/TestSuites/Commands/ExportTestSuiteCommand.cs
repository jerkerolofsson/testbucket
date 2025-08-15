using TestBucket.Contracts.Localization;
using TestBucket.Domain;
using TestBucket.Domain.Commands;
using TestBucket.Domain.Keyboard;

namespace TestBucket.Components.Requirements.Commands.Collections;

internal class ExportTestSuiteCommand : ICommand
{
    public string Id => "export-test-suite";

    public string Name => _loc.Shared["export-test-suite"];

    public string Description => _loc.Shared["export-test-suite-description"];

    public bool Enabled => _appNav.State.SelectedTestSuite is not null;
    public int SortOrder => 69;
    public string? Folder => null;

    public KeyboardBinding? DefaultKeyboardBinding => null;
    public string? Icon => TbIcons.BoldDuoTone.Export;
    public string[] ContextMenuTypes => ["TestSuite"];
    public PermissionEntityType? PermissionEntityType => Domain.Identity.Permissions.PermissionEntityType.TestSuite;
    public PermissionLevel? RequiredLevel => PermissionLevel.Read;
    private readonly IAppLocalization _loc;
    private readonly AppNavigationManager _appNav;

    public ExportTestSuiteCommand(
        AppNavigationManager appNav,
        IAppLocalization loc)
    {
        _appNav = appNav;
        _loc = loc;
    }

    public ValueTask ExecuteAsync(ClaimsPrincipal principal)
    {
        var testSuite = _appNav.State.SelectedTestSuite;
        if(testSuite is null)
        {
            return ValueTask.CompletedTask;
        }

        _appNav.NavigateTo(_appNav.GetExportTestSuiteUrl(testSuite), true);
        return ValueTask.CompletedTask;
    }
}
