using System.Diagnostics;
using System.Net;

using Microsoft.Extensions.Localization;

using TestBucket.Components.Automation;
using TestBucket.Components.Shared.Fields;
using TestBucket.Components.Shared.Tree;
using TestBucket.Components.Tests.Dialogs;
using TestBucket.Components.Tests.Models;
using TestBucket.Components.Tests.TestCases.Dialogs;
using TestBucket.Components.Tests.TestLab.Services;
using TestBucket.Components.Tests.TestRepository.Services;
using TestBucket.Components.Tests.TestRuns.Controllers;
using TestBucket.Components.Tests.TestSuites.Dialogs;
using TestBucket.Components.Tests.TestSuites.Services;
using TestBucket.Domain;
using TestBucket.Domain.Automation.Pipelines.Models;
using TestBucket.Domain.Code.Services;
using TestBucket.Domain.Files;
using TestBucket.Domain.Milestones;
using TestBucket.Domain.Requirements.Models;
using TestBucket.Domain.Shared.Specifications;
using TestBucket.Domain.Teams;
using TestBucket.Domain.Testing.Aggregates;
using TestBucket.Domain.Testing.ImportExport;
using TestBucket.Domain.Testing.Models;
using TestBucket.Domain.Testing.Services.Import;
using TestBucket.Domain.Testing.Specifications.TestCases;
using TestBucket.Domain.Testing.TestCases;
using TestBucket.Domain.Testing.TestCases.Search;
using TestBucket.Domain.Testing.TestRuns;
using TestBucket.Domain.Testing.TestRuns.Search;
using TestBucket.Localization;

namespace TestBucket.Components.Tests.Services;
internal class TestBrowser : TenantBaseService
{
    private readonly TestRepositoryController _testRepositoryController;
    private readonly TestLabController _testLabController;
    private readonly TestSuiteController _testSuiteController;
    private readonly TestRunController _testRunController;
    private readonly IDialogService _dialogService;
    private readonly ITextTestResultsImporter _textImporter;
    private readonly IFileRepository _fileRepository;
    private readonly ITestRunManager _testRunManager;
    private readonly ITeamManager _teamManager;
    private readonly ITestCaseManager _testCaseManager;
    private readonly IMilestoneManager _milestoneManager;
    private readonly IArchitectureManager _architectureManager;
    private readonly FieldController _fieldController;
    private readonly AppNavigationManager _appNavigationManager;
    private readonly PipelineController _pipelineController;
    private readonly IStringLocalizer<SharedStrings> _loc;
    private readonly ITestCaseImporter _testCaseImporter;

    // todo: migrate to domain manager!
    private readonly ITestCaseRepository _testCaseRepository;

    /// <summary>
    /// Virtual folder for test repo
    /// </summary>
    public const string ROOT_TEST_REPOSITORY = "TestRepository";

    /// <summary>
    /// Virtual folder for test lab
    /// </summary>
    public const string ROOT_TEST_LAB = "TestLab";

    /// <summary>
    /// Virtual folder for pipelines
    /// </summary>
    public const string FOLDER_PIPELINES = "Pipelines";

    /// <summary>
    /// Virtual folder for test runs
    /// </summary>
    public const string ROOT_RECENT_TEST_RUNS = "RecentTestRuns";

    public TestBrowser(TestSuiteController testSuiteService,
        AuthenticationStateProvider authenticationStateProvider,
        IDialogService dialogService,
        ITextTestResultsImporter textImporter,
        IFileRepository fileRepository,
        ITestCaseRepository testCaseRepository,
        ITestRunManager testRunManager,
        AppNavigationManager appNavigationManager,
        PipelineController pipelineManager,
        ITestCaseManager testCaseManager,
        IStringLocalizer<SharedStrings> loc,
        ITeamManager teamManager,
        FieldController fieldController,
        IMilestoneManager milestoneManager,
        IArchitectureManager architectureManager,
        ITestCaseImporter testCaseImporter,
        TestRepositoryController testRepositoryController,
        TestLabController testLabController,
        TestRunController testRunController) : base(authenticationStateProvider)

    {
        _testSuiteController = testSuiteService;
        _dialogService = dialogService;
        _textImporter = textImporter;
        _fileRepository = fileRepository;
        _testCaseRepository = testCaseRepository;
        _testRunManager = testRunManager;
        _appNavigationManager = appNavigationManager;
        _pipelineController = pipelineManager;
        _testCaseManager = testCaseManager;
        _loc = loc;
        _teamManager = teamManager;
        _fieldController = fieldController;
        _milestoneManager = milestoneManager;
        _architectureManager = architectureManager;
        _testCaseImporter = testCaseImporter;
        _testRepositoryController = testRepositoryController;
        _testLabController = testLabController;
        _testRunController = testRunController;
    }


    public async Task<TestCase?> PickTestCaseAsync(TestProject? project, Team? team)
    {
        var principal = await GetUserClaimsPrincipalAsync();

        var parameters = new DialogParameters<PickTestCaseDialog>()
        {
            { x => x.Project, project },
            { x => x.Team, team },
        };

        var dialog = await _dialogService.ShowAsync<PickTestCaseDialog>(_loc["select-test"], parameters, DefaultBehaviors.DialogOptions);
        var result = await dialog.Result;
        if (result?.Data is TestCase testCase)
        {
            return testCase;
        }
        return null;
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
    /// Imports test results for the specified team/project
    /// </summary>
    /// <param name="team"></param>
    /// <param name="project"></param>
    /// <returns></returns>
    public async Task ImportResultsAsync(Team? team, TestProject project)
    {
        var principal = await GetUserClaimsPrincipalAsync();
        var tenantId = await GetTenantIdAsync();

        if(project.TeamId is null)
        {
            return;
        }
        team ??= await _teamManager.GetTeamByIdAsync(principal, project.TeamId.Value);
        if (team is null)
        {
            return;
        }

        // Show dialog
        var dialog = await _dialogService.ShowAsync<ImportResultsDialog>(_loc["import-test-results"], DefaultBehaviors.DialogOptions);
        var result = await dialog.Result;

        if (result?.Data is ImportOptions importOptions && importOptions.File?.Id is not null)
        {
            // Get the uploaded file and import it
            var resource = await _fileRepository.GetResourceByIdAsync(tenantId, importOptions.File.Id);
            if (resource is not null)
            {
                string xml = TextConversionUtils.FromBomEncoded(resource.Data);
                try
                {
                    await _textImporter.ImportTextAsync(principal, team.Id, project.Id, importOptions.Format, xml, importOptions.HandlingOptions);
                }
                catch (Exception ex)
                {
                    await _dialogService.ShowMessageBox("Error", ex.Message);
                }
            }
        }
    }


    /// <summary>
    /// Imports test cases for the specified team/project
    /// </summary>
    /// <param name="team"></param>
    /// <param name="project"></param>
    /// <returns></returns>
    public async Task ImportTestCasesAsync(Team? team, TestProject project)
    {
        var principal = await GetUserClaimsPrincipalAsync();
        var tenantId = await GetTenantIdAsync();

        if (project.TeamId is null)
        {
            return;
        }
        team ??= await _teamManager.GetTeamByIdAsync(principal, project.TeamId.Value);
        if (team is null)
        {
            return;
        }

        // Show dialog
        var dialog = await _dialogService.ShowAsync<ImportRepoDialog>(_loc["import-test-cases"], DefaultBehaviors.DialogOptions);
        var result = await dialog.Result;

        if (result?.Data is ImportOptions importOptions && importOptions.File?.Id is not null)
        {
            // Get the uploaded file and import it
            var resource = await _fileRepository.GetResourceByIdAsync(tenantId, importOptions.File.Id);
            if (resource is not null)
            {
                try
                {
                    await _testCaseImporter.ImportAsync(principal, team.Id, project.Id, resource.Data, resource.ContentType, importOptions.HandlingOptions);
                }
                catch(Exception ex)
                {
                    await _dialogService.ShowMessageBox("Error", ex.Message);
                }
            }
        }
    }

    /// <summary>
    /// Returns a flat list of test cases and folders
    /// </summary>
    /// <param name="query"></param>
    /// <param name="offset"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    public async Task<PagedResult<TestSuiteItem>> SearchItemsAsync(SearchTestQuery query, int offset, int count = 20, bool includeFolders = true)
    {
        if(query.TestSuiteId is null && query.ProjectId is null)
        {
            return new PagedResult<TestSuiteItem>() { Items = [], TotalCount = 0 };
        }

        List<TestSuiteItem> items = new();
        query.Offset = offset;
        query.Count = count;

        // Get all folders
        int folderCount = 0;
        if (includeFolders && query.TestSuiteId is not null)
        {
            var folders = await _testSuiteController.GetTestSuiteFoldersAsync(query.ProjectId, query.TestSuiteId.Value, query.FolderId);
            items.AddRange(folders.Select(x => new TestSuiteItem() { Folder = x }).Skip(offset).Take(count));
            int visibleFolderCount = items.Count;

            // Get tests
            int testCount = Math.Max(0, count - items.Count);
            int testCaseOffset = Math.Max(0, offset - folders.Length);

            query.Offset = testCaseOffset;
            query.Count = testCount;
            folderCount = folders.Length;
        }
        var tests = await _testSuiteController.SearchTestCasesAsync(query);

        items.AddRange(tests.Items.Select(x => new TestSuiteItem() { TestCase = x }));

        return new PagedResult<TestSuiteItem>()
        {
            TotalCount = folderCount + tests.TotalCount,
            Items = items.ToArray(),
        };
    }


    //public async Task<PagedResult<TestCase>> SearchTestCasesAsync(SearchTestQuery query, int offset, int count = 20)
    //{
    //    query.Offset = offset;
    //    query.Count = count;

    //    return await _testSuiteController.SearchTestCasesAsync(query);
    //}

    public async Task<List<TreeNode<BrowserItem>>> BrowseAsync(TestBrowserRequest request)
    {
        long? teamId = request.TeamId;
        long? projectId = request.ProjectId;
        BrowserItem? parent = request.Parent;
        string? searchText = request.Text;

        if (parent is not null)
        {
            if (parent.TestRepositoryFolder is not null)
            {
                if(projectId is null)
                {
                    return [];
                }
                var folders = await _testRepositoryController.GetFoldersAsync(projectId.Value, parent.TestRepositoryFolder.Id);
                var folderItems = folders.Select(x => CreateRepositoryFolderTreeNode(x));
                
                var suites = await _testSuiteController.GetTestSuitesInFolderAsync(null, projectId.Value, parent.TestRepositoryFolder.Id, 0, 100);
                var suiteItems = suites.Items.Select(x => CreateTestSuiteTreeNode(x));

                return [.. folderItems, .. suiteItems];
            }
            else if (parent.TestLabFolder is not null)
            {
                if (projectId is null)
                {
                    return [];
                }
                var folders = await _testLabController.GetFoldersAsync(projectId.Value, parent.TestLabFolder.Id);
                
                var folderItems = folders.Select(x => CreateLabFolderTreeNode(x));

                var runs = await _testRunController.GetTestRunsInFolderAsync(projectId.Value, parent.TestLabFolder.Id, 0, 100);
                List<TreeNode<BrowserItem>> runItems = [];
                foreach(var run in runs.Items)
                {
                    runItems.Add(await CreateTestRunTreeNode(request, run));
                }

                return [.. folderItems, .. runItems];

            }
            else if (parent.TestCase is not null)
            {
                return [];
            }
            else if(parent.Folder is not null)
            {
                var folders = await _testSuiteController.GetTestSuiteFoldersAsync(projectId, parent.Folder.TestSuiteId, parent.Folder.Id);
                var items = MapFoldersToTreeNode(folders);

                if (request.ShowTestCases)
                {
                    var tests = await _testSuiteController.SearchTestCasesAsync(new SearchTestQuery
                    {
                        CompareFolder = true,
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
                var folders = await _testSuiteController.GetTestSuiteFoldersAsync(projectId, parent.TestSuite.Id, parentFolderId);
                var items = MapFoldersToTreeNode(folders);

                var tests = await _testSuiteController.SearchTestCasesAsync(new SearchTestQuery
                {
                    CompareFolder = true,
                    Count = 1_000,
                    Offset = 0,
                    FolderId = null,
                    TestSuiteId = parent.TestSuite.Id,
                });
                items.AddRange(MapTestsToTreeNode(tests.Items));

                return items;
            }
            else if (parent.Pipeline is not null)
            {
                return [];
            }
            else if (parent.VirtualFolderName == FOLDER_PIPELINES)
            {
                var items = new List<TreeNode<BrowserItem>>();

                if (parent.TestRun is not null)
                {
                    var pipelines = await _pipelineController.GetPipelinesForTestRunAsync(parent.TestRun.Id);
                    items.AddRange(MapPipelinesToTreeNode(pipelines));
                }

                return items;
            }
            else if (parent.TestRun is not null)
            {
                // This should not happen as child folders are added when creating a tree node for test run
                // If it happens, it may be a virtual folder that has a test run, that check should be above this in the 
                // if statement
                Debug.Assert(false, "TreeNode for test run should never need to call browse");
            }
        }

        if(searchText is not null)
        {
            return await SearchAsync(request);
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

    private List<TreeNode<BrowserItem>> MapPipelinesToTreeNode(IEnumerable<Pipeline> pipelines)
    {
        return pipelines.Select(x => CreateTreeNodeFromPipeline(x)).ToList();
    }

    private List<TreeNode<BrowserItem>> MapTestsToTreeNode(IEnumerable<TestCase> tests)
    {
        return tests.Select(x => CreateTreeNodeFromTestCase(x)).ToList();
    }
    private List<TreeNode<BrowserItem>> MapFoldersToTreeNode(IEnumerable<TestSuiteFolder> folders)
    {
        return folders.Select(x => CreateTreeNodeFromFolder(x)).ToList();
    }

    public TreeNode<BrowserItem> CreateLabFolderTreeNode(TestLabFolder folder)
    {
        return new TreeNode<BrowserItem>
        {
            Value = new BrowserItem() { TestLabFolder = folder, Color = folder.Color },
            Text = folder.Name,
            Icon = TestIcons.GetIcon(folder),
            Children = null,
        };
    }
    public TreeNode<BrowserItem> CreateRepositoryFolderTreeNode(TestRepositoryFolder folder)
    {
        return new TreeNode<BrowserItem>
        {
            Value = new BrowserItem() { TestRepositoryFolder = folder, Color = folder.Color },
            Text = folder.Name,
            Icon = TestIcons.GetIcon(folder),
            Children = null,
        };
    }

    public TreeNode<BrowserItem> CreateTestSuiteTreeNode(TestSuite testSuite)
    {
        return new TreeNode<BrowserItem>
        {
            Value = new BrowserItem() { TestSuite = testSuite, Color = testSuite.Color },
            Text = testSuite.Name,
            Icon = TestIcons.GetIcon(testSuite),
            Children = null,
        };
    }

    public async Task<TreeNode<BrowserItem>> CreateTestRunTreeNode(TestBrowserRequest request, TestRun testRun)
    {
        var principal = await GetUserClaimsPrincipalAsync();

        var items = new List<TreeNode<BrowserItem>>();
        if (request.ShowTestRunPipelines)
        {
            items.Add(new TreeNode<BrowserItem>
            {
                Text = _loc["pipelines"],
                Children = null,
                Expanded = false,
                Value = new BrowserItem() { VirtualFolderName = FOLDER_PIPELINES, TestRun = testRun },
                Icon = TbIcons.BoldDuoTone.Rocket,
            });
        }
        if (request.ShowTestRunTests)
        {
            items.Add(new TreeNode<BrowserItem>
            {
                Text = _loc["test-case-runs"],
                Children = [],
                Expanded = false,
                Expandable = false,
                Value = new BrowserItem() { Href = $"{_appNavigationManager.GetTestRunTestsUrl(testRun.Id)}" },
                Icon = TbIcons.BoldDuoTone.FolderCheck,
            });

            items.Add(new TreeNode<BrowserItem>
            {
                Text = _loc["fields"],
                Children = [],
                Expanded = false,
                Expandable = false,
                Value = new BrowserItem() { Href = _appNavigationManager.GetTestRunFieldsUrl(testRun.Id), TestRun = testRun },
                Icon = TbIcons.BoldDuoTone.Field,
            });


            // Search folders
            items.Add(new TreeNode<BrowserItem>
            {
                Text = _loc["my-backlog"],
                Children = [],
                Expanded = false,
                Value = new BrowserItem()
                {
                    Href = $"{_appNavigationManager.GetTestRunTestsUrl(testRun.Id)}?q=completed:no%20assigned-to:{WebUtility.UrlEncode(principal.Identity?.Name)}"
                },
                Icon = TbIcons.IconSaxDuoTone.Document1,
            });
            items.Add(new TreeNode<BrowserItem>
            {
                Text = _loc["assigned-to-me"],
                Children = [],
                Expanded = false,
                Value = new BrowserItem() 
                { 
                    Href = $"{_appNavigationManager.GetTestRunTestsUrl(testRun.Id)}?q=assigned-to:{WebUtility.UrlEncode(principal.Identity?.Name)}" 
                },
                Icon = TbIcons.BoldDuoTone.UserCircle,
            });

            items.Add(new TreeNode<BrowserItem>
            {
                Text = _loc["unassigned"],
                Children = [],
                Expanded = false,
                Value = new BrowserItem()
                {
                    Href = $"{_appNavigationManager.GetTestRunTestsUrl(testRun.Id)}?q=unassigned:yes"
                },
                Icon = TbIcons.BoldDuoTone.UserCircle,
            });

            items.Add(new TreeNode<BrowserItem>
            {
                Text = _loc["incomplete"],
                Children = [],
                Expanded = false,
                Value = new BrowserItem()
                {
                    Href = $"{_appNavigationManager.GetTestRunTestsUrl(testRun.Id)}?q=completed:no"
                },
                Icon = TbIcons.BoldDuoTone.QuestionSquare,
            });
          
            items.Add(new TreeNode<BrowserItem>
            {
                Text = _loc["passed"],
                Children = [],
                Expanded = false,
                Value = new BrowserItem()
                {
                    Href = $"{_appNavigationManager.GetTestRunTestsUrl(testRun.Id)}?q=result:passed"
                },
                IconColor = "rgba(11, 186, 131, 1)",
                Icon = TbIcons.BoldDuoTone.FolderCheck,
            });
            items.Add(new TreeNode<BrowserItem>
            {
                Text = _loc["blocked"],
                Children = [],
                Expanded = false,
                Value = new BrowserItem()
                {
                    Href = $"{_appNavigationManager.GetTestRunTestsUrl(testRun.Id)}?q=result:blocked"
                },
                IconColor = " rgb(250,220,80)",
                Icon = TbIcons.BoldDuoTone.FolderError,
            });
            items.Add(new TreeNode<BrowserItem>
            {
                Text = _loc["failures"],
                Children = [],
                Expanded = false,
                Value = new BrowserItem() 
                {
                    Href = $"{_appNavigationManager.GetTestRunTestsUrl(testRun.Id)}?q=result:failed"
                },
                IconColor = "rgba(246, 78, 98, 1)",
                Icon = TbIcons.BoldDuoTone.FolderError,
            });
        }

        var treeNode = new TreeNode<BrowserItem>
        {
            Value = new BrowserItem() { TestRun = testRun },
            Text = testRun.Name,
            Icon = testRun.Icon ?? TbIcons.BoldDuoTone.Box,
            Expandable = true,
            Children = items.ToArray(),
        };

        return treeNode;
    }


    public TreeNode<BrowserItem> CreateTreeNodeFromPipeline(Pipeline pipeline)
    {
        var treeNode = new TreeNode<BrowserItem>
        {
            Value = new BrowserItem { Pipeline = pipeline },
            Text = pipeline.DisplayTitle ?? "#" + pipeline.CiCdPipelineIdentifier,
            Icon = TestIcons.GetIcon(pipeline),
            Expandable = false,
            Children = null,
        };

        return treeNode;
    }
    public TreeNode<BrowserItem> CreateTreeNodeFromTestCase(TestCase x)
    {
        var treeNode = new TreeNode<BrowserItem>
        {
            Value = new BrowserItem { TestCase = x },
            Text = x.Name,
            Icon = TestIcons.GetIcon(x),
            Expandable = false,
            Children = null,
        };

        return treeNode;
    }
    public async Task AddTestSuiteFolderAsync(long projectId, long testSuiteId, long? parentFolderId)
    {
        var parameters = new DialogParameters<AddTestSuiteFolderDialog>
        {
        };
        var dialog = await _dialogService.ShowAsync<AddTestSuiteFolderDialog>(_loc["new-folder"], parameters, DefaultBehaviors.DialogOptions);
        var result = await dialog.Result;
        if(result?.Data is string name)
        {
            var testSuiteFolder = await _testSuiteController.AddTestSuiteFolderAsync(projectId, testSuiteId, parentFolderId, name);
        }
    }
    public async Task AddTestRepositoryFolderAsync(long projectId, long? parentFolderId)
    {
        var parameters = new DialogParameters<AddTestSuiteFolderDialog>
        {
        };
        var dialog = await _dialogService.ShowAsync<AddTestSuiteFolderDialog>(_loc["new-folder"], parameters, DefaultBehaviors.DialogOptions);
        var result = await dialog.Result;
        if (result?.Data is string name)
        {
            await _testRepositoryController.AddTestRepositoryFolderAsync(projectId, parentFolderId, name);
        }
    }
    public async Task AddTestLabFolderAsync(long projectId, long? parentFolderId)
    {
        var parameters = new DialogParameters<AddTestSuiteFolderDialog>
        {
        };
        var dialog = await _dialogService.ShowAsync<AddTestSuiteFolderDialog>(_loc["new-folder"], parameters, DefaultBehaviors.DialogOptions);
        var result = await dialog.Result;
        if (result?.Data is string name)
        {
            await _testLabController.AddTestLabFolderAsync(projectId, parentFolderId, name);
        }
    }

    public TreeNode<BrowserItem> CreateTreeNodeFromFolder(TestSuiteFolder x)
    {

        return new TreeNode<BrowserItem>
        {
            Value = new BrowserItem { Folder = x, Color = x.Color },
            Text = x.Name,
            Icon = TestIcons.GetIcon(x),
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
    /// Returns a test distribution by field value
    /// </summary>
    /// <param name="query"></param>
    /// <param name="fieldDefinition"></param>
    /// <returns></returns>
    public async Task<Dictionary<string, long>> GetTestCaseDistributionByFieldAsync(SearchTestQuery query, FieldDefinition fieldDefinition)
    {
        var principal = await GetUserClaimsPrincipalAsync();
        return await _testCaseManager.GetTestCaseDistributionByFieldAsync(principal, query, fieldDefinition.Id);
    }

    public async Task<Dictionary<string, Dictionary<string, long>>> GetTestCaseCoverageMatrixByFieldAsync(SearchTestQuery query, FieldDefinition fieldDefinition1, FieldDefinition fieldDefinition2)
    {
        var principal = await GetUserClaimsPrincipalAsync();
        return await _testCaseManager.GetTestCaseCoverageMatrixByFieldAsync(principal, query, fieldDefinition1.Id, fieldDefinition2.Id); 
    }

    /// <summary>
    /// Returns a summary report of results (passed, failed..) filtered by the query
    /// </summary>
    /// <param name="query"></param>
    /// <param name="fieldDefinition"></param>
    /// <returns></returns>
    public async Task<Dictionary<string,TestExecutionResultSummary>> GetTestExecutionResultSummaryByFieldAsync(SearchTestCaseRunQuery query, FieldDefinition fieldDefinition)
    {
        var principal = await GetUserClaimsPrincipalAsync();
        return await _testRunManager.GetTestExecutionResultSummaryByFieldAsync(principal, query, fieldDefinition.Id);
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

        //var test = await _testRunManager.GetTestExecutionResultSummaryByFieldAsync(principal, query, 11);

        return await _testRunManager.GetTestExecutionResultSummaryAsync(principal, query);
    }
    public async Task<Dictionary<DateOnly, TestExecutionResultSummary>> GetTestExecutionResultSummaryByDayAsync(SearchTestCaseRunQuery query)
    {
        var principal = await GetUserClaimsPrincipalAsync();

        return await _testRunManager.GetTestExecutionResultSummaryByDayAsync(principal, query);
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

    /// <summary>
    /// Search for test cases
    /// </summary>
    /// <param name="teamId"></param>
    /// <param name="projectId"></param>
    /// <param name="searchText"></param>
    /// <returns></returns>
    private async Task<List<TreeNode<BrowserItem>>> SearchAsync(TestBrowserRequest request)
    {
        if (request.ProjectId is null)
        {
            return [];
        }
        if (request.Text is null)
        {
            return [];
        }
        var fields = await _fieldController.GetDefinitionsAsync(request.ProjectId.Value, Contracts.Fields.FieldTarget.TestCase);
        var searchRequest = SearchTestCaseQueryParser.Parse(request.Text, fields);
        searchRequest.ProjectId = request.ProjectId.Value;

        var searchTestResult = await _testSuiteController.SearchTestCasesAsync(searchRequest); 
        //new SearchTestQuery() 
        //{ 
        //    CompareFolder = false,
        //    ProjectId = request.ProjectId, 
        //    Count = 20, 
        //    TeamId = request.TeamId, 
        //    Offset = 0,
        //    Text = request.Text
        //});

        var rootItems = await BrowseRootAsync(request);
        rootItems[0].Expanded = true;

        foreach (var testCase in searchTestResult.Items)
        {
            await AddToHierarchyAsync(testCase, rootItems);
        }
        return rootItems;
    }

    private async Task AddToHierarchyAsync(TestCase testCase, List<TreeNode<BrowserItem>> rootItems)
    {
        if(testCase.PathIds is null)
        {
            // We can't show dangling tests..
            return;
        }

        // The root items should already contain the test suites, so we should be able to find it here
        var testSuiteNode = TreeView<BrowserItem>.FindTreeNode(rootItems, x => x.TestSuite?.Id == testCase.TestSuiteId);
        if(testSuiteNode is null)
        {
            return;
        }

        TreeNode<BrowserItem> parent = testSuiteNode;
        testSuiteNode.Expanded = true;

        // Resolve the parent hierarchy, with test suites and folders
        foreach (var folderId in testCase.PathIds)
        {
            var folderNode = TreeView<BrowserItem>.FindTreeNode(rootItems, x => x.Folder?.Id == folderId);
            if (folderNode is null)
            {
                var childRequest = new TestBrowserRequest() { ProjectId = testCase.TestProjectId, TeamId = testCase.TeamId, Parent = parent.Value, TestSuiteId = testCase.TestSuiteId };
                childRequest.Parent = parent.Value;

                var items = await BrowseAsync(childRequest);
                parent.Children = items;
                folderNode = TreeView<BrowserItem>.FindTreeNode(rootItems, x => x.Folder?.Id == folderId);
            }
            if (folderNode is null)
            {
                return;
            }
            folderNode.Expanded = true;
            parent = folderNode;
        }

        if (parent is not null)
        {
            var testCaseNode = CreateTreeNodeFromTestCase(testCase);
            if(parent.Children is null)
            {
                parent.Children = [testCaseNode];
            }
            else
            {
                parent.Children = [.. parent.Children, testCaseNode];
            }
        }
    }


    private async Task<List<TreeNode<BrowserItem>>> BrowseRootAsync(TestBrowserRequest request)
    {
        var teamId = request.TeamId;
        var projectId = request.ProjectId;
        if(projectId is null)
        {
            return [];
        }
        var principal = await GetUserClaimsPrincipalAsync();

        // Load data
        var milestones = await _milestoneManager.GetMilestonesAsync(principal, projectId.Value);
        var components = await _architectureManager.GetComponentsAsync(principal, projectId.Value);
        var features = await _architectureManager.GetFeaturesAsync(principal, projectId.Value);

        var items = new List<TreeNode<BrowserItem>>();
        if (request.ShowTestSuites)
        {
            // Get test suites that are not located in any folder
            var suites = await _testSuiteController.GetRootTestSuitesAsync(teamId, projectId);
            var suiteItems = suites.Items.Select(x => CreateTestSuiteTreeNode(x)).ToList();

            // Get root folders
            var repositoryFolderItems = new List<TreeNode<BrowserItem>>();
            if (projectId is not null)
            {
                var repositoryFolders = await _testRepositoryController.GetRootFoldersAsync(projectId.Value);
                repositoryFolderItems = repositoryFolders.Select(x => CreateRepositoryFolderTreeNode(x)).ToList();
            }
            List<TreeNode<BrowserItem>> repositoryItems = [.. repositoryFolderItems, .. suiteItems];

            items.Add(new TreeNode<BrowserItem>
            {
                Text = _loc["test-repository"],
                Children = repositoryItems,
                Expanded = true,
                Value = new BrowserItem() { VirtualFolderName = ROOT_TEST_REPOSITORY },
                Icon = TbIcons.BoldDuoTone.Database,
            });

            // Virtual search folder for test cases
            var testCases = new TreeNode<BrowserItem>
            {
                Text = _loc["test-cases"],
                Expanded = false,
                Expandable = true,
                Value = new BrowserItem() { Href = _appNavigationManager.GetTestCasesUrl() },
                Icon = TbIcons.BoldDuoTone.FolderStar,
            };
            testCases.Children = 
            [
                ..TestBrowserSearchFolders.GetTestCategoryFolders(_loc, _appNavigationManager),
                ..TestBrowserSearchFolders.GetMilestoneFolders(_loc, _appNavigationManager, milestones),
                ..TestBrowserSearchFolders.GetFeatureFolders(_loc, _appNavigationManager, features),
                ..TestBrowserSearchFolders.GetComponentFolders(_loc, _appNavigationManager, components),
            ];
            repositoryItems.Insert(0, testCases);
        }
        if(request.ShowTestRuns)
        {
            var testLabItems = new List<TreeNode<BrowserItem>>();
            items.Add(new TreeNode<BrowserItem>
            {
                Text = _loc["test-lab"],
                Children = testLabItems,
                Expanded = true,
                Expandable = true,
                Value = new BrowserItem() { VirtualFolderName = ROOT_TEST_LAB },
                Icon = TbIcons.Filled.Vial,
            });

            // All test results
            testLabItems.Add(new TreeNode<BrowserItem>
            {
                Text = _loc["test-case-runs"],
                Expanded = false,
                Expandable = false,
                Value = new BrowserItem() { Href = _appNavigationManager.GetTestCaseRunsUrl() },
                Icon = TbIcons.BoldDuoTone.FolderStar,
            });

            // Recent test runs
            List<TreeNode<BrowserItem>> recentRuns = await CreateRecentTestRunsTreeNodeItemsAsync(request, teamId, projectId);
            testLabItems.Add(new TreeNode<BrowserItem>
            {
                Text = _loc["recent-test-runs"],
                Children = recentRuns,
                Expanded = false,
                Expandable = true,
                Value = new BrowserItem() { VirtualFolderName = ROOT_RECENT_TEST_RUNS, Href = _appNavigationManager.GetTestRunsUrl() },
                Icon = TbIcons.BoldDuoTone.FolderStar,
            });

            if (projectId is not null)
            {
                var labFolders = await _testLabController.GetRootFoldersAsync(projectId.Value);
                var labFoldersItems = labFolders.Select(x => CreateLabFolderTreeNode(x)).ToList();
                testLabItems.AddRange(labFoldersItems);
            }

        }
        return items;
    }

    public async Task<Dictionary<long, TestExecutionResultSummary>> GetTestExecutionResultSummaryForRunsAsync(IReadOnlyList<long> testRunsIds, SearchTestCaseRunQuery query)
    {
        var principal = await GetUserClaimsPrincipalAsync();
        return await _testCaseManager.GetTestExecutionResultSummaryForRunsAsync(principal, testRunsIds, query);
    }

    public async Task<PagedResult<TestRun>> GetRecentTestRunsAsync(long? projectId, long? teamId)
    {
        var principal = await GetUserClaimsPrincipalAsync();
        var query = new SearchTestRunQuery() { ProjectId = projectId, Count = 20, TeamId = teamId, Offset = 0 };
        var recentRunsResult = await _testRunManager.SearchTestRunsAsync(principal, query);
        return recentRunsResult;
    }

    private async Task<List<TreeNode<BrowserItem>>> CreateRecentTestRunsTreeNodeItemsAsync(TestBrowserRequest request, long? teamId, long? projectId)
    {
        var recentRunsResult = await GetRecentTestRunsAsync(projectId, teamId);
        List<TreeNode<BrowserItem>> recentRuns = [];
        foreach (var testRun in recentRunsResult.Items)
        {
            recentRuns.Add(await CreateTestRunTreeNode(request, testRun));
        }

        return recentRuns;
    }

    internal async Task<TestSuite?> AddTestSuiteAsync(Team? team, TestProject? project)
    {

        var parameters = new DialogParameters<AddTestSuiteDialog>
        {
            { x => x.Project, project },
            { x => x.Team, team },
        };
        var dialog = await _dialogService.ShowAsync<AddTestSuiteDialog>(null, parameters, DefaultBehaviors.DialogOptions);
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

    internal async Task<long[]> GetTestSuiteSuiteTestsAsync(TestSuite testSuite, bool excludeAutomated)
    {
        var tenantId = await GetTenantIdAsync();
        List<FilterSpecification<TestCase>> specifications = [new FilterByTenant<TestCase>(tenantId), new FilterTestCasesByTestSuite(testSuite.Id)];
        if (excludeAutomated)
        {
            specifications.Add(new FilterTestCasesExcludeAutomated());
        }
        return await _testCaseRepository.SearchTestCaseIdsAsync(specifications);
    }

    public async Task<IReadOnlyList<TestEntity>> ExpandUntilRootAsync(TestCase testCase)
    {
        var principal = await GetUserClaimsPrincipalAsync();
        return await _testCaseManager.ExpandUntilRootAsync(principal, testCase);    
    }

    internal async Task SyncWithActiveDocumentAsync(TestCase selectedTestCase)
    {
        if(_appNavigationManager.UIState.TestTreeView is not null)
        {
            await _appNavigationManager.UIState.TestTreeView.GoToTestCaseAsync(selectedTestCase);
        }
    }

    internal async Task UpdateTestRepositoryFolderAsync(TestRepositoryFolder testRepositoryFolder)
    {
        await _testRepositoryController.UpdateTestRepositoryFolderAsync(testRepositoryFolder);
    }

    internal async Task UpdateTestLabFolderAsync(TestLabFolder testLabFolder)
    {
        await _testLabController.UpdateTestLabFolderAsync(testLabFolder);
    }
}
