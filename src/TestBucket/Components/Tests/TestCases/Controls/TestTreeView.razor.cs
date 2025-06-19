using TestBucket.Components.Shared.Tree;
using TestBucket.Components.Tests.Services;
using TestBucket.Components.Tests.TestRuns.Controllers;
using TestBucket.Domain.Automation.Pipelines.Models;
using TestBucket.Domain.Testing.Models;


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
    /// Invoked when something was clicked which is not handled by other event callbacks
    /// </summary>
    [Parameter] public EventCallback OnOtherClicked { get; set; }

    /// <summary>
    /// Invoked when a test lab folder is clicked
    /// </summary>
    [Parameter] public EventCallback<TestLabFolder> OnTestLabFolderClicked { get; set; }

    /// <summary>
    /// Invoked when a test repo folder is clicked
    /// </summary>
    [Parameter] public EventCallback<TestRepositoryFolder> OnTestRepositoryFolderClicked { get; set; }

    /// <summary>
    /// Invoked when a test repository node is clicked
    /// </summary>
    [Parameter] public EventCallback OnTestRepositoryClicked { get; set; }

    /// <summary>
    /// Invoked when a test lab node is clicked
    /// </summary>
    [Parameter] public EventCallback OnTestLabClicked { get; set; }

    /// <summary>
    /// Invoked when a test suite folder is clicked
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

    private bool _multiSelect = false;

    /// <summary>
    /// Instance of the tree view UI 
    /// </summary>
    private TreeView<BrowserItem> _treeView = default!;

    /// <summary>
    /// Root items in the tree view
    /// </summary>
    private List<TreeNode<BrowserItem>> _rootItems = [];

    /// <summary>
    /// All selected nodes
    /// </summary>
    private List<BrowserItem> _selectedValues = [];

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

    private async Task OnDropTestSuiteFolderAsync(TestSuiteFolder folder, TreeNode<BrowserItem> targetNode)
    {
        var target = targetNode?.Value;

        if (target?.Folder is not null && targetNode is not null)
        {
            var targetFolder = target.Folder;

            if(targetFolder.Id == folder.Id)
            {
                // Drop on self
                return;
            }

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
            //TreeNode<BrowserItem>? sourceNode = FindTreeNode(x => x.Folder?.Id == folder.Id);
            folder.ParentId = targetFolder.Id;
            folder.TestSuiteId = targetFolder.TestSuiteId;

            await testSuiteController.SaveTestSuiteFolderAsync(folder);

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
            //TreeNode<BrowserItem>? sourceNode = FindTreeNode(x => x.Folder?.Id == folder.Id);
            folder.ParentId = null;
            folder.TestSuiteId = target.TestSuite.Id;

            await testSuiteController.SaveTestSuiteFolderAsync(folder);

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
        else if (testEntity is TestSuiteFolder testSuiteFolder)
        {
            await OnDropTestSuiteFolderAsync(testSuiteFolder, targetNode);
        }
        else if (testEntity is TestSuite testSuite)
        {
            await OnDropTestSuiteAsync(testSuite, targetNode);
        }
        else if (testEntity is TestRun testRun)
        {
            await OnDropTestRunAsync(testRun, targetNode);
        }
        else if (testEntity is TestRepositoryFolder testRepositoryFolder)
        {
            await OnDropTestRepositoryFolderAsync(testRepositoryFolder, targetNode);
        }
        else if (testEntity is TestLabFolder testLabFolder)
        {
            await OnDropTestLabFolderAsync(testLabFolder, targetNode);
        }
    }

    private async Task OnDropTestRepositoryFolderAsync(TestRepositoryFolder testRepositoryFolder, TreeNode<BrowserItem> targetNode)
    {
        if (targetNode.Value?.VirtualFolderName == TestBrowser.ROOT_TEST_REPOSITORY)
        {
            // Drop in root
            if (testRepositoryFolder.ParentId is not null)
            {
                testRepositoryFolder.ParentId = null;
                await testBrowser.UpdateTestRepositoryFolderAsync(testRepositoryFolder);
            }
        }
        if (targetNode.Value?.TestRepositoryFolder is not null)
        {
            if(targetNode.Value.TestRepositoryFolder.Id == testRepositoryFolder.Id)
            {
                // Drop on self
                return;
            }
            testRepositoryFolder.ParentId = targetNode.Value.TestRepositoryFolder.Id;
            await testBrowser.UpdateTestRepositoryFolderAsync(testRepositoryFolder);
        }
    }
    private async Task OnDropTestLabFolderAsync(TestLabFolder folder, TreeNode<BrowserItem> targetNode)
    {
        if (targetNode.Value?.VirtualFolderName == TestBrowser.ROOT_TEST_LAB)
        {
            // Drop in root
            if (folder.ParentId is not null)
            {
                folder.ParentId = null;
                await testBrowser.UpdateTestLabFolderAsync(folder);
            }
        }
        if (targetNode.Value?.TestLabFolder is not null)
        {
            if (targetNode.Value.TestLabFolder.Id == folder.Id)
            {
                // Drop on self
                return;
            }
            folder.ParentId = targetNode.Value.TestLabFolder.Id;
            await testBrowser.UpdateTestLabFolderAsync(folder);
        }
    }

    private async Task OnDropTestSuiteAsync(TestSuite testSuite, TreeNode<BrowserItem> targetNode)
    {
        if (targetNode.Value?.VirtualFolderName == TestBrowser.ROOT_TEST_REPOSITORY)
        {
            // Drop in root
            if (testSuite.FolderId is not null)
            {
                testSuite.FolderId = null;
                await testSuiteController.SaveTestSuiteAsync(testSuite);
            }
        }
        else if (targetNode.Value?.TestRepositoryFolder is not null)
        {
            if (testSuite.FolderId == targetNode.Value.TestRepositoryFolder.Id)
            {
                // Same
                return;
            }
            testSuite.FolderId = targetNode.Value.TestRepositoryFolder.Id;
            await testSuiteController.SaveTestSuiteAsync(testSuite);
        }
    }
    private async Task OnDropTestRunAsync(TestRun testRun, TreeNode<BrowserItem> targetNode)
    {
        if (targetNode.Value?.VirtualFolderName == TestBrowser.ROOT_TEST_LAB)
        {
            // Drop in root
            if (testRun.FolderId is not null)
            {
                testRun.FolderId = null;
                await testRunController.SaveTestRunAsync(testRun);
            }
        }
        else if (targetNode.Value?.TestLabFolder is not null)
        {
            if (testRun.FolderId == targetNode.Value.TestLabFolder.Id)
            {
                // Same
                return;
            }
            testRun.FolderId = targetNode.Value.TestLabFolder.Id;
            await testRunController.SaveTestRunAsync(testRun);
        }
    }

    private async Task OnDropTestCaseAsync(TestCase testCase, TreeNode<BrowserItem> targetNode)
    {
        var target = targetNode?.Value;
        if (target?.Folder is not null && targetNode is not null)
        {
            var fromFolderId = testCase.TestSuiteFolderId;

            // Find the source
            //TreeNode<BrowserItem>? sourceNode = FindTreeNode(x => x.Folder?.Id == fromFolderId);

            testCase.TestSuiteFolderId = target.Folder.Id;
            testCase.TestSuiteId = target.Folder.TestSuiteId;

            await testCaseEditor.SaveTestCaseAsync(testCase);

            // Remove the tree-node from the tree and add it to the new location
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
            //TreeNode<BrowserItem>? sourceNode = FindTreeNode(x => x.Folder?.Id == fromFolderId);

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

    private void OnSelectedValuesChanged(IReadOnlyCollection<BrowserItem>? values)
    {
        if (values is null)
        {
            _selectedValues.Clear();
        }
        else
        {
            _selectedValues = values.ToList();
        }
        appNavigationManager.State.SetMultiSelectedTestCases(_selectedValues.Select(x => x.TestCase!).Where(x => x is not null).ToList());
        appNavigationManager.State.SetMultiSelectedTestSuites(_selectedValues.Select(x => x.TestSuite!).Where(x => x is not null).ToList());
        appNavigationManager.State.SetMultiSelectedTestSuiteFolders(_selectedValues.Select(x => x.Folder!).Where(x => x is not null).ToList());
    }

    private async Task OnSelectedValueChangedAsync(BrowserItem? item)
    {
        _selectedTreeItem = item;
        if (item is not null)
        {
            if (item.Href is not null)
            {
                appNavigationManager.NavigateTo(item.Href);
                return;
            }

            if (item.Folder is not null)
            {
                await OnFolderClicked.InvokeAsync(item.Folder);
            }
            else if (item.TestRepositoryFolder is not null)
            {
                await OnTestRepositoryFolderClicked.InvokeAsync(item.TestRepositoryFolder);
            }
            else if (item.TestLabFolder is not null)
            {
                await OnTestLabFolderClicked.InvokeAsync(item.TestLabFolder);
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
            else if (item.VirtualFolderName == TestBrowser.ROOT_TEST_LAB)
            {
                await OnTestLabClicked.InvokeAsync();
            }
            else if (item.VirtualFolderName == TestBrowser.ROOT_TEST_REPOSITORY)
            {
                await OnTestRepositoryClicked.InvokeAsync();
            }
            else
            {
                await OnOtherClicked.InvokeAsync();
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


    #region Lifecycle
    protected override void OnInitialized()
    {
        testCaseManager.AddObserver(this);
        testRunManager.AddObserver(this);
        testSuiteManager.AddObserver(this);
        testSuiteManager.AddFolderObserver(this);
        testRepositoryManager.AddObserver(this);
        testLabManager.AddObserver(this);
    }

    public void Dispose()
    {
        testRepositoryManager.RemoveObserver(this);
        testLabManager.RemoveObserver(this);
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
                node.Icon = TestIcons.GetIcon(suiteFolder);
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

    private async Task AddRunToTreeViewAsync(TestRun testRun)
    {
        if (testRun.FolderId is null)
        {
            var testrunsNode = FindTreeNode(x => x.VirtualFolderName == TestBrowser.ROOT_RECENT_TEST_RUNS);
            if (testrunsNode is not null)
            {
                var childNode = await testBrowser.CreateTestRunTreeNode(CreateRequest(), testRun);
                testrunsNode.Expanded = true;
                if (testrunsNode.Children is null)
                {
                    testrunsNode.Children = [childNode];
                }
                else
                {
                    testrunsNode.Children = [.. testrunsNode.Children, childNode];
                }
            }
        }
        else
        {
            var parentNode = FindTreeNode(x => x.TestLabFolder?.Id == testRun.FolderId);
            if (parentNode?.Children is not null)
            {
                var node = await testBrowser.CreateTestRunTreeNode(CreateRequest(), testRun);
                parentNode.Children = [.. parentNode.Children, node];
            }
        }
    }

    public async Task OnRunMovedAsync(TestRun testRun)
    {
        await InvokeAsync(async () =>
        {
            // Todo: Don't delete if moved from recent
            RemoveTreeNode(x => x.TestRun?.Id == testRun.Id);

            //RemoveRunFromTreeView(testRun);
            await AddRunToTreeViewAsync(testRun);
            this.StateHasChanged();
        });
    }
    public async Task OnRunCreatedAsync(TestRun testRun)
    {
        await InvokeAsync(async () =>
        {
            await AddRunToTreeViewAsync(testRun);
            this.StateHasChanged();
        });
    }

    private void RemoveRunFromTreeView(TestRun testRun)
    {
        RemoveTreeNode(x => x.TestRun?.Id == testRun.Id);
    }
    public async Task OnRunDeletedAsync(TestRun testRun)
    {
        await InvokeAsync(() =>
        {
            RemoveRunFromTreeView(testRun);
            this.StateHasChanged();
        });
    }

    #endregion

    #region Test Repository Observer
    private void AddRepositoryFolderToTreeView(TestRepositoryFolder folder)
    {
        if (folder.ParentId is null)
        {
            var rootNode = FindTreeNode(x => x.VirtualFolderName == TestBrowser.ROOT_TEST_REPOSITORY);
            if (rootNode is not null)
            {
                var childNode = testBrowser.CreateRepositoryFolderTreeNode(folder);
                rootNode.Expanded = true;
                if (rootNode.Children is null)
                {
                    rootNode.Children = [childNode];
                }
                else
                {
                    rootNode.Children = [.. rootNode.Children, childNode];
                }
            }
        }
        else
        {
            var parentNode = FindTreeNode(x => x.TestRepositoryFolder?.Id == folder.ParentId);
            if (parentNode?.Children is not null)
            {
                var childNode = testBrowser.CreateRepositoryFolderTreeNode(folder);
                parentNode.Children = [.. parentNode.Children, childNode];
            }
        }
    }
    public async Task OnTestRepositoryFolderCreatedAsync(TestRepositoryFolder folder)
    {
        await InvokeAsync(async () =>
        {
            AddRepositoryFolderToTreeView(folder);
            await InvokeAsync(StateHasChanged);
        });
    }
    public async Task OnTestRepositoryFolderSavedAsync(TestRepositoryFolder folder)
    {
        await InvokeAsync(() => 
        {
            var node = FindTreeNode(x => x.TestRepositoryFolder?.Id == folder.Id);
            if (node is not null)
            {
                node.Text = folder.Name;
                node.Icon = TestIcons.GetIcon(folder);
                if (node.Value is not null)
                {
                    node.Value.Color = folder.Color;
                }
                this.StateHasChanged();
            }
        });
    }
    public async Task OnTestRepositoryFolderMovedAsync(TestRepositoryFolder folder)
    {
        await InvokeAsync(() => 
        {
            RemoveTreeNode(x => x.TestRepositoryFolder?.Id == folder.Id);
            AddRepositoryFolderToTreeView(folder);
            this.StateHasChanged();
        });
    }
    public async Task OnTestRepositoryFolderDeletedAsync(TestRepositoryFolder folder)
    {
        await InvokeAsync(() =>
        {
            RemoveTreeNode(x => x.TestRepositoryFolder?.Id == folder.Id);
            this.StateHasChanged();
        });
    }
    #endregion Test Repository Observer

    #region Test Lab Observer
    private void AddLabFolderToTreeView(TestLabFolder folder)
    {
        if (folder.ParentId is null)
        {
            var rootNode = FindTreeNode(x => x.VirtualFolderName == TestBrowser.ROOT_TEST_LAB);
            if (rootNode is not null)
            {
                var childNode = testBrowser.CreateLabFolderTreeNode(folder);
                rootNode.Expanded = true;
                if (rootNode.Children is null)
                {
                    rootNode.Children = [childNode];
                }
                else
                {
                    rootNode.Children = [.. rootNode.Children, childNode];
                }
            }
        }
        else
        {
            var parentNode = FindTreeNode(x => x.TestLabFolder?.Id == folder.ParentId);
            if (parentNode?.Children is not null)
            {
                var childNode = testBrowser.CreateLabFolderTreeNode(folder);
                parentNode.Children = [.. parentNode.Children, childNode];
            }
        }
    }
    public async Task OnTestLabFolderCreatedAsync(TestLabFolder folder)
    {
        await InvokeAsync(async () =>
        {
            AddLabFolderToTreeView(folder);
            await InvokeAsync(StateHasChanged);
        });
    }
    public async Task OnTestLabFolderSavedAsync(TestLabFolder folder)
    {
        await InvokeAsync(() => 
        {
            var node = FindTreeNode(x => x.TestLabFolder?.Id == folder.Id);
            if (node is not null)
            {
                node.Text = folder.Name;
                node.Icon = TestIcons.GetIcon(folder);
                if (node.Value is not null)
                {
                    node.Value.Color = folder.Color;
                }
                this.StateHasChanged();
            }
        });
    }
    public async Task OnTestLabFolderMovedAsync(TestLabFolder folder)
    {
        await InvokeAsync(() => 
        { 
            RemoveTreeNode(x => x.TestLabFolder?.Id == folder.Id);
            AddLabFolderToTreeView(folder);
            this.StateHasChanged(); 

        });
    }
    public async Task OnTestLabFolderDeletedAsync(TestLabFolder folder)
    {
        await InvokeAsync(() =>
        {
            RemoveTreeNode(x => x.TestLabFolder?.Id == folder.Id);
            this.StateHasChanged();
        });
    }

    #endregion

    #region Test Suite Observer

    public async Task OnTestSuiteMovedAsync(TestSuite suite)
    {
        await InvokeAsync(() =>
        {
            RemoveTestSuiteFromTreeView(suite);
            AddTestSuiteToTreeView(suite);
            this.StateHasChanged();
        });
    }

    public async Task OnTestSuiteSavedAsync(TestSuite suite)
    {
        await InvokeAsync(() =>
        {
            // If the test suite folder has not changed, just update the tree node properties
            // with color/name
            var node = FindTreeNode(x => x.TestSuite?.Id == suite.Id);
            if (node is not null)
            {
                node.Text = suite.Name;
                node.Icon = TestIcons.GetIcon(suite);
                if (node.Value is not null)
                {
                    node.Value.Color = suite.Color;
                }
                this.StateHasChanged();
            }
        });
    }
    private void RemoveTestSuiteFromTreeView(TestSuite suite)
    {
        RemoveTreeNode(x => x.TestSuite?.Id == suite.Id);
    }

    private void AddTestSuiteToTreeView(TestSuite suite)
    {
        if (suite.FolderId is null)
        {
            var parentNode = FindTreeNode(x => x.VirtualFolderName == TestBrowser.ROOT_TEST_REPOSITORY);
            if (parentNode?.Children is not null)
            {
                var node = testBrowser.CreateTestSuiteTreeNode(suite);
                parentNode.Children = [.. parentNode.Children, node];
            }
        }
        else
        {
            var parentNode = FindTreeNode(x => x.TestRepositoryFolder?.Id == suite.FolderId);
            if (parentNode?.Children is not null)
            {
                var node = testBrowser.CreateTestSuiteTreeNode(suite);
                parentNode.Children = [.. parentNode.Children, node];
            }
        }
    }

    public async Task OnTestSuiteCreatedAsync(TestSuite suite)
    {
        await InvokeAsync(() =>
        {
            AddTestSuiteToTreeView(suite);
            this.StateHasChanged();
        });
    }
    public async Task OnTestSuiteDeletedAsync(TestSuite suite)
    {
        await InvokeAsync(() =>
        {
            RemoveTestSuiteFromTreeView(suite);
            this.StateHasChanged();
        });
    }
    private TreeNode<BrowserItem>? RemoveTreeNode(Predicate<BrowserItem> predicate)
    {
        return TreeView<BrowserItem>.RemoveTreeNode(null, _rootItems, predicate);
    }

    private TreeNode<BrowserItem>? FindTreeNode(Predicate<BrowserItem> predicate)
    {
        return TreeView<BrowserItem>.FindTreeNode(_rootItems, predicate);
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
                await testSuiteController.SaveTestSuiteAsync(_editItem.Value.TestSuite);
            }
            if (_editItem.Value?.Folder is not null)
            {
                _editItem.Text = text;
                _editItem.Value.Folder.Name = text;
                await testSuiteController.SaveTestSuiteFolderAsync(_editItem.Value.Folder);
            }
            if (_editItem.Value?.TestRun is not null)
            {
                _editItem.Text = text;
                _editItem.Value.TestRun.Name = text;
                await testCaseEditor.SaveTestRunAsync(_editItem.Value.TestRun);
            }
            if (_editItem.Value?.TestRepositoryFolder is not null)
            {
                _editItem.Text = text;
                _editItem.Value.TestRepositoryFolder.Name = text;
                await testRepositoryController.UpdateTestRepositoryFolderAsync(_editItem.Value.TestRepositoryFolder);
            }
            if (_editItem.Value?.TestLabFolder is not null)
            {
                _editItem.Text = text;
                _editItem.Value.TestLabFolder.Name = text;
                await testLabController.UpdateTestLabFolderAsync(_editItem.Value.TestLabFolder);
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

    private async Task OnMenuOpened(TreeNode<BrowserItem> treeNode)
    {
        if (treeNode.Value?.TestCase is not null)
        {
            TestSuiteFolder? folder = null;
            var testCase = treeNode.Value.TestCase;
            TestSuite? suite = await testBrowser.GetTestSuiteByIdAsync(testCase.TestSuiteId);
            if (testCase.TestSuiteFolderId is not null)
            {
                folder = await testBrowser.GetTestSuiteFolderByIdAsync(testCase.TestSuiteFolderId.Value);
            }

            appNavigationManager.State.SetSelectedTestCase(treeNode.Value.TestCase, folder, suite);
        }
        else if (treeNode.Value?.Folder is not null)
        {
            var suite = await testBrowser.GetTestSuiteByIdAsync(treeNode.Value.Folder.TestSuiteId); 
            appNavigationManager.State.SetSelectedTestSuiteFolder(treeNode.Value.Folder, suite);
        }
        else if (treeNode.Value?.TestSuite is not null)
        {
            appNavigationManager.State.SetSelectedTestSuite(treeNode.Value.TestSuite);
        }
        else if(treeNode.Value?.TestRun is not null)
        {
            appNavigationManager.State.SetSelectedTestRun(treeNode.Value.TestRun);
        }
    }
}
