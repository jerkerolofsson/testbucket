
using MudBlazor;

using TestBucket.Components.Tenants;
using TestBucket.Components.Tests.Models;
using TestBucket.Contracts;
using TestBucket.Data.Testing.Models;

namespace TestBucket.Components.Tests.Services;

internal class TestBrowser
{
    private readonly TestSuiteService _testSuiteService;

    public TestBrowser(TestSuiteService testSuiteService)
    {
        _testSuiteService = testSuiteService;
    }

    public async Task<PagedResult<TestCase>> SearchTestCasesAsync(long? testSuiteId, long? folderId, int offset, int count = 20)
    {
        return await _testSuiteService.SearchTestCasesAsync(new SearchTestQuery
        {
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
        return suites.Items.Select(x => 
            new TreeItemData<BrowserItem>
            {
                Value = new BrowserItem { TestSuite = x },
                Text = x.Name,
                Icon = Icons.Material.Filled.Article,
                Children = null,
            }).ToList();
    }
}
