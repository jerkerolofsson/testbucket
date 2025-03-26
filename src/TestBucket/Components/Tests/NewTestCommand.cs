
using TestBucket.Components.Shared;
using TestBucket.Components.Tests.Controls;
using TestBucket.Components.Tests.Services;
using TestBucket.Domain.Commands;
using TestBucket.Domain.Keyboard;

namespace TestBucket.Components.Tests;

internal class NewTestCommand : ICommand
{
    private readonly AppNavigationManager _appNavigationManager;
    private readonly TestCaseEditorController _controller;

    public NewTestCommand(AppNavigationManager appNavigationManager, TestCaseEditorController controller)
    {
        _appNavigationManager = appNavigationManager;
        _controller = controller;
    }

    public bool Enabled => _appNavigationManager.State.SelectedProject is not null;
    public string Id => "new-test";
    public string Name => "New Test";
    public string Description => "Create a new test case";
    public KeyboardBinding DefaultKeyboardBinding => new KeyboardBinding() { CommandId = "new-test", Key = "KeyA", ModifierKeys = ModifierKey.Ctrl | ModifierKey.Shift };
    public string? Icon => Icons.Material.Filled.Add;
    public string[] ContextMenuTypes => ["TestSuite", "TestSuiteFolder", "menu-new"];

    public async ValueTask ExecuteAsync()
    {
        if(_appNavigationManager.State.SelectedProject is null)
        {
            return;
        }

        var testCase = await _controller.CreateNewTestCaseAsync(_appNavigationManager.State.SelectedProject, _appNavigationManager.State.SelectedTestSuiteFolder, _appNavigationManager.State.SelectedTestSuite?.Id);
        if (testCase is not null)
        {
            _appNavigationManager.NavigateTo(testCase);
        }
    }
}
