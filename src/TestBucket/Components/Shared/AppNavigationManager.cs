using NGitLab.Models;

using TestBucket.Domain.Requirements.Models;
using TestBucket.Domain.Tenants.Models;
using TestBucket.Domain.Testing.Models;

namespace TestBucket.Components.Shared;

/// <summary>
/// Contains helper functions for navigations within the application
/// </summary>
public class AppNavigationManager
{
    private readonly NavigationManager _navigationManager;

    internal class NavigationState
    {
        /// <summary>
        /// Selected test case
        /// </summary>
        public TestCase? SelectedTestCase { get; set; }

        /// <summary>
        /// Selected test suite
        /// </summary>
        public TestSuite? SelectedTestSuite { get; set; }

        /// <summary>
        /// Selected test suite
        /// </summary>
        public TestSuiteFolder? SelectedTestSuiteFolder { get; set; }

        /// <summary>
        /// Selected project
        /// </summary>
        public TestProject? SelectedProject { get; set; }
    }

    internal NavigationState State { get; } = new();

    public AppNavigationManager(NavigationManager navigationManager)
    {
        _navigationManager = navigationManager;
    }

    public string GetImportSpecificationsUrl()
    {
        var tenantId = TenantResolver.ResolveTenantIdFromUrl(_navigationManager.Uri);
        return $"/{tenantId}/Requirements/Import";
    }

    public string GetUrl(Requirement requirement)
    {
        var tenantId = TenantResolver.ResolveTenantIdFromUrl(_navigationManager.Uri);
        return $"/{tenantId}/Requirements/Requirements/{requirement.Id}";
    }
    public string GetUrl(RequirementSpecification spec)
    {
        var tenantId = TenantResolver.ResolveTenantIdFromUrl(_navigationManager.Uri);
        return $"/{tenantId}/Requirements/Specifications/{spec.Id}";
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

    public string GetUrl(TestProject project)
    {
        var tenantId = TenantResolver.ResolveTenantIdFromUrl(_navigationManager.Uri);
        return $"/{tenantId}/Settings/Projects/{project.Slug}";
    }

    public void NavigateTo(Requirement requirement, bool forceLoad = false)
    {
        var url = GetUrl(requirement);
        _navigationManager.NavigateTo(url, forceLoad);
    }
    public void NavigateTo(RequirementSpecification spec, bool forceLoad = false)
    {
        var url = GetUrl(spec);
        _navigationManager.NavigateTo(url, forceLoad);
    }

    public void NavigateTo(TestRun testRun, bool forceLoad = false)
    {
        var url = GetUrl(testRun);
        _navigationManager.NavigateTo(url, forceLoad);
    }

    public void NavigateTo(TestSuiteFolder folder, bool forceLoad = false)
    {
        this.State.SelectedTestSuiteFolder = folder;
        this.State.SelectedTestCase = null;
        var url = GetUrl(folder);
        _navigationManager.NavigateTo(url, forceLoad);
    }

    public void NavigateTo(TestSuite suite, bool forceLoad = false)
    {
        this.State.SelectedTestSuite = suite;
        this.State.SelectedTestSuiteFolder = null;
        this.State.SelectedTestCase = null;
        var url = GetUrl(suite);
        _navigationManager.NavigateTo(url, forceLoad);
    }
    public void NavigateTo(TestCase testCase, bool forceLoad = false)
    {
        this.State.SelectedTestCase = testCase;
        var url = GetUrl(testCase);
        _navigationManager.NavigateTo(url, forceLoad);
    }

    public void NavigateTo(TestProject project, bool forceLoad = false)
    {
        var url = GetUrl(project);
        _navigationManager.NavigateTo(url, forceLoad);
    }
}
