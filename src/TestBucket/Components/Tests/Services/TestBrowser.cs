using System.Collections.Concurrent;

using TestBucket.Components.Tests.Browser.Controls;
using TestBucket.Components.Tests.Dialogs;
using TestBucket.Domain.Files;
using TestBucket.Domain.Teams.Models;

namespace TestBucket.Components.Tests.Services;
internal class TestBrowser : TenantBaseService
{
    private readonly TestSuiteService _testSuiteService;
    private readonly IDialogService _dialogService;
    private readonly ITextTestResultsImporter _textImporter;
    private readonly IFileRepository _fileRepository;

    public TestBrowser(TestSuiteService testSuiteService,
        AuthenticationStateProvider authenticationStateProvider,
        IDialogService dialogService,
        ITextTestResultsImporter textImporter,
        IFileRepository fileRepository) : base(authenticationStateProvider)

    {
        _testSuiteService = testSuiteService;
        _dialogService = dialogService;
        _textImporter = textImporter;
        _fileRepository = fileRepository;
    }

    public async Task ImportAsync(Team? team, TestProject? project)
    {
        var tenantId = await GetTenantIdAsync();

        var dialog = await _dialogService.ShowAsync<ImportResultsDialog>(null);
        var result = await dialog.Result;

        if (result?.Data is ImportOptions importOptions && importOptions.File?.Id is not null)
        {
            var resource = await _fileRepository.GetResourceByIdAsync(tenantId, importOptions.File.Id);
            if (resource is not null)
            {
                string xml = Encoding.UTF8.GetString(resource.Data);
                await _textImporter.ImportTextAsync(tenantId, team?.Id, project?.Id, importOptions.Format, xml);
            }
        }
    }

    public async Task<PagedResult<TestCase>> SearchTestCasesAsync(long? testSuiteId, long? folderId, string? searchText, int offset, int count = 20)
    {
        if(string.IsNullOrWhiteSpace(searchText))
        {
            searchText = null;
        }

        return await _testSuiteService.SearchTestCasesAsync(new SearchTestQuery
        {
            Text = searchText,
            TestSuiteId = testSuiteId,
            FolderId = folderId,
            Count = count,
            Offset = offset,
        });
    }

    public async Task<List<TreeItemData<BrowserItem>>> BrowseAsync(long? teamId, long? projectId, BrowserItem? parent)
    {
        if(parent is not null)
        {
            if(parent.TestCase is not null)
            {
                return [];
            }
            if(parent.Folder is not null)
            {
                var folders = await _testSuiteService.GetTestSuiteFoldersAsync(projectId, parent.Folder.TestSuiteId, parent.Folder.Id);
                var items = MapFoldersToTreeItemData(folders);

                var tests = await _testSuiteService.SearchTestCasesAsync(new SearchTestQuery
                {
                    Count = 1_000,
                    Offset = 0,
                    FolderId = parent.Folder.Id,
                    TestSuiteId = parent.Folder.TestSuiteId,
                });
                items.AddRange(MapTestsToTreeItemData(tests.Items));

                return items;
            }
            else if(parent.TestSuite is not null)
            {
                long? parentFolderId = null;
                var folders = await _testSuiteService.GetTestSuiteFoldersAsync(projectId, parent.TestSuite.Id, parentFolderId);
                var items = MapFoldersToTreeItemData(folders);
                return items;
            }
        }
        return await BrowseRootAsync(teamId, projectId);
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

    private List<TreeItemData<BrowserItem>> MapTestsToTreeItemData(TestCase[] tests)
    {
        return tests.Select(x => CreateTreeItemDataFromTestCase(x)).ToList();
    }
    private List<TreeItemData<BrowserItem>> MapFoldersToTreeItemData(TestSuiteFolder[] folders)
    {
        return folders.Select(x => CreateTreeItemDataFromFolder(x)).ToList();
    }

    public TreeItemData<BrowserItem> CreateTreeItemDataFromTestCase(TestCase x)
    {
        return new TreeItemData<BrowserItem>
        {
            Value = new BrowserItem { TestCase = x },
            Text = x.Name,
            Icon = Icons.Material.Filled.PlaylistAddCheck,
            Expandable = false,
            Children = null,
        };
    }
    public TreeItemData<BrowserItem> CreateTreeItemDataFromFolder(TestSuiteFolder x)
    {
        return new TreeItemData<BrowserItem>
        {
            Value = new BrowserItem { Folder = x, Color = x.Color },
            Text = x.Name,
            Icon = x.Icon ?? Icons.Material.Filled.Folder,
            Expandable = true,
            Children = null,
        };
    }

    private async Task<List<TreeItemData<BrowserItem>>> BrowseRootAsync(long? teamId, long? projectId)
    {
        var suites = await _testSuiteService.GetTestSuitesAsync(teamId, projectId);
        var suiteItems = suites.Items.Select(x => 
            new TreeItemData<BrowserItem>
            {
                Value = new BrowserItem { TestSuite = x, Color = x.Color },
                Text = x.Name,
                Icon = x.Icon ?? Icons.Material.Filled.Article,
                Children = null,
            }).ToList();

        return new List<TreeItemData<BrowserItem>>
        {
            new TreeItemData<BrowserItem>
            {
                Text = "Test Suites",
                Children = suiteItems,
                Expanded = true,
                Icon = Icons.Material.Filled.FolderSpecial,
            },
            new TreeItemData<BrowserItem>
            {
                Text = "Test Runs",
                Expanded = false,
                Icon = Icons.Material.Filled.FolderSpecial,
            },
            new TreeItemData<BrowserItem>
            {
                Text = "Reports",
                Expanded = false,
                Icon = Icons.Material.Filled.FolderSpecial,
            },
            new TreeItemData<BrowserItem>
            {
                Text = "Environments",
                Expanded = false,
                Icon = Icons.Material.Filled.FolderSpecial,
            },
            new TreeItemData<BrowserItem>
            {
                Text = "Test Parameters",
                Expanded = false,
                Icon = Icons.Material.Filled.FolderSpecial,
            }
        };
    }
}
