﻿using Microsoft.Extensions.Localization;

using TestBucket.Components.Tests.TestCases.Services;
using TestBucket.Contracts.Keyboard;
using TestBucket.Domain;
using TestBucket.Domain.Commands;
using TestBucket.Domain.Keyboard;
using TestBucket.Localization;

namespace TestBucket.Components.Tests.TestCases.Commands;

/// <summary>
/// Creates a new test
/// </summary>
internal class NewTestCommand : ICommand
{
    private readonly AppNavigationManager _appNavigationManager;
    private readonly TestCaseEditorController _controller;
    private readonly IStringLocalizer<SharedStrings> _loc;
    public int SortOrder => 10;
    public string? Folder => _loc["add"];

    public PermissionEntityType? PermissionEntityType => Domain.Identity.Permissions.PermissionEntityType.TestCase;
    public PermissionLevel? RequiredLevel => PermissionLevel.ReadWrite;
    public bool Enabled => _appNavigationManager.State.SelectedProject is not null;
    public string Id => "new-test";
    public string Name => _loc[Id];
    public string Description => _loc["new-test-description"];
    public KeyboardBinding? DefaultKeyboardBinding => new KeyboardBinding() { CommandId = "new-test", Key = "KeyA", ModifierKeys = ModifierKey.Ctrl | ModifierKey.Shift };
    public string? Icon => TbIcons.BoldDuoTone.File;
    public string[] ContextMenuTypes => ["TestSuite", "TestSuiteFolder", "menu-new"];

    public NewTestCommand(AppNavigationManager appNavigationManager, TestCaseEditorController controller, IStringLocalizer<SharedStrings> loc)
    {
        _appNavigationManager = appNavigationManager;
        _controller = controller;
        _loc = loc;
    }

    public async ValueTask ExecuteAsync(ClaimsPrincipal principal)
    {
        if (_appNavigationManager.State.SelectedProject is null)
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
