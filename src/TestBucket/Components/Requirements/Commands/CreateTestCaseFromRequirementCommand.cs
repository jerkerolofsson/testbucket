
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.Localization;

using TestBucket.Components.Requirements.Controls;
using TestBucket.Components.Requirements.Services;
using TestBucket.Components.Shared.Icons;
using TestBucket.Components.Tests.TestCases.Controls;
using TestBucket.Components.Tests.TestCases.Services;
using TestBucket.Domain.Commands;
using TestBucket.Domain.Identity.Permissions;
using TestBucket.Domain.Keyboard;
using TestBucket.Domain.Requirements.Models;
using TestBucket.Localization;

namespace TestBucket.Components.Requirements.Commands;

internal class CreateTestCaseFromRequirementCommand : ICommand
{
    public string Id => "create-test-from-requirement";

    public string Name => _loc["create-test-from-requirement"];

    public string Description => _loc["create-test-from-requirement-description"];

    public bool Enabled => _appNav.State.SelectedRequirement is not null && _appNav.State.SelectedProject is not null;
    public PermissionLevel? RequiredLevel => PermissionLevel.ReadWrite;
    public KeyboardBinding? DefaultKeyboardBinding => null;

    public string? Icon => TbIcons.Filled.PaperPlane;

    public string[] ContextMenuTypes => ["Requirement"];

    private readonly IStringLocalizer<RequirementStrings> _loc;
    private readonly AppNavigationManager _appNav;
    private readonly RequirementEditorController _requirementEditor;
    private readonly TestCaseEditorController _testCaseEditor;

    public CreateTestCaseFromRequirementCommand(
        IStringLocalizer<RequirementStrings> loc, 
        AppNavigationManager appNav, 
        RequirementEditorController requirementEditor, 
        TestCaseEditorController testCaseEditor)
    {
        _loc = loc;
        _appNav = appNav;
        _requirementEditor = requirementEditor;
        _testCaseEditor = testCaseEditor;
    }

    public async ValueTask ExecuteAsync()
    {
        if (_appNav.State.SelectedRequirement is null || _appNav.State.SelectedProject is null)
        {
            return;
        }
        var testCase = await _testCaseEditor.CreateNewTestCaseAsync(_appNav.State.SelectedProject, null, null, _appNav.State.SelectedRequirement.Name);
        if (testCase is not null)
        {
            await _requirementEditor.LinkRequirementToTestCaseAsync(_appNav.State.SelectedRequirement, testCase);
        }
    }
}
