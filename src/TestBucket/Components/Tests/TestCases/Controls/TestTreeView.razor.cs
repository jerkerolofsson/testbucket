using TestBucket.Components.Shared;
using TestBucket.Components.Shared.Tree;
using TestBucket.Components.Tests.Services;
using TestBucket.Domain.Automation.Models;
using TestBucket.Domain.Teams.Models;

namespace TestBucket.Components.Tests.TestCases.Controls;

public partial class TestTreeView
{
    #region Parameters
    [Parameter] public bool ShowTestCases { get; set; } = true;
    [Parameter] public bool ShowTestRuns { get; set; } = true;
    [Parameter] public bool ShowTestSuites { get; set; } = true;
    [Parameter] public bool ShowTestRunPipelines { get; set; } = true;
    [Parameter] public bool ShowTestRunTests { get; set; } = true;

    /// <summary>
    /// Invoked when a folder is clicked
    /// </summary>
    [Parameter] public EventCallback<TestSuiteFolder> OnFolderClicked { get; set; }

    /// <summary>
    /// Invoked when the tests virtual folder is clicked for a test run
    /// </summary>
    [Parameter] public EventCallback<TestRun> OnTestRunTestsFolderClicked { get; set; }

    /// <summary>
    /// Invoked when a pipeline is clicked
    /// </summary>
    [Parameter] public EventCallback<Pipeline> OnPipelineClicked { get; set; }

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

    private TestBrowserRequest CreateRequest()
    {
        var text = _searchText;
        if (string.IsNullOrEmpty(text))
        {
            text = null;
        }
        return new TestBrowserRequest
        {
            TeamId = _team?.Id,
            ProjectId = _project?.Id,
            Text = text,
            ShowTestCases = ShowTestCases,
            ShowTestRuns = ShowTestRuns,
            ShowTestSuites = ShowTestSuites,
            ShowTestRunPipelines = ShowTestRunPipelines,
            ShowTestRunTests = ShowTestRunTests,
        };
    }

    private async Task OnSearchTextChangedAsync(string text)
    {
        this.StateHasChanged();

        _searchText = text;
        _rootItems = await testBrowser.BrowseAsync(CreateRequest());
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
        return TestBrowser.FindTreeNode(_rootItems, predicate);
    }


    private async Task OnDropTestSuiteFolderAsync(TestSuiteFolder folder, TreeNode<BrowserItem> targetNode)
    {
        var target = targetNode?.Value;
        if (target?.Folder is not null && targetNode is not null)
        {
            var targetFolder = target.Folder;
            if (targetFolder.PathIds is null)
            {
                return;
            }

            // Cannot drop a parent to a child
            if (targetFolder.PathIds.Contains(folder.Id))
            {
                return;
            }

            // Find the source
            TreeNode<BrowserItem>? sourceNode = FindTreeNode(x => x.Folder?.Id == folder.Id);
            folder.ParentId = targetFolder.Id;
            folder.TestSuiteId = targetFolder.TestSuiteId;

            await testSuiteService.SaveTestSuiteFolderAsync(folder);

            // Update the UI
            RemoveTreeNode(x => x.Folder?.Id == folder.Id);
            if (targetNode.Children is null)
            {
                var request = CreateRequest();
                request.Parent = target;
                targetNode.Children = await testBrowser.BrowseAsync(request);
            }
            else
            {
                targetNode.Children = [.. targetNode.Children, testBrowser.CreateTreeNodeFromFolder(folder)];
            }
        }
        else if (target?.TestSuite is not null && targetNode is not null)
        {
            // Find the source
            TreeNode<BrowserItem>? sourceNode = FindTreeNode(x => x.Folder?.Id == folder.Id);
            folder.ParentId = null;
            folder.TestSuiteId = target.TestSuite.Id;

            await testSuiteService.SaveTestSuiteFolderAsync(folder);

            // Update the UI
            RemoveTreeNode(x => x.Folder?.Id == folder.Id);
            if (targetNode.Children is null)
            {
                var request = CreateRequest();
                request.Parent = target;
                targetNode.Children = await testBrowser.BrowseAsync(request);
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
                var request = CreateRequest();
                request.Parent = target;
                targetNode.Children = await testBrowser.BrowseAsync(request);
            }
            else
            {
                targetNode.Children = [.. targetNode.Children, testBrowser.CreateTreeNodeFromTestCase(testCase)];
            }
        }
        else if (target?.TestSuite is not null && targetNode is not null)
        {
            var fromFolderId = testCase.TestSuiteFolderId;

            // Find the source
            TreeNode<BrowserItem>? sourceNode = FindTreeNode(x => x.Folder?.Id == fromFolderId);

            testCase.TestSuiteFolderId = null;
            testCase.TestSuiteId = target.TestSuite.Id;

            await testCaseEditor.SaveTestCaseAsync(testCase);

            // Remove the test from the tree
            RemoveTreeNode(x => x.TestCase?.Id == testCase.Id);
            if (targetNode.Children is null)
            {
                var request = CreateRequest();
                request.Parent = target;
                targetNode.Children = await testBrowser.BrowseAsync(request);
            }
            else
            {
                targetNode.Children = [.. targetNode.Children, testBrowser.CreateTreeNodeFromTestCase(testCase)];
            }
        }
    }

    private async Task ImportAsync()
    {
        if (_team is null)
        {
            return;
        }
        if (_project is null)
        {
            return;
        }
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
            else if (item.TestRun is not null && item.VirtualFolderName is null)
            {
                await OnTestRunClicked.InvokeAsync(item.TestRun);
            }
            else if (item.Pipeline is not null)
            {
                await OnPipelineClicked.InvokeAsync(item.Pipeline);
            }
            else if (item.TestRun is not null && item.VirtualFolderName == TestBrowser.FOLDER_RUN_TESTS)
            {
                await OnTestRunTestsFolderClicked.InvokeAsync(item.TestRun);
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
            if (Project is not null)
            {
                var request = CreateRequest();
                _rootItems = await testBrowser.BrowseAsync(request);
            }
        }
    }

    public async Task<IReadOnlyCollection<TreeNode<BrowserItem>>> LoadServerData(BrowserItem? browserItem)
    {
        // wait 500ms to simulate a server load

        if (_project is null)
        {
            return [];
        }

        var request = CreateRequest();
        request.Parent = browserItem;
        return await testBrowser.BrowseAsync(request);
    }

    //private void OnItemsLoaded(TreeNode<BrowserItem> TreeNode, IReadOnlyCollection<TreeNode<BrowserItem>> children)
    //{
    //    // here we store the server-loaded children in the TreeNode so that they are available in the InitialTreeItems
    //    // if you don't do this you loose already loaded children on next render update
    //    TreeNode.Children = children?.ToList();
    //}

    private async Task AddTestSuiteAsync()
    {
        await testBrowser.AddTestSuiteAsync(Team, Project);
    }

    #region Lifecycle
    protected override void OnInitialized()
    {
        testCaseManager.AddObserver(this);
        testRunManager.AddObserver(this);
        testSuiteManager.AddObserver(this);
        testSuiteManager.AddFolderObserver(this);
    }

    public void Dispose()
    {
        testCaseManager.RemoveObserver(this);
        testRunManager.RemoveObserver(this);
        testSuiteManager.RemoveObserver(this);
        testSuiteManager.RemoveFolderObserver(this);
    }
    #endregion

    #region Test Observer
    public async Task OnTestCreatedAsync(TestCase testCase)
    {
        if (testCase.TestSuiteFolderId is null)
        {
            // The test case is in the root of a test suite
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
        else if (testCase.TestSuiteFolderId is not null)
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
    public async Task OnTestSavedAsync(TestCase testCase)
    {
        await InvokeAsync(() =>
        {
            var node = FindTreeNode(x => x.TestCase?.Id == testCase?.Id);
            if (node?.Value is not null)
            {
                node.Text = testCase.Name;
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
    #endregion Test Observer

    #region Folder Observer
    public async Task OnTestSuiteFolderCreatedAsync(TestSuiteFolder suiteFolder)
    {
        await InvokeAsync(() =>
        {
            if (suiteFolder.ParentId is not null)
            {
                var parentNode = FindTreeNode(x => x.Folder?.Id == suiteFolder.ParentId);
                if (parentNode?.Children is not null)
                {
                    var node = testBrowser.CreateTreeNodeFromFolder(suiteFolder);
                    parentNode.Children = [.. parentNode.Children, node];
                    this.StateHasChanged();
                }
            }
            else
            {
                var parentNode = FindTreeNode(x => x.TestSuite?.Id == suiteFolder.TestSuiteId);
                if (parentNode?.Children is not null)
                {
                    var node = testBrowser.CreateTreeNodeFromFolder(suiteFolder);
                    parentNode.Children = [.. parentNode.Children, node];
                    this.StateHasChanged();
                }
            }
        });
    }
    public async Task OnTestSuiteFolderDeletedAsync(TestSuiteFolder suiteFolder)
    {
        await InvokeAsync(() =>
        {
            RemoveTreeNode(x => x.Folder?.Id == suiteFolder.Id);
            this.StateHasChanged();
        });
    }
    public async Task OnTestSuiteFolderSavedAsync(TestSuiteFolder suiteFolder)
    {
        await InvokeAsync(() =>
        {
            var node = FindTreeNode(x => x.Folder?.Id == suiteFolder.Id);
            if (node is not null)
            {
                node.Text = suiteFolder.Name;
                node.Icon = testBrowser.GetIcon(suiteFolder);
                if (node.Value is not null)
                {
                    node.Value.Color = suiteFolder.Color;
                }
                this.StateHasChanged();
            }
        });
    }

    #endregion

    #region Run Observer

    public Task OnTestCaseRunCreatedAsync(TestCaseRun testCaseRun)
    {
        return Task.CompletedTask;
    }
    public Task OnTestCaseRunUpdatedAsync(TestCaseRun testCaseRun)
    {
        return Task.CompletedTask;
    }

    public async Task OnTestCaseRunDeletedAsync(TestCaseRun testCaseRun)
    {
        await InvokeAsync(() =>
        {
            //RemoveTreeNode(x => x.TestCaseRun?.Id == testCaseRun.Id);
            this.StateHasChanged();
        });
    }
    public async Task OnRunUpdatedAsync(TestRun testRun)
    {
        await InvokeAsync(() =>
        {
            var testrunsNode = FindTreeNode(x => x.TestRun?.Id == testRun.Id);
            if (testrunsNode?.Value is not null)
            {
                testrunsNode.Text = testRun.Name;
                testrunsNode.Value.TestRun = testRun;
                this.StateHasChanged();
            }
        });
    }
    public async Task OnRunCreatedAsync(TestRun testRun)
    {
        await InvokeAsync(() =>
        {
            var testrunsNode = FindTreeNode(x => x.VirtualFolderName == TestBrowser.ROOT_TEST_RUNS);
            if (testrunsNode is not null)
            {
                var childNode = testBrowser.CreateTestRunTreeNode(CreateRequest(), testRun);
                testrunsNode.Expanded = true;
                if (testrunsNode.Children is null)
                {
                    testrunsNode.Children = [childNode];
                }
                else
                {
                    testrunsNode.Children = [.. testrunsNode.Children, childNode];
                }
                return;
            }
            this.StateHasChanged();
        });
    }
    public async Task OnRunDeletedAsync(TestRun testRun)
    {
        await InvokeAsync(() =>
        {
            RemoveTreeNode(x => x.TestRun?.Id == testRun.Id);
            this.StateHasChanged();
        });
    }

    #endregion

    #region Test Suite Observer

    public async Task OnTestSuiteCreatedAsync(TestSuite suite)
    {
        await InvokeAsync(() =>
        {
            var parentNode = FindTreeNode(x => x.VirtualFolderName == "TestSuites");
            if (parentNode?.Children is not null)
            {
                var node = testBrowser.CreateTestSuiteTreeNode(suite);
                parentNode.Children = [.. parentNode.Children, node];
                this.StateHasChanged();
            }
        });
    }
    public async Task OnTestSuiteDeletedAsync(TestSuite suite)
    {
        await InvokeAsync(() =>
        {
            RemoveTreeNode(x => x.TestSuite?.Id == suite.Id);
            this.StateHasChanged();
        });
    }
    public async Task OnTestSuiteSavedAsync(TestSuite suite)
    {
        await InvokeAsync(() =>
        {
            var node = FindTreeNode(x => x.TestSuite?.Id == suite.Id);
            if (node is not null)
            {
                node.Text = suite.Name;
                node.Icon = testBrowser.GetIcon(suite);
                if (node.Value is not null)
                {
                    node.Value.Color = suite.Color;
                }
                this.StateHasChanged();
            }
        });
    }


    #endregion

    #region Rename

    private TreeNode<BrowserItem>? _editItem;

    private void BeginRename(TreeNode<BrowserItem> item)
    {
        _editItem = item;
    }

    private void OnItemEditCanceled()
    {
        _editItem = null;
    }

    private async Task OnRenamed(string text)
    {
        if (_editItem is not null)
        {
            if (_editItem.Value?.TestCase is not null)
            {
                _editItem.Text = text;
                _editItem.Value.TestCase.Name = text;
                await testCaseEditor.SaveTestCaseAsync(_editItem.Value.TestCase);
            }
            if (_editItem.Value?.TestSuite is not null)
            {
                _editItem.Text = text;
                _editItem.Value.TestSuite.Name = text;
                await testSuiteService.SaveTestSuiteAsync(_editItem.Value.TestSuite);
            }
            if (_editItem.Value?.Folder is not null)
            {
                _editItem.Text = text;
                _editItem.Value.Folder.Name = text;
                await testSuiteService.SaveTestSuiteFolderAsync(_editItem.Value.Folder);
            }
            if (_editItem.Value?.TestRun is not null)
            {
                _editItem.Text = text;
                _editItem.Value.TestRun.Name = text;
                await testCaseEditor.SaveTestRunAsync(_editItem.Value.TestRun);
                //await testCaseEditor.saveTest(_editItem.Value.Folder);
            }
        }
        _editItem = null;
    }

    internal async Task SyncWithActiveDocumentAsync()
    {
        if (appNavigationManager.State.SelectedTestCase is not null)
        {
            await GoToTestCaseAsync(appNavigationManager.State.SelectedTestCase);
        }
    }

    internal async Task GoToTestCaseAsync(TestCase testCase, bool invokeStateHasChanged = true)
    {
        // Find the test suite node
        var testSuiteNode = FindTreeNode(x => x.TestSuite?.Id == testCase.TestSuiteId);
        if (testSuiteNode is null)
        {
            return;
        }
        testSuiteNode.Expanded = true;

        // Next traverse all folders
        if (testCase.PathIds is not null)
        {
            TreeNode<BrowserItem> parent = testSuiteNode;
            foreach (var folderId in testCase.PathIds)
            {
                var folderNode = FindTreeNode(x => x.Folder?.Id == folderId);
                if (folderNode is null)
                {
                    var request = CreateRequest();
                    request.Parent = parent.Value;

                    var items = await testBrowser.BrowseAsync(request);
                    parent.Children = items;
                    folderNode = FindTreeNode(x => x.Folder?.Id == folderId);
                }
                if (folderNode is null)
                {
                    return;
                }
                folderNode.Expanded = true;
                parent = folderNode;

                // Browse items in the folder, this will include tests, if any..
                if (folderNode.Children is null)
                {
                    var request = CreateRequest();
                    request.Parent = folderNode.Value;

                    var items = await testBrowser.BrowseAsync(request);
                    folderNode.Children = items;
                }
            }
        }

        var testCaseTreeNode = FindTreeNode(x => x.TestCase?.Id == testCase.Id);
        if (testCaseTreeNode is not null)
        {
            _selectedTreeItem = testCaseTreeNode?.Value;
        }
        if (invokeStateHasChanged)
        {
            await InvokeAsync(StateHasChanged);
        }
    }

    #endregion

}
