using Microsoft.Extensions.Localization;

using TestBucket.Components.Tests.TestCases.Controls;
using TestBucket.Components.Tests.TestCases.Services;
using TestBucket.Domain.Commands;
using TestBucket.Domain.Keyboard;
using TestBucket.Localization;

namespace TestBucket.Components.Tests.TestRuns.Commands;

internal class DeleteTestCommand : ICommand
{
    private readonly AppNavigationManager _appNavigationManager;
    private readonly TestCaseEditorController _controller;
    private readonly IStringLocalizer<SharedStrings> _loc;

    public int SortOrder => 99;

    public string? Folder => null;

    public DeleteTestCommand(AppNavigationManager appNavigationManager, TestCaseEditorController browser, IStringLocalizer<SharedStrings> loc)
    {
        _appNavigationManager = appNavigationManager;
        _controller = browser;
        _loc = loc;
    }

    public PermissionEntityType? PermissionEntityType => Domain.Identity.Permissions.PermissionEntityType.TestCase;
    public PermissionLevel? RequiredLevel => PermissionLevel.Delete;
    public bool Enabled => _appNavigationManager.State.SelectedTestCase is not null;
    public string Id => "delete-test";
    public string Name => _loc["delete-test"];
    public string Description => "";
    public KeyboardBinding? DefaultKeyboardBinding => null;
    public string? Icon => Icons.Material.Filled.Delete;
    public string[] ContextMenuTypes => ["TestCase"];

    public async ValueTask ExecuteAsync(ClaimsPrincipal principal)
    {
        if (_appNavigationManager.State.SelectedTestCase is null)
        {
            return;
        }
        await _controller.DeleteTestCaseAsync(_appNavigationManager.State.SelectedTestCase);
        //await _controller.DuplicateTestAsync(_appNavigationManager.State.SelectedTestCase);
    }
}
