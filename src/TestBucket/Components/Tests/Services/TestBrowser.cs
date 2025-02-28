
namespace TestBucket.Components.Tests.Services;

internal class TestBrowser : TenantBaseService
{
    private readonly TestSuiteService _testSuiteService;
    private readonly ITextTestResultsImporter _textImporter;
    public TestBrowser(TestSuiteService testSuiteService,
        AuthenticationStateProvider authenticationStateProvider,
        ITextTestResultsImporter textImporter) : base(authenticationStateProvider)

    {
        _testSuiteService = testSuiteService;
        _textImporter = textImporter;
    }

    public async Task ImportAsync(TestProject? project)
    {
        var tenantId = await GetTenantIdAsync();
        string xml = await File.ReadAllTextAsync(@"c:\temp\junit.xml");
        await _textImporter.ImportTextAsync(tenantId, project?.Id, Domain.Testing.Formats.TestResultFormat.JUnitXml, xml);
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

    public async Task<List<TreeItemData<BrowserItem>>> BrowseAsync(long? projectId, BrowserItem? parent)
    {
        if(parent is not null)
        {
            if(parent.Folder is not null)
            {
                var folders = await _testSuiteService.GetTestSuiteFoldersAsync(projectId, parent.Folder.TestSuiteId, parent.Folder.Id);
                return MapFoldersToTreeItemData(folders);
            }
            else if(parent.TestSuite is not null)
            {
                long? parentFolderId = null;
                var folders = await _testSuiteService.GetTestSuiteFoldersAsync(projectId, parent.TestSuite.Id, parentFolderId);
                return MapFoldersToTreeItemData(folders);
            }
        }
        return await BrowseRootAsync(projectId);
    }

    private List<TreeItemData<BrowserItem>> MapFoldersToTreeItemData(TestSuiteFolder[] folders)
    {
        return folders.Select(x => CreateTreeItemDataFromFolder(x)).ToList();
    }

    public TreeItemData<BrowserItem> CreateTreeItemDataFromFolder(TestSuiteFolder x)
    {
        return new TreeItemData<BrowserItem>
        {
            Value = new BrowserItem { Folder = x },
            Text = x.Name,
            Icon = Icons.Material.Filled.Folder,
            Children = null,
        };
    }

    private async Task<List<TreeItemData<BrowserItem>>> BrowseRootAsync(long? projectId)
    {
        var suites = await _testSuiteService.GetTestSuitesAsync(projectId);
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
