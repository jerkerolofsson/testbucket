using TestBucket.Components.Tests.Dialogs;
using TestBucket.Domain.Teams.Models;

namespace TestBucket.Components.Tests.Controls;

public partial class TestTreeView
{
    #region Parameters
    /// <summary>
    /// Invoked when a folder is clicked
    /// </summary>
    [Parameter] public EventCallback<TestSuiteFolder> OnFolderClicked { get; set; }

    /// <summary>
    /// Invoked when a run is clicked
    /// </summary>
    [Parameter] public EventCallback<TestRun> OnTestRunClicked { get; set; }

    /// <summary>
    /// Invokeed when a test suite is clicked
    /// </summary>
    [Parameter] public EventCallback<TestSuite> OnTestSuiteClicked { get; set; }

    /// <summary>
    /// Invoked when a test case is clicked
    /// </summary>
    [Parameter] public EventCallback<TestCase> OnTestCaseClicked { get; set; }

    [CascadingParameter] protected TestProject? Project { get; set; }

    [CascadingParameter] protected Team? Team { get; set; }
    #endregion

    private TestProject? _project;
    private Team? _team;

    /// <summary>
    /// Instance of the tree view UI 
    /// </summary>
    private MudTreeView<BrowserItem> _treeView = default!;

    /// <summary>
    /// Root items in the tree view
    /// </summary>
    private List<TreeItemData<BrowserItem>> _rootItems = [];

    /// <summary>
    /// currently selected item
    /// </summary>
    private BrowserItem? _selectedTreeItem;

    private TreeItemData<BrowserItem>? RemoveTreeItemData(Predicate<BrowserItem> predicate)
    {
        return RemoveTreeItemData(null, _rootItems, predicate);
    }

    private TreeItemData<BrowserItem>? RemoveTreeItemData(TreeItemData<BrowserItem>? parentNode, IEnumerable<TreeItemData<BrowserItem>> treeItems, Predicate<BrowserItem> predicate)
    {
        foreach (var node in treeItems)
        {
            if (node.Value is not null && predicate(node.Value))
            {
                if (parentNode?.Children is not null)
                {
                    parentNode.Children = parentNode.Children.Where(x => x.Value != null && !predicate(x.Value)).ToList();
                }

                return node;
            }
            else if (node.Children is not null)
            {
                var match = RemoveTreeItemData(node, node.Children, predicate);
                if (match is not null)
                {
                    return match;
                }
            }
        }
        return null;
    }

    private TreeItemData<BrowserItem>? FindTreeItemData(Predicate<BrowserItem> predicate)
    {
        return FindTreeItemData(_rootItems, predicate);
    }

    private TreeItemData<BrowserItem>? FindTreeItemData(IEnumerable<TreeItemData<BrowserItem>> treeItems, Predicate<BrowserItem> predicate)
    {
        foreach (var node in treeItems)
        {
            if (node.Value is not null && predicate(node.Value))
            {
                return node;
            }
            else if (node.Children is not null)
            {
                var match = FindTreeItemData(node.Children, predicate);
                if (match is not null)
                {
                    return match;
                }
            }
        }
        return null;
    }

    private async Task OnDrop(TestCase? testCase, TreeItemData<BrowserItem>? targetNode)
    {
        var target = targetNode?.Value;
        if (target?.Folder is not null && targetNode is not null && testCase is not null)
        {
            var fromFolderId = testCase.TestSuiteFolderId;

            // Find the source
            TreeItemData<BrowserItem>? sourceNode = FindTreeItemData(x => x.Folder?.Id == fromFolderId);

            testCase.TestSuiteFolderId = target.Folder.Id;
            testCase.TestSuiteId = target.Folder.TestSuiteId;

            await testCaseEditor.SaveTestCaseAsync(testCase);

            // Refresh the target
            targetNode.Children = await testBrowser.BrowseAsync(_team?.Id, _project?.Id, target);

            // Refresh the source node
            if (sourceNode is not null)
            {
                sourceNode.Children = await testBrowser.BrowseAsync(_team?.Id, _project?.Id, sourceNode.Value);
            }
        }
    }

    private async Task EditTestSuiteFolderAsync(TestSuiteFolder? folder)
    {
        if (_selectedTreeItem?.Folder is not null)
        {
            await testBrowser.CustomizeFolderAsync(_selectedTreeItem.Folder);

            // Refresh tree node
            var treeNode = FindTreeItemData(x => x.Folder?.Id == _selectedTreeItem.Folder.Id);
            if (treeNode is not null)
            {
                treeNode.Text = _selectedTreeItem.Folder.Name;
            }
        }
    }

    private async Task EditTestSuiteAsync()
    {
        if (_selectedTreeItem?.TestSuite is not null)
        {
            _selectedTreeItem.TestSuite.Color = "yellow";
            _selectedTreeItem.TestSuite.Icon = MudBlazor.Icons.Custom.Brands.MicrosoftAzureDevOps;
            await testSuiteService.SaveTestSuiteAsync(_selectedTreeItem.TestSuite);
        }
    }

    private async Task ImportAsync()
    {
        await testBrowser.ImportAsync(_team, _project);
    }

    private async Task OnSelectedValueChangedAsync(BrowserItem? item)
    {
        _selectedTreeItem = item;
        if (item is not null)
        {
            if (item.Folder is not null)
            {
                await OnFolderClicked.InvokeAsync(item.Folder);
            }
            else if (item.TestSuite is not null)
            {
                await OnTestSuiteClicked.InvokeAsync(item.TestSuite);
            }
            else if (item.TestCase is not null)
            {
                await OnTestCaseClicked.InvokeAsync(item.TestCase);
            }
            else if (item.TestRun is not null)
            {
                await OnTestRunClicked.InvokeAsync(item.TestRun);
            }
        }
    }

    protected override async Task OnParametersSetAsync()
    {
        if (_rootItems.Count == 0 || _project != Project || _team != Team)
        {
            _team = Team;
            _project = Project;
            _selectedTreeItem = null;
            _rootItems = await testBrowser.BrowseAsync(_team?.Id, _project?.Id, null);
        }
    }

    public async Task<IReadOnlyCollection<TreeItemData<BrowserItem>>> LoadServerData(BrowserItem? browserItem)
    {
        // wait 500ms to simulate a server load

        return await testBrowser.BrowseAsync(_team?.Id, _project?.Id, browserItem);
    }

    private void OnItemsLoaded(TreeItemData<BrowserItem> treeItemData, IReadOnlyCollection<TreeItemData<BrowserItem>> children)
    {
        // here we store the server-loaded children in the treeItemData so that they are available in the InitialTreeItems
        // if you don't do this you loose already loaded children on next render update
        treeItemData.Children = children?.ToList();
    }

    private async Task CreateNewTestCaseAsync(TestSuiteFolder folder)
    {
        await testCaseEditor.CreateNewTestCaseAsync(folder, folder.TestSuiteId);
    }

    private async Task AddTestSuiteFolderAsync(TestSuite? testSuite, TestSuiteFolder? parentFolder, TestCase? testCase)
    {
        if (Project is not null && parentFolder is not null || _selectedTreeItem?.TestCase is not null)
        {
            var parentFolderId = parentFolder?.Id ?? testCase?.TestSuiteFolder?.Id;
            var projectId = Project?.Id ?? parentFolder?.TestProjectId ?? testCase?.TestProjectId;
            var testSuiteId = parentFolder?.TestSuiteId ?? testCase?.TestSuiteId;

            if (projectId is null || testSuiteId is null)
            {
                return;
            }
            var parameters = new DialogParameters<AddTestSuiteFolderDialog>
            {
                { x => x.ProjectId, projectId.Value },
                { x => x.TestSuiteId, testSuiteId.Value },
                { x => x.ParentFolderId, parentFolderId },
            };
            var dialog = await dialogService.ShowAsync<AddTestSuiteFolderDialog>("Add folder", parameters);
            var result = await dialog.Result;
            if (result?.Data is TestSuiteFolder folder)
            {
                if (_selectedTreeItem?.Folder is not null)
                {
                    AddFolderToTreeView(folder, _selectedTreeItem.Folder, _rootItems);
                }
                if (_selectedTreeItem?.TestCase is not null)
                {
                    // Todo, find folder
                }
            }
        }
        else if (Project is not null && testSuite is not null)
        {
            var parameters = new DialogParameters<AddTestSuiteFolderDialog>
            {
                { x => x.ProjectId, Project.Id },
                { x => x.TestSuiteId, testSuite.Id },
            };
            var dialog = await dialogService.ShowAsync<AddTestSuiteFolderDialog>("Add folder", parameters);
            var result = await dialog.Result;
            if (result?.Data is TestSuiteFolder folder)
            {
                AddFolderToTreeView(folder, testSuite, _rootItems);
            }
        }
    }


    private void AddFolderToTreeView(TestSuiteFolder folder, TestSuite parent, IEnumerable<TreeItemData<BrowserItem>> items)
    {
        foreach (var item in items)
        {
            if (item.Value?.TestSuite is not null && item.Value.TestSuite.Id == parent.Id)
            {
                item.Expanded = true;
                item.Children ??= new();
                item.Children.Add(testBrowser.CreateTreeItemDataFromFolder(folder));
                return;
            }
            else if (item.Children is not null)
            {
                AddFolderToTreeView(folder, parent, item.Children);
            }
        }
    }
    private void AddFolderToTreeView(TestSuiteFolder folder, TestSuiteFolder parent, IEnumerable<TreeItemData<BrowserItem>> items)
    {
        foreach (var item in items)
        {
            if (item.Value?.Folder is not null && item.Value.Folder.Id == parent.Id)
            {
                item.Expanded = true;
                item.Children ??= new();
                item.Children.Add(testBrowser.CreateTreeItemDataFromFolder(folder));
                return;
            }
            else if (item.Children is not null)
            {
                AddFolderToTreeView(folder, parent, item.Children);
            }
        }
    }

    private async Task DeleteTestRunAsync(TestRun? run)
    {
        if (run is not null)
        {
            await testCaseEditor.DeleteTestRunByIdAsync(run.Id);
            RemoveTreeItemData(x => x.TestRun?.Id == run.Id);
        }
    }

    private async Task DeleteTestSuiteFolderAsync(TestSuiteFolder? folder)
    {
        if (folder is not null)
        {
            await testSuiteService.DeleteFolderByIdAsync(folder.Id);
            RemoveTreeItemData(x => x.Folder?.Id == folder.Id);
        }
    }
    private async Task DeleteTestSuiteAsync(TestSuite? testSuite)
    {
        if (testSuite is not null)
        {
            await testSuiteService.DeleteTestSuiteByIdAsync(testSuite.Id);
        }
    }

    private async Task AddTestSuiteAsync()
    {
        TestSuite? testSuite = await testBrowser.AddTestSuiteAsync(Team, Project);
        if (testSuite is not null)
        {
            _rootItems = await testBrowser.BrowseAsync(Team?.Id, Project?.Id, null);
        }
    }

    #region Lifecycle
    protected override void OnInitialized()
    {
        testCaseEditor.AddObserver(this);
    }

    public void Dispose()
    {
        testCaseEditor.RemoveObserver(this);
    }
    #endregion

    public async Task OnTestSavedAsync(TestCase testCase)
    {
        await InvokeAsync(() =>
        {
            var node = FindTreeItemData(x => x.TestCase?.Id == testCase?.Id);
            if (node?.Value is not null)
            {
                node.Value.TestCase = testCase;
                this.StateHasChanged();
            }
        });
    }

    /// <summary>
    /// Invoked when a test has been deleted
    /// </summary>
    /// <param name="testCase"></param>
    /// <returns></returns>
    public async Task OnTestDeletedAsync(TestCase testCase)
    {
        await InvokeAsync(() =>
        {
            RemoveTreeItemData(x => x.TestCase == testCase);
            this.StateHasChanged();
        });
    }

    public Task OnTestCreatedAsync(TestCase testCase)
    {
        if (testCase.TestSuiteFolderId is null)
        {
            // Root test
            var testSuiteNode = FindTreeItemData(x => x.TestSuite?.Id == testCase.TestSuiteId);
            if (testSuiteNode?.Children is not null)
            {
                testSuiteNode.Children.Add(testBrowser.CreateTreeItemDataFromTestCase(testCase));
                this.StateHasChanged();
            }
        }
        else if(testCase.TestSuiteFolderId is not null)
        {
            // Added within a folder
            var folder = FindTreeItemData(x => x.Folder?.Id == testCase.TestSuiteFolderId);
            if (folder?.Children is not null)
            {
                folder.Children.Add(testBrowser.CreateTreeItemDataFromTestCase(testCase));
                this.StateHasChanged();
            }
        }
        return Task.CompletedTask;
    }

    private async Task EditTestCaseAutomationLinkAsync(TestCase? testCase)
    {
        if (testCase is null)
        {
            return;
        }
        await testCaseEditor.EditTestCaseAutomationLinkAsync(testCase);
    }
    private async Task DeleteTestCaseAsync(TestCase? testCase)
    {
        if (testCase is null)
        {
            return;
        }
        await testCaseEditor.DeleteTestCaseAsync(testCase);
    }

}
