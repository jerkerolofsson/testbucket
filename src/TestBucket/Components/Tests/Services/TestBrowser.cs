using System.Collections.Concurrent;

using Microsoft.CodeAnalysis;

using MudBlazor;

using NGitLab.Models;

using TestBucket.Components.Shared.Icons;
using TestBucket.Components.Shared.Tree;
using TestBucket.Components.Tests.Dialogs;
using TestBucket.Domain.Files;
using TestBucket.Domain.Requirements.Models;
using TestBucket.Domain.Shared.Specifications;
using TestBucket.Domain.Teams.Models;
using TestBucket.Domain.Testing.Aggregates;
using TestBucket.Domain.Testing.Models;
using TestBucket.Domain.Testing.Specifications;
using TestBucket.Domain.Testing.Specifications.TestCases;
using TestBucket.Formats;

namespace TestBucket.Components.Tests.Services;
internal class TestBrowser : TenantBaseService
{
    private readonly TestSuiteService _testSuiteService;
    private readonly IDialogService _dialogService;
    private readonly ITextTestResultsImporter _textImporter;
    private readonly IFileRepository _fileRepository;
    private readonly ITestRunManager _testRunManager;

    // todo: migrate to domain manager!
    private readonly ITestCaseRepository _testCaseRepository;

    public const string ROOT_TEST_SUITES = "TestSuites";
    public const string ROOT_TEST_RUNS = "TestRuns";

    public TestBrowser(TestSuiteService testSuiteService,
        AuthenticationStateProvider authenticationStateProvider,
        IDialogService dialogService,
        ITextTestResultsImporter textImporter,
        IFileRepository fileRepository,
        ITestCaseRepository testCaseRepository,
        ITestRunManager testRunManager) : base(authenticationStateProvider)

    {
        _testSuiteService = testSuiteService;
        _dialogService = dialogService;
        _textImporter = textImporter;
        _fileRepository = fileRepository;
        _testCaseRepository = testCaseRepository;
        _testRunManager = testRunManager;
    }

    public async Task<TestRun?> GetTestRunByIdAsync(long id)
    {
        var tenantId = await GetTenantIdAsync();
        return await _testCaseRepository.GetTestRunByIdAsync(tenantId, id);
    }

    public async Task<TestSuiteFolder?> GetTestSuiteFolderByIdAsync(long id)
    {
        var tenantId = await GetTenantIdAsync();
        return await _testCaseRepository.GetTestSuiteFolderByIdAsync(tenantId, id);
    }

    public async Task<TestSuite?> GetTestSuiteByIdAsync(long id)
    {
        var tenantId = await GetTenantIdAsync();
        return await _testCaseRepository.GetTestSuiteByIdAsync(tenantId, id);
    }

    public async Task<TestCase?> GetTestCaseByIdAsync(long id)
    {
        var tenantId = await GetTenantIdAsync();
        return await _testCaseRepository.GetTestCaseByIdAsync(tenantId, id);
    }

    /// <summary>
    /// Imports test cases for the specified team/project
    /// </summary>
    /// <param name="team"></param>
    /// <param name="project"></param>
    /// <returns></returns>
    public async Task ImportAsync(Team? team, TestProject? project)
    {
        var principal = await GetUserClaimsPrincipalAsync();
        var tenantId = await GetTenantIdAsync();

        // Show dialog
        var dialog = await _dialogService.ShowAsync<ImportResultsDialog>(null);
        var result = await dialog.Result;

        if (result?.Data is ImportOptions importOptions && importOptions.File?.Id is not null)
        {
            // Get the uploaded file and import it
            var resource = await _fileRepository.GetResourceByIdAsync(tenantId, importOptions.File.Id);
            if (resource is not null)
            {
                string xml = Encoding.UTF8.GetString(resource.Data);
                await _textImporter.ImportTextAsync(principal, team?.Id, project?.Id, importOptions.Format, xml, importOptions.HandlingOptions);
            }
        }
    }

    /// <summary>
    /// Searches for test cases
    /// </summary>
    /// <param name="testSuiteId"></param>
    /// <param name="folderId"></param>
    /// <param name="searchText"></param>
    /// <param name="offset"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    //public async Task<PagedResult<TestCase>> SearchTestCasesAsync(long? testSuiteId, long? folderId, string? searchText, int offset, int count = 20)
    //{
    //    if(string.IsNullOrWhiteSpace(searchText))
    //    {
    //        searchText = null;
    //    }

    //    return await _testSuiteService.SearchTestCasesAsync(new SearchTestQuery
    //    {
    //        Text = searchText,
    //        TestSuiteId = testSuiteId,
    //        FolderId = folderId,
    //        Count = count,
    //        Offset = offset,
    //    });
    //}
    public async Task<PagedResult<TestCase>> SearchTestCasesAsync(SearchTestQuery query, int offset, int count = 20)
    {
        query.Offset = offset;
        query.Count = count;

        return await _testSuiteService.SearchTestCasesAsync(query);
    }

    public async Task<List<TreeNode<BrowserItem>>> BrowseAsync(TestBrowserRequest request)
    {
        long? teamId = request.TeamId;
        long? projectId = request.ProjectId;
        BrowserItem? parent = request.Parent;
        string? searchText = request.Text;

        if (parent is not null)
        {
            if(parent.TestCase is not null)
            {
                return [];
            }
            if(parent.Folder is not null)
            {
                var folders = await _testSuiteService.GetTestSuiteFoldersAsync(projectId, parent.Folder.TestSuiteId, parent.Folder.Id);
                var items = MapFoldersToTreeNode(folders);

                if (request.ShowTestCases)
                {
                    var tests = await _testSuiteService.SearchTestCasesAsync(new SearchTestQuery
                    {
                        Count = 1_000,
                        Offset = 0,
                        FolderId = parent.Folder.Id,
                        TestSuiteId = parent.Folder.TestSuiteId,
                    });
                    items.AddRange(MapTestsToTreeNode(tests.Items));
                }

                return items;
            }
            else if (parent.TestSuite is not null)
            {
                long? parentFolderId = null;
                var folders = await _testSuiteService.GetTestSuiteFoldersAsync(projectId, parent.TestSuite.Id, parentFolderId);
                var items = MapFoldersToTreeNode(folders);

                var tests = await _testSuiteService.SearchTestCasesAsync(new SearchTestQuery
                {
                    Count = 1_000,
                    Offset = 0,
                    FolderId = null,
                    TestSuiteId = parent.TestSuite.Id,
                });
                items.AddRange(MapTestsToTreeNode(tests.Items));

                return items;
            }
            else if (parent.TestRun is not null)
            {
                return [];
            }
        }

        if(searchText is not null)
        {
            return await SearchAsync(teamId, projectId, searchText);
        }

        return await BrowseRootAsync(request);
    }

    public async Task CustomizeFolderAsync(TestSuiteFolder folder)
    {
        var parameters = new DialogParameters<EditFolderDialog>()
        {
            { x => x.Folder, folder }
        };
        var dialog = await _dialogService.ShowAsync<EditFolderDialog>(null, parameters);
        var result = await dialog.Result;
    }

    private List<TreeNode<BrowserItem>> MapTestsToTreeNode(TestCase[] tests)
    {
        return tests.Select(x => CreateTreeNodeFromTestCase(x)).ToList();
    }
    private List<TreeNode<BrowserItem>> MapFoldersToTreeNode(TestSuiteFolder[] folders)
    {
        return folders.Select(x => CreateTreeNodeFromFolder(x)).ToList();
    }

    public TreeNode<BrowserItem> CreateTestSuiteTreeNode(TestSuite testSuite)
    {
        return new TreeNode<BrowserItem>
        {
            Value = new BrowserItem() { TestSuite = testSuite, Color = testSuite.Color },
            Text = testSuite.Name,
            Icon = testSuite.Icon ?? Icons.Material.Outlined.Article,
            Children = null,
        };
    }

    public TreeNode<BrowserItem> CreateTestRunTreeNode(TestRun testRun)
    {
        return new TreeNode<BrowserItem>
        {
            Value = new BrowserItem() { TestRun = testRun },
            Text = testRun.Name,
            Icon = Icons.Material.Filled.PlaylistPlay,
            Children = [],
        };
    }
    public TreeNode<BrowserItem> CreateTreeNodeFromTestCase(TestCase x)
    {
        return new TreeNode<BrowserItem>
        {
            Value = new BrowserItem { TestCase = x },
            Text = x.Name,
            Icon = TbIcons.Filled.PaperPlane,
            Expandable = false,
            Children = null,
        };
    }
    public async Task AddTestSuiteFolderAsync(long projectId, long testSuiteId, long? parentFolderId)
    {
        var parameters = new DialogParameters<AddTestSuiteFolderDialog>
        {
            { x => x.ProjectId, projectId },
            { x => x.TestSuiteId, testSuiteId },
            { x => x.ParentFolderId, parentFolderId },
        };
        var dialog = await _dialogService.ShowAsync<AddTestSuiteFolderDialog>("Add folder", parameters);
        var result = await dialog.Result;
    }

    public TreeNode<BrowserItem> CreateTreeNodeFromFolder(TestSuiteFolder x)
    {
        var defaultFolder = x.IsFeature ? Icons.Material.Filled.Stars : Icons.Material.Filled.Folder;

        return new TreeNode<BrowserItem>
        {
            Value = new BrowserItem { Folder = x, Color = x.Color },
            Text = x.Name,
            Icon = x.Icon ?? defaultFolder,
            Expandable = true,
            Children = null,
        };
    }

    public async Task<PagedResult<TestCaseRun>> SearchTestCaseRunsAsync(SearchTestCaseRunQuery query)
    {
        var principal = await GetUserClaimsPrincipalAsync();
        return await _testRunManager.SearchTestCaseRunsAsync(principal, query);
    }

    /// <summary>
    /// Searches for test case runs
    /// </summary>
    /// <param name="testRunId"></param>
    /// <param name="searchText"></param>
    /// <param name="offset"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    public async Task<PagedResult<TestCaseRun>> SearchTestCaseRunsAsync(long testRunId, string? searchText, int offset, int count = 20)
    {
        return await SearchTestCaseRunsAsync(new SearchTestCaseRunQuery
        {
            Text = searchText,
            TestRunId = testRunId,
            Count = count,
            Offset = offset,
        });
    }
    /// <summary>
    /// Returns a summary report of results (passed, failed..) filtered by the query
    /// </summary>
    /// <param name="testRunId"></param>
    /// <param name="searchText"></param>
    /// <returns></returns>
    public async Task<TestExecutionResultSummary> GetTestExecutionResultSummaryAsync(SearchTestCaseRunQuery query)
    {
        var principal = await GetUserClaimsPrincipalAsync();
        return await _testRunManager.GetTestExecutionResultSummaryAsync(principal, query);
    }
    /// <summary>
    /// Returns a summary report of results (passed, failed..) filtered by the query
    /// </summary>
    /// <param name="testRunId"></param>
    /// <param name="searchText"></param>
    /// <returns></returns>
    public async Task<TestExecutionResultSummary> GetTestExecutionResultSummaryAsync(long testRunId, string? searchText)
    {
        if (string.IsNullOrWhiteSpace(searchText))
        {
            searchText = null;
        }
        return await GetTestExecutionResultSummaryAsync(new SearchTestCaseRunQuery
        {
            Text = searchText,
            TestRunId = testRunId,
        });
    }


    private async Task<List<TreeNode<BrowserItem>>> SearchAsync(long? teamId, long? projectId, string searchText)
    {
        var rootItems = new List<TreeNode<BrowserItem>>();

        var recentRunsResult = await _testSuiteService.SearchTestCasesAsync(new SearchTestQuery() 
        { 
            CompareFolder = false,
            ProjectId = projectId, 
            Count = 50, 
            TeamId = teamId, 
            Offset = 0,
            Text = searchText
        });
        foreach (var testCase in recentRunsResult.Items)
        {
            var testCaseNode = CreateTreeNodeFromTestCase(testCase);

            rootItems.Add(testCaseNode);
        }
        return rootItems;
    }

    private async Task<List<TreeNode<BrowserItem>>> BrowseRootAsync(TestBrowserRequest request)
    {
        var teamId = request.TeamId;
        var projectId = request.ProjectId;

        var suites = await _testSuiteService.GetTestSuitesAsync(teamId, projectId);
        var suiteItems = suites.Items.Select(x => CreateTestSuiteTreeNode(x)).ToList();

        // For test runs, as there can be a huge amount, sort them by year
        List<TreeNode<BrowserItem>> recentRuns = [];

        var principal = await GetUserClaimsPrincipalAsync();

        var items = new List<TreeNode<BrowserItem>>();
        if (request.ShowTestSuites)
        {
            items.Add(new TreeNode<BrowserItem>
            {
                Text = "Test Suites",
                Children = suiteItems,
                Expanded = true,
                Value = new BrowserItem() { RootFolderName = ROOT_TEST_SUITES },
                Icon = Icons.Material.Filled.FolderOpen,
            });
        }
        if(request.ShowTestRuns)
        {
            var query = new SearchTestRunQuery() { ProjectId = projectId, Count = 50, TeamId = teamId, Offset = 0 };
            var recentRunsResult = await _testRunManager.SearchTestRunsAsync(principal, query);
            foreach (var testRun in recentRunsResult.Items)
            {
                recentRuns.Add(CreateTestRunTreeNode(testRun));
            }

            items.Add(new TreeNode<BrowserItem>
            {
                Text = "Test Runs",
                Children = recentRuns,
                Expanded = false,
                Value = new BrowserItem() { RootFolderName = ROOT_TEST_RUNS },
                Icon = Icons.Material.Filled.FolderOpen,
            });
        }
        return items;
    }


    internal async Task<TestSuite?> AddTestSuiteAsync(Team? team, TestProject? project)
    {

        var parameters = new DialogParameters<AddTestSuiteDialog>
        {
            { x => x.Project, project },
            { x => x.Team, team },
        };
        var dialog = await _dialogService.ShowAsync<AddTestSuiteDialog>("Add test suite", parameters);
        var result = await dialog.Result;
        if (result?.Data is TestSuite testSuite)
        {
            return testSuite;
        }
        return null;

    }

    /// <summary>
    /// Returns an array of test case IDs for a folder
    /// </summary>
    /// <param name="folder"></param>
    /// <returns></returns>
    internal async Task<long[]> GetTestSuiteSuiteFolderTestsAsync(TestSuiteFolder folder, bool includeAllDescendants)
    {
        var tenantId = await GetTenantIdAsync();
        FilterSpecification<TestCase>[] specifications = [new FilterByTenant<TestCase>(tenantId), new FilterTestCasesByTestSuiteFolder(folder.Id, includeAllDescendants)];
        return await _testCaseRepository.SearchTestCaseIdsAsync(specifications);
    }

    /// <summary>
    /// Returns an array of test case IDs for a test suite
    /// </summary>
    /// <param name="testSuite"></param>
    /// <returns></returns>
    internal async Task<long[]> GetTestSuiteSuiteTestsAsync(TestSuite testSuite)
    {
        var tenantId = await GetTenantIdAsync();
        FilterSpecification<TestCase>[] specifications = [new FilterByTenant<TestCase>(tenantId), new FilterTestCasesByTestSuite(testSuite.Id)];
        return await _testCaseRepository.SearchTestCaseIdsAsync(specifications);
    }
}
