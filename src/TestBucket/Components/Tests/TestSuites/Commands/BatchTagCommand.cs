using System.Threading;

using Microsoft.Extensions.Localization;

using TestBucket.Components.Shared.Fields;
using TestBucket.Components.Tests.Dialogs;
using TestBucket.Components.Tests.Services;
using TestBucket.Components.Tests.TestCases.Services;
using TestBucket.Components.Tests.TestRuns.Controllers;
using TestBucket.Contracts.Fields;
using TestBucket.Contracts.Testing.States;
using TestBucket.Domain;
using TestBucket.Domain.Commands;
using TestBucket.Domain.Keyboard;
using TestBucket.Domain.Progress;
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
    public bool Enabled => _appNavigationManager.State.SelectedTestSuite is not null ||
        _appNavigationManager.State.SelectedTestSuiteFolder is not null ||
        _appNavigationManager.State.SelectedTestCase is not null ||
        _appNavigationManager.State.MultiSelectedTestCases.Count > 0;

    public string Id => "batch-tag";
    public string Name => _loc["batch-tag"];
    public string Description => _loc["batch-tag-description"];
    public KeyboardBinding? DefaultKeyboardBinding => null;
    public string? Icon => TbIcons.BoldDuoTone.Field;
    public string[] ContextMenuTypes => ["TestSuiteFolder", "TestSuite", "TestCase"];

    public async ValueTask ExecuteAsync(ClaimsPrincipal principal)
    {
        var suite = _appNavigationManager.State.SelectedTestSuite;
        var folder = _appNavigationManager.State.SelectedTestSuiteFolder;
        var tests = _appNavigationManager.State.MultiSelectedTestCases.ToList();

        if (tests.Count == 0 && _appNavigationManager.State.SelectedTestCase is not null)
        {
            tests.Add(_appNavigationManager.State.SelectedTestCase);
        }

        if (folder is null && suite is null && tests.Count == 0)
        {
            return;
        }
        var projectId = folder?.TestProjectId ?? suite?.TestProjectId;
        if (tests.Count > 0)
        {
            projectId = tests.First().TestProjectId;
        }
        if (projectId is null)
        {
            return;
        }

        var progressMessage = _loc["batch-updating-tests"];

        var parameters = new DialogParameters<BatchTagFieldDialog>() { { x => x.ProjectId, projectId }, { x => x.Target, FieldTarget.TestCase } };
        var dialog = await _dialogService.ShowAsync<BatchTagFieldDialog>(null, parameters, DefaultBehaviors.DialogOptions);
        var result = await dialog.Result;
        if (result?.Data is FieldValue field)
        {
            await using var progress = _progressManager.CreateProgressTask(progressMessage);
            long[] testCaseIds = await GetTestCaseIdsAsync(suite, folder, tests);

            await _fieldController.UpdateTestCaseFieldsAsync(testCaseIds, projectId, new FieldValue[] { field });
        }
        else if (result?.Data is Requirement requirement)
        {
            await using var progress = _progressManager.CreateProgressTask(progressMessage);
            long[] testCaseIds = await GetTestCaseIdsAsync(suite, folder, tests);

            int count = 0;
            foreach (var testCaseId in testCaseIds)
            {
                await _testEditor.LinkTestCaseToRequirementAsync(testCaseId, requirement);

                count++;
                double percent = count * 100.0 / testCaseIds.Length;
                await progress.ReportStatusAsync(progressMessage, percent);
            }
        }
        else if (result?.Data is TestState state)
        {
            await using var progress = _progressManager.CreateProgressTask(progressMessage);
            long[] testCaseIds = await GetTestCaseIdsAsync(suite, folder, tests);

            int count = 0;
            foreach (var testCaseId in testCaseIds)
            {
                await _testEditor.SetTestCaseStateAsync(testCaseId, state);

                count++;
                double percent = count * 100.0 / testCaseIds.Length;
                await progress.ReportStatusAsync(progressMessage, percent);
            }
        }
    }

    private async Task<long[]> GetTestCaseIdsAsync(TestSuite? suite, TestSuiteFolder? folder, IReadOnlyList<TestCase> tests)
    {
        long[] testCaseIds = tests.Select(x => x.Id).ToArray();
        if (testCaseIds.Length == 0)
        {
            if (folder is not null)
            {
                testCaseIds = await _browser.GetTestSuiteSuiteFolderTestsAsync(folder, includeAllDescendants: true);
            }
            else if (suite is not null)
            {
                testCaseIds = await _browser.GetTestSuiteSuiteTestsAsync(suite);
            }
        }

        return testCaseIds;
    }
}
