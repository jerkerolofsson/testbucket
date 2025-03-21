﻿using TestBucket.Components.Shared.Tree;
using TestBucket.Domain.Requirements.Models;
using TestBucket.Domain.Testing;

namespace TestBucket.Components.Tests.Controls;

public partial class TestCaseRunGrid
{
    [Parameter] public TestRun? Run { get; set; }
    [Parameter] public EventCallback<TestCaseRun> OnTestCaseRunClicked { get; set; }

    private string? _searchText;

    private MudDataGrid<TestCaseRun?> _dataGrid = default!;

    public Task OnTestCreatedAsync(TestCase testCase)
    {
        return Task.CompletedTask;
    }
    public Task OnTestDeletedAsync(TestCase testCase)
    {
        return Task.CompletedTask;
    }
    public Task OnTestSavedAsync(TestCase testCase)
    {
        return Task.CompletedTask;
    }

    #region Lifecycle
    protected override void OnInitialized()
    {
        testCaseManager.AddObserver(this);
    }

    protected override void OnParametersSet()
    {
        _dataGrid?.ReloadServerData();
    }

    public void Dispose()
    {
        testCaseManager.RemoveObserver(this);
    }
    #endregion

    private async Task OnDrop(TestEntity? testEntity)
    {
        if (testEntity is null)
        {
            return;
        }
       
        if (testEntity is TestCase testCase)
        {
            await OnDropTestCaseAsync(testCase);
        }
        if (testEntity is TestSuiteFolder testSuiteFolder)
        {
            await OnDropTestSuiteFolderAsync(testSuiteFolder);
        }
    }

    private async Task OnDropTestSuiteFolderAsync(TestSuiteFolder folder)
    {
        if (Run is null || Run.TestProjectId is null)
        {
            return;
        }

        long[] testCaseIds = await testBrowser.GetTestSuiteSuiteFolderTestsAsync(folder, true);
        foreach (var testCaseId in testCaseIds)
        {
            await testRunCreationService.AddTestCaseToRunAsync(Run, testCaseId);
        }

        _dataGrid?.ReloadServerData();
    }
    private async Task OnDropTestCaseAsync(TestCase testCase)
    {
        if(Run is null || Run.TestProjectId is null)
        {
            return;
        }

        await testRunCreationService.AddTestCaseToRunAsync(Run, testCase);

        _dataGrid?.ReloadServerData();
    }

    private async Task OnClicked(TestCaseRun testCaseRun)
    {
        await OnTestCaseRunClicked.InvokeAsync(testCaseRun);
    }

    //private async Task CreateNewTestCaseAsync()
    //{
    //    if (TestSuiteId is not null || Folder is not null)
    //    {
    //        TestCase? testCase = await testCaseEditor.CreateNewTestCaseAsync(Folder, TestSuiteId);
    //        if (testCase is not null)
    //        {
    //            _dataGrid?.ReloadServerData();
    //        }
    //    }
    //}

    private void OnSearch(string text)
    {
        _searchText = text;
        _dataGrid?.ReloadServerData();
    }

    private async Task<GridData<TestCaseRun>> LoadGridData(GridState<TestCaseRun> state)
    {
        if(Run is null)
        {
            return new()
            {
                Items = [],
                TotalItems = 0
            };
        }
        var result = await testBrowser.SearchTestCaseRunsAsync(Run.Id, _searchText, state.Page * state.PageSize, state.PageSize);

        GridData<TestCaseRun> data = new()
        {
            Items = result.Items,
            TotalItems = (int)result.TotalCount
        };

        return data;
    }

}
