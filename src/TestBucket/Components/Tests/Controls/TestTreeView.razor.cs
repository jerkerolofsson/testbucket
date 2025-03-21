using TestBucket.Components.Shared;
using TestBucket.Components.Shared.Tree;
using TestBucket.Components.Tests.Dialogs;
using TestBucket.Domain.Requirements.Models;
using TestBucket.Domain.Teams.Models;
using TestBucket.Domain.Testing;
using TestBucket.Domain.Testing.Models;

using static MudBlazor.CategoryTypes;

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

    [Parameter] public TestProject? Project { get; set; }

    [Parameter] public Team? Team { get; set; }
    #endregion

    private TestProject? _project;
    private Team? _team;

    /// <summary>
    /// Instance of the tree view UI 
    /// </summary>
    private TreeView<BrowserItem> _treeView = default!;
    //private TreeView<BrowserItem> _treeView2 = default!;

    /// <summary>
    /// Root items in the tree view
    /// </summary>
    private List<TreeNode<BrowserItem>> _rootItems = [];

    /// <summary>
    /// currently selected item
    /// </summary>
    private BrowserItem? _selectedTreeItem;
    private string _searchText = "";

    private async Task OnSearchTextChangedAsync(string text)
    {
        this.StateHasChanged();

        _searchText = text;
        if (string.IsNullOrWhiteSpace(_searchText))
        {
            _rootItems = await testBrowser.BrowseAsync(_team?.Id, _project?.Id, null, null);
        }
        else
        {
            _rootItems = await testBrowser.BrowseAsync(_team?.Id, _project?.Id, null, _searchText);
        }
    }
    private TreeNode<BrowserItem>? RemoveTreeNode(Predicate<BrowserItem> predicate)
    {
        return RemoveTreeNode(null, _rootItems, predicate);
    }

    private TreeNode<BrowserItem>? RemoveTreeNode(TreeNode<BrowserItem>? parentNode, IEnumerable<TreeNode<BrowserItem>> treeItems, Predicate<BrowserItem> predicate)
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
                var match = RemoveTreeNode(node, node.Children, predicate);
                if (match is not null)
                {
                    return match;
                }
            }
        }
        return null;
    }

    private TreeNode<BrowserItem>? FindTreeNode(Predicate<BrowserItem> predicate)
    {
        return FindTreeNode(_rootItems, predicate);
    }

    private TreeNode<BrowserItem>? FindTreeNode(IEnumerable<TreeNode<BrowserItem>> treeItems, Predicate<BrowserItem> predicate)
    {
        foreach (var node in treeItems)
        {
            if (node.Value is not null && predicate(node.Value))
            {
                return node;
            }
            else if (node.Children is not null)
            {
                var match = FindTreeNode(node.Children, predicate);
                if (match is not null)
                {
                    return match;
                }
            }
        }
        return null;
    }

    private async Task OnDropTestSuiteFolderAsync(TestSuiteFolder folder, TreeNode<BrowserItem> targetNode)
    {
        var target = targetNode?.Value;
        if (target?.Folder is not null && targetNode is not null)
        {
            var targetFolder = target.Folder;
            if(targetFolder.PathIds is null)
            {
                return;
            }

            // Cannot drop a parent to a child
            if(targetFolder.PathIds.Contains(folder.Id))
            {
                return;
            }

            // Find the source
            TreeNode<BrowserItem>? sourceNode = FindTreeNode(x => x.Folder?.Id == folder.Id);
            folder.ParentId = targetFolder.Id;
            folder.TestSuiteId = targetFolder.TestSuiteId;

            await testSuiteService.SaveTestSuiteFolderAsync(folder);

            //    testCase.TestSuiteFolderId = target.Folder.Id;
            //    testCase.TestSuiteId = target.Folder.TestSuiteId;

            //    await testCaseEditor.SaveTestCaseAsync(testCase);

            // Update the UI
            RemoveTreeNode(x => x.Folder?.Id == folder.Id);
            if (targetNode.Children is null)
            {
                targetNode.Children = await testBrowser.BrowseAsync(_team?.Id, _project?.Id, target, null);
            }
            else
            {
                targetNode.Children = [.. targetNode.Children, testBrowser.CreateTreeNodeFromFolder(folder)];
            }
        }
    }

    private async Task OnDrop(TestEntity? testEntity, TreeNode<BrowserItem>? targetNode)
    {
        if (testEntity is null || targetNode is null)
        {
            return;
        }
        var target = targetNode.Value;
        if (target is null)
        {
            return;
        }
        if (testEntity is TestCase testCase)
        {
            await OnDropTestCaseAsync(testCase, targetNode);
        }
        if (testEntity is TestSuiteFolder testSuiteFolder)
        {
            await OnDropTestSuiteFolderAsync(testSuiteFolder, targetNode);
        }
    }

    private async Task OnDropTestCaseAsync(TestCase testCase, TreeNode<BrowserItem> targetNode)
    {
        var target = targetNode?.Value;
        if (target?.Folder is not null && targetNode is not null)
        {
            var fromFolderId = testCase.TestSuiteFolderId;

            // Find the source
            TreeNode<BrowserItem>? sourceNode = FindTreeNode(x => x.Folder?.Id == fromFolderId);

            testCase.TestSuiteFolderId = target.Folder.Id;
            testCase.TestSuiteId = target.Folder.TestSuiteId;

            await testCaseEditor.SaveTestCaseAsync(testCase);

            // Remove the test from the tree
            RemoveTreeNode(x => x.TestCase?.Id == testCase.Id);
            if (targetNode.Children is null)
            { 
                targetNode.Children = await testBrowser.BrowseAsync(_team?.Id, _project?.Id, target, null);
            }
            else
            {
                targetNode.Children = [.. targetNode.Children, testBrowser.CreateTreeNodeFromTestCase(testCase)];
            }
        }
    }

    private async Task EditTestSuiteFolderAsync(TestSuiteFolder? folder)
    {
        if (folder is not null)
        {
            await testBrowser.CustomizeFolderAsync(folder);

            // Refresh tree node
            var treeNode = FindTreeNode(x => x.Folder?.Id == folder.Id);
            if (treeNode?.Value is not null)
            {
                treeNode.Text = folder.Name;
                treeNode.Value.Color = folder.Color;
                treeNode.Value.Icon = folder.Icon;
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
            if (Project is not null && Team is not null)
            {
                _rootItems = await testBrowser.BrowseAsync(_team?.Id, _project?.Id, null, null);
            }
        }
    }


    public async Task<IReadOnlyCollection<TreeNode<BrowserItem>>> LoadServerData(BrowserItem? browserItem)
    {
        // wait 500ms to simulate a server load

        if (_team is null || _project is null)
        {
            return [];
        }

        return await testBrowser.BrowseAsync(_team?.Id, _project?.Id, browserItem, null);
    }

    //private void OnItemsLoaded(TreeNode<BrowserItem> TreeNode, IReadOnlyCollection<TreeNode<BrowserItem>> children)
    //{
    //    // here we store the server-loaded children in the TreeNode so that they are available in the InitialTreeItems
    //    // if you don't do this you loose already loaded children on next render update
    //    TreeNode.Children = children?.ToList();
    //}

    private async Task CreateNewTestCaseAsync(TestSuiteFolder folder)
    {
        await testCaseEditor.CreateNewTestCaseAsync(folder, folder.TestSuiteId);
    }

    private async Task GenerateAiTestsAsync(TestSuiteFolder folder)
    {
        await testCaseEditor.GenerateAiTestsAsync(folder, folder.TestSuiteId);
    }

    private async Task RunAsync(TestSuite testSuite)
    {
        if (testSuite.TestProjectId is null)
        {
            return;
        }
        long[] testCaseIds = await testBrowser.GetTestSuiteSuiteTestsAsync(testSuite);
        var run = await testRunCreation.CreateTestRunAsync(testSuite.TestProjectId.Value, testCaseIds);
        if (run is not null)
        {
            appNavigationManager.NavigateTo(run);
        }
    }
    private async Task RunAsync(TestSuiteFolder folder)
    {
        if (folder.TestProjectId is null)
        {
            return;
        }
        long[] testCaseIds = await testBrowser.GetTestSuiteSuiteFolderTestsAsync(folder, true);
        var run = await testRunCreation.CreateTestRunAsync(folder.TestProjectId.Value, testCaseIds);
        if (run is not null)
        {
            appNavigationManager.NavigateTo(run);
        }
    }
    private async Task RunAsync(TestCase testCase)
    {
        if (testCase.TestProjectId is null)
        {
            return;
        }
        var run = await testRunCreation.CreateTestRunAsync(testCase.TestProjectId.Value, [testCase.Id]);
        if (run is not null)
        {
            appNavigationManager.NavigateTo(run);
        }
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


    private void AddFolderToTreeView(TestSuiteFolder folder, TestSuite parent, IEnumerable<TreeNode<BrowserItem>> items)
    {
        foreach (var item in items)
        {
            if (item.Value?.TestSuite is not null && item.Value.TestSuite.Id == parent.Id)
            {
                item.Expanded = true;

                var childNode = testBrowser.CreateTreeNodeFromFolder(folder);
                if (item.Children is null)
                {
                    item.Children = [childNode];
                }
                else
                {
                    item.Children = [.. item.Children, childNode];
                }

                return;
            }
            else if (item.Children is not null)
            {
                AddFolderToTreeView(folder, parent, item.Children);
            }
        }
    }
    private void AddFolderToTreeView(TestSuiteFolder folder, TestSuiteFolder parent, IEnumerable<TreeNode<BrowserItem>> items)
    {
        foreach (var item in items)
        {
            if (item.Value?.Folder is not null && item.Value.Folder.Id == parent.Id)
            {
                var childNode = testBrowser.CreateTreeNodeFromFolder(folder);
                item.Expanded = true;
                if (item.Children is null)
                {
                    item.Children = [childNode];
                }
                else
                {
                    item.Children = [.. item.Children, childNode];
                }
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
            await testCaseEditor.DeleteTestRunAsync(run);
            RemoveTreeNode(x => x.TestRun?.Id == run.Id);
        }
    }

    private async Task DeleteTestSuiteFolderAsync(TestSuiteFolder? folder)
    {
        if (folder is not null)
        {
            await testSuiteService.DeleteFolderByIdAsync(folder.Id);
            RemoveTreeNode(x => x.Folder?.Id == folder.Id);
            if (_selectedTreeItem?.Folder?.Id == folder.Id)
            {
                _selectedTreeItem = null;
            }
        }
    }
    private async Task DeleteTestSuiteAsync(TestSuite? testSuite)
    {
        if (testSuite is not null)
        {
            await testSuiteService.DeleteTestSuiteByIdAsync(testSuite.Id);
            RemoveTreeNode(x => x.TestSuite?.Id == testSuite.Id);
            if (_selectedTreeItem?.TestSuite?.Id ==testSuite.Id)
            {
                _selectedTreeItem = null;
            }
        }
    }

    private async Task AddTestSuiteAsync()
    {
        TestSuite? testSuite = await testBrowser.AddTestSuiteAsync(Team, Project);
        if (testSuite is not null)
        {
            _rootItems = await testBrowser.BrowseAsync(Team?.Id, Project?.Id, null, null);
        }
    }

    #region Lifecycle
    protected override void OnInitialized()
    {
        testCaseManager.AddObserver(this);
    }

    public void Dispose()
    {
        testCaseManager.RemoveObserver(this);
    }
    #endregion

    public async Task OnTestSavedAsync(TestCase testCase)
    {
        await InvokeAsync(() =>
        {
            var node = FindTreeNode(x => x.TestCase?.Id == testCase?.Id);
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
            RemoveTreeNode(x => x.TestCase == testCase);
            this.StateHasChanged();
        });
    }

    public async Task OnTestCreatedAsync(TestCase testCase)
    {
        if (testCase.TestSuiteFolderId is null)
        {
            // Root test
            var testSuiteNode = FindTreeNode(x => x.TestSuite?.Id == testCase.TestSuiteId);
            if (testSuiteNode?.Children is not null)
            {
                var childNode = testBrowser.CreateTreeNodeFromTestCase(testCase);
                if (testSuiteNode.Children is null)
                {
                    testSuiteNode.Children = [childNode];
                }
                else
                {
                    testSuiteNode.Children = [.. testSuiteNode.Children, childNode];
                }
                await InvokeAsync(StateHasChanged);
            }
        }
        else if(testCase.TestSuiteFolderId is not null)
        {
            // Added within a folder
            var folder = FindTreeNode(x => x.Folder?.Id == testCase.TestSuiteFolderId);
            if (folder?.Children is not null)
            {
                var childNode = testBrowser.CreateTreeNodeFromTestCase(testCase);
                if (folder.Children is null)
                {
                    folder.Children = [childNode];
                }
                else
                {
                    folder.Children = [.. folder.Children, childNode];
                }
                await InvokeAsync(StateHasChanged);
            }
        }
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
