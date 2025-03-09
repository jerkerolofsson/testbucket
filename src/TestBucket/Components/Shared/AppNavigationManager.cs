namespace TestBucket.Components.Shared;

public class AppNavigationManager
{
    private readonly NavigationManager _navigationManager;

    public AppNavigationManager(NavigationManager navigationManager)
    {
        _navigationManager = navigationManager;
    }

    public string GetUrl(TestRun testrun)
    {
        var tenantId = TenantResolver.ResolveTenantIdFromUrl(_navigationManager.Uri);
        return $"/{tenantId}/Testing/TestRuns/{testrun.Id}";
    }
    public string GetUrl(TestSuiteFolder testSuiteFolder)
    {
        var tenantId = TenantResolver.ResolveTenantIdFromUrl(_navigationManager.Uri);
        return $"/{tenantId}/Testing/TestSuites/{testSuiteFolder.TestSuiteId}/Folders/{testSuiteFolder.Id}";
    }
    public string GetFolderUrl(long testSuiteId, long folderId)
    {
        var tenantId = TenantResolver.ResolveTenantIdFromUrl(_navigationManager.Uri);
        return $"/{tenantId}/Testing/TestSuites/{testSuiteId}/Folders/{folderId}";
    }

    public string GetUrl(TestSuite testSuite)
    {
        var tenantId = TenantResolver.ResolveTenantIdFromUrl(_navigationManager.Uri);
        return $"/{tenantId}/Testing/TestSuites/{testSuite.Id}";
    }

    public string GetUrl(TestCase testCase)
    {
        var tenantId = TenantResolver.ResolveTenantIdFromUrl(_navigationManager.Uri);
        return $"/{tenantId}/Testing/TestCases/{testCase.Id}";
    }

    public void NavigateTo(TestCase testCase, bool forceLoad = false)
    {
        var url = GetUrl(testCase);
        _navigationManager.NavigateTo(url, forceLoad);
    }
}
