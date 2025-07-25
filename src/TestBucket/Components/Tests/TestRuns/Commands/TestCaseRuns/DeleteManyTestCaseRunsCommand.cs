using Microsoft.Extensions.Localization;

using TestBucket.Components.Tests.TestCases.Controls;
using TestBucket.Components.Tests.TestCases.Services;
using TestBucket.Components.Tests.TestRuns.Controllers;
using TestBucket.Domain.Commands;
using TestBucket.Domain.Keyboard;
using TestBucket.Domain.Testing.TestRuns;
using TestBucket.Localization;

namespace TestBucket.Components.Tests.TestRuns.Commands.TestCaseRuns;

/// <summary>
/// Deletes multiple runs. This is only enabled if 2 or more are selected
/// </summary>
internal class DeleteManyTestCaseRunsCommand : ICommand
{
    private readonly AppNavigationManager _appNavigationManager;
    private readonly IStringLocalizer<SharedStrings> _loc;
    private readonly TestRunController _controller;

    public int SortOrder => 99;
    public string? Folder => null;
    public PermissionEntityType? PermissionEntityType => Domain.Identity.Permissions.PermissionEntityType.TestCaseRun;
    public PermissionLevel? RequiredLevel => PermissionLevel.Delete;
    public bool Enabled => _appNavigationManager.State.MultiSelectedTestCaseRuns.Count >= 2;
    public string Id => "delete-many-test-case-runs";

    public string Name
    {
        get
        {
            if (_appNavigationManager.State.MultiSelectedTestCaseRuns.Count > 0)
            {
                return string.Format(_loc["delete-many-test-case-runs"], _appNavigationManager.State.MultiSelectedTestCaseRuns.Count);
            }
            return "";
        }
    }
    public string Description => _loc["delete-many-test-case-runs-description"];
    public KeyboardBinding? DefaultKeyboardBinding => null;
    public string? Icon => Icons.Material.Filled.Delete;
    public string[] ContextMenuTypes => ["TestCaseRun"];
    public DeleteManyTestCaseRunsCommand(AppNavigationManager appNavigationManager, IStringLocalizer<SharedStrings> loc, TestRunController controller)
    {
        _appNavigationManager = appNavigationManager;
        _loc = loc;
        _controller = controller;
    }

    public async ValueTask ExecuteAsync(ClaimsPrincipal principal)
    {
        await _controller.DeleteTestCaseRunsAsync(_appNavigationManager.State.MultiSelectedTestCaseRuns);
        _appNavigationManager.State.SetMultiSelectedTestCaseRuns([]);
    }
}
