using Microsoft.Extensions.Localization;

using TestBucket.Components.Requirements.Services;
using TestBucket.Components.Shared;
using TestBucket.Components.Shared.Fields;
using TestBucket.Components.Tests.Dialogs;
using TestBucket.Components.Tests.Services;
using TestBucket.Components.Tests.TestCases.Services;
using TestBucket.Components.Tests.TestRuns.Controllers;
using TestBucket.Contracts.Fields;
using TestBucket.Domain;
using TestBucket.Domain.Commands;
using TestBucket.Domain.Identity.Permissions;
using TestBucket.Domain.Keyboard;
using TestBucket.Domain.Progress;
using TestBucket.Domain.Requirements;
using TestBucket.Domain.Requirements.Models;
using TestBucket.Localization;

namespace TestBucket.Components.Tests.TestSuites.Commands;

internal class BatchTagCommand : ICommand
{
    private readonly IStringLocalizer<SharedStrings> _loc;
    private readonly AppNavigationManager _appNavigationManager;
    private readonly TestBrowser _browser;
    private readonly IDialogService _dialogService;
    private readonly IProgressManager _progressManager;
    private readonly FieldController _fieldController;
    private readonly TestCaseEditorController _testEditor;

    public int SortOrder => 60;
    public string? Folder => null;

    public BatchTagCommand(
        IStringLocalizer<SharedStrings> loc,
        AppNavigationManager appNavigationManager,
        TestBrowser browser,
        TestRunCreationController testRunCreationController,
        IDialogService dialogService,
        IProgressManager progressManager,
        FieldController fieldController,
        TestCaseEditorController testEditor)
    {
        _loc = loc;
        _appNavigationManager = appNavigationManager;
        _browser = browser;
        _dialogService = dialogService;
        _progressManager = progressManager;
        _fieldController = fieldController;
        _testEditor = testEditor;
    }
    public PermissionEntityType? PermissionEntityType => Domain.Identity.Permissions.PermissionEntityType.TestCase;
    public PermissionLevel? RequiredLevel => PermissionLevel.ReadWrite;
    public bool Enabled => _appNavigationManager.State.SelectedTestSuite is not null;
    public string Id => "batch-tag";
    public string Name => _loc["batch-tag"];
    public string Description => "Applies tags to all descendant tests";
    public KeyboardBinding? DefaultKeyboardBinding => null;
    public string? Icon => TbIcons.BoldDuoTone.Field;
    public string[] ContextMenuTypes => ["TestSuiteFolder", "TestSuite"];

    public async ValueTask ExecuteAsync()
    {
        var suite = _appNavigationManager.State.SelectedTestSuite;
        var folder = _appNavigationManager.State.SelectedTestSuiteFolder;
        if (folder is null && suite is null)
        {
            return;
        }
        var projectId = folder?.TestProjectId ?? suite?.TestProjectId;
        if (projectId is null)
        {
            return;
        }

        var parameters = new DialogParameters<BatchTagFieldDialog>() { { x => x.ProjectId, projectId }, { x => x.Target, FieldTarget.TestCase } };
        var dialog = await _dialogService.ShowAsync<BatchTagFieldDialog>(null, parameters, DefaultBehaviors.DialogOptions);
        var result = await dialog.Result;
        if(result?.Data is FieldValue field)
        {
            await using var progress = _progressManager.CreateProgressTask("Updating tests..");

            long[] testCaseIds = [];
            if (folder is not null)
            {
                testCaseIds = await _browser.GetTestSuiteSuiteFolderTestsAsync(folder, includeAllDescendants: true);
            }
            else if (suite is not null)
            {
                testCaseIds = await _browser.GetTestSuiteSuiteTestsAsync(suite);
            }

            await _fieldController.UpdateTestCaseFieldsAsync(testCaseIds, projectId, new FieldValue[] { field });
        }
        else if (result?.Data is Requirement requirement)
        {
            await using var progress = _progressManager.CreateProgressTask("Updating tests..");

            long[] testCaseIds = [];
            if (folder is not null)
            {
                testCaseIds = await _browser.GetTestSuiteSuiteFolderTestsAsync(folder, includeAllDescendants: true);
            }
            else if (suite is not null)
            {
                testCaseIds = await _browser.GetTestSuiteSuiteTestsAsync(suite);
            }

            foreach (var testCaseId in testCaseIds)
            {
                await _testEditor.LinkTestCaseToRequirementAsync(testCaseId, requirement);
            }
        }
    }
}
