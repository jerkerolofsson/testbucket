using Microsoft.Extensions.Localization;

using TestBucket.Components.Tests.TestCases.Services;
using TestBucket.Domain.Commands;
using TestBucket.Domain.Keyboard;
using TestBucket.Localization;

namespace TestBucket.Components.Tests.TestCases.Commands;

internal class DuplicateTestCommand : ICommand
{
    private readonly AppNavigationManager _appNavigationManager;
    private readonly TestCaseEditorController _controller;
    private readonly IStringLocalizer<SharedStrings> _loc;

    public int SortOrder => 30;

    public string? Folder => null;

    public PermissionEntityType? PermissionEntityType => Domain.Identity.Permissions.PermissionEntityType.TestCase;
    public PermissionLevel? RequiredLevel => PermissionLevel.Read;
    public bool Enabled => _appNavigationManager.State.SelectedTestCase is not null || 
        _appNavigationManager.State.MultiSelectedTestCases.Count > 0;
    public string Id => "duplicate-test";
    public string Name => _loc["duplicate-test"];
    public string Description => "";
    public KeyboardBinding? DefaultKeyboardBinding => null;
    public string? Icon => Icons.Material.Filled.ContentCopy;
    public string[] ContextMenuTypes => ["TestCase"];

    public DuplicateTestCommand(AppNavigationManager appNavigationManager, TestCaseEditorController browser, IStringLocalizer<SharedStrings> loc)
    {
        _appNavigationManager = appNavigationManager;
        _controller = browser;
        _loc = loc;
    }

    public async ValueTask ExecuteAsync(ClaimsPrincipal principal)
    {
        if(_appNavigationManager.State.MultiSelectedTestCases.Count > 0)
        {
            foreach(var test in _appNavigationManager.State.MultiSelectedTestCases)
            {
                await _controller.DuplicateTestAsync(test);
            }
        }
        else if (_appNavigationManager.State.SelectedTestCase is { })
        {
            await _controller.DuplicateTestAsync(_appNavigationManager.State.SelectedTestCase);
        }
    }
}
