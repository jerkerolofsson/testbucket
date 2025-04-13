using Microsoft.AspNetCore.Components;

using TestBucket.Components.Tests.TestCases.Controls;
using TestBucket.Domain.Automation.Models;
using TestBucket.Domain.Environments.Models;
using TestBucket.Domain.Requirements.Models;
using TestBucket.Domain.Tenants.Models;
using TestBucket.Domain.TestAccounts.Models;
using TestBucket.Domain.Testing.Models;
using TestBucket.Domain.TestResources.Models;

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

        public TestTreeView? TestTreeView { get; set; }
    }

    internal NavigationState State { get; } = new();

    public AppNavigationManager(NavigationManager navigationManager)
    {
        _navigationManager = navigationManager;
    }

    /// <summary>
    /// This depends on the route convention in the format:
    /// /{tenantId}/Testing/EntityType/{entityId}/{SubPage}
    /// </summary>
    /// <returns></returns>
    public long? GetEntityIdFromCurrentUri()
    {
        return ResolveEntityIdFromUrl(_navigationManager.Uri);
    }

    public static long? ResolveEntityIdFromPath(string? path)
    {
        if (path is null)
        {
            return null;
        }
        path = path.Split('?')[0].Split('#')[0];

        var pathItems = path.Trim('/').Split('/');
        if (pathItems.Length >= 4)
        {
            if (long.TryParse(pathItems[3], out var testCaseId))
            {
                return testCaseId;
            }
        }
        return null;
    }
    public static long? ResolveEntityIdFromUrl(string url)
    {
        if (Uri.TryCreate(url, UriKind.Absolute, out var uri))
        {
            return ResolveEntityIdFromPath(uri.PathAndQuery);
        }
        return null;
    }

    public string GetSettingsUrl()
    {
        var tenantId = TenantResolver.ResolveTenantIdFromUrl(_navigationManager.Uri);
        return $"/{tenantId}/Settings";
    }
    public string GetProfileSettingsUrl()
    {
        var tenantId = TenantResolver.ResolveTenantIdFromUrl(_navigationManager.Uri);
        return $"/{tenantId}/Settings/Categories/Profile";
    }
    public string GetManageProjectsUrl()
    {
        var tenantId = TenantResolver.ResolveTenantIdFromUrl(_navigationManager.Uri);
        return $"/{tenantId}/Settings/ManageProjects";
    }
    public string GetTestAccountsSettingsUrl()
    {
        var tenantId = TenantResolver.ResolveTenantIdFromUrl(_navigationManager.Uri);
        return $"/{tenantId}/Settings/Accounts";
    }
    public string GetTestEnvironmentSettingsUrl()
    {
        var tenantId = TenantResolver.ResolveTenantIdFromUrl(_navigationManager.Uri);
        return $"/{tenantId}/Settings/ManageEnvironments";
    }
    public string GetTestResourcesSettingsUrl()
    {
        var tenantId = TenantResolver.ResolveTenantIdFromUrl(_navigationManager.Uri);
        return $"/{tenantId}/Settings/Resources";
    }

    public string GetImportSpecificationsUrl()
    {
        var tenantId = TenantResolver.ResolveTenantIdFromUrl(_navigationManager.Uri);
        return $"/{tenantId}/Requirements/Import";
    }

    public string GetTestSuiteVariablesUrl()
    {
        var tenantId = TenantResolver.ResolveTenantIdFromUrl(_navigationManager.Uri);
        var testSuiteId = ResolveEntityIdFromUrl(_navigationManager.Uri);
        return $"{tenantId}/Testing/TestSuites/{testSuiteId}/Variables";
    }
    public string GetTestCaseVariablesUrl()
    {
        var tenantId = TenantResolver.ResolveTenantIdFromUrl(_navigationManager.Uri);
        var testCaseId = ResolveEntityIdFromUrl(_navigationManager.Uri);
        return $"{tenantId}/Testing/TestCases/{testCaseId}/Variables";
    }
    public string GetTestSuiteFieldsUrl()
    {
        var tenantId = TenantResolver.ResolveTenantIdFromUrl(_navigationManager.Uri);
        var testCaseId = ResolveEntityIdFromUrl(_navigationManager.Uri);
        return $"{tenantId}/Testing/TestSuites/{testCaseId}/Fields";
    }
    public string GetTestCaseFieldsUrl()
    {
        var tenantId = TenantResolver.ResolveTenantIdFromUrl(_navigationManager.Uri);
        var testCaseId = ResolveEntityIdFromUrl(_navigationManager.Uri);
        return $"{tenantId}/Testing/TestCases/{testCaseId}/Fields";
    }
    public string GetTestSuiteSettingsUrl()
    {
        var tenantId = TenantResolver.ResolveTenantIdFromUrl(_navigationManager.Uri);
        var testCaseId = ResolveEntityIdFromUrl(_navigationManager.Uri);
        return $"{tenantId}/Testing/TestSuites/{testCaseId}/Settings";
    }
    public string GetTestSuiteAttachmentsUrl()
    {
        var tenantId = TenantResolver.ResolveTenantIdFromUrl(_navigationManager.Uri);
        var testCaseId = ResolveEntityIdFromUrl(_navigationManager.Uri);
        return $"{tenantId}/Testing/TestSuites/{testCaseId}/Attachments";
    }
    public string GetTestCaseAttachmentsUrl()
    {
        var tenantId = TenantResolver.ResolveTenantIdFromUrl(_navigationManager.Uri);
        var testCaseId = ResolveEntityIdFromUrl(_navigationManager.Uri);
        return $"{tenantId}/Testing/TestCases/{testCaseId}/Attachments";
    }

    /// <summary>
    /// Map requirements to test cases
    /// </summary>
    /// <returns></returns>
    public string GetTestCaseRequimentsUrl()
    {
        var tenantId = TenantResolver.ResolveTenantIdFromUrl(_navigationManager.Uri);
        var testCaseId = ResolveEntityIdFromUrl(_navigationManager.Uri);
        return $"{tenantId}/Testing/TestCases/{testCaseId}/requirements";
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

    public string GetUrl(SearchTestCaseRunQuery query)
    {
        var tenantId = TenantResolver.ResolveTenantIdFromUrl(_navigationManager.Uri);
        return $"/{tenantId}/Testing/TestRuns/{query.TestRunId}/Tests?{query.ToQueryString()}";
    }

    public string GetUrl(TestRun testrun)
    {
        var tenantId = TenantResolver.ResolveTenantIdFromUrl(_navigationManager.Uri);
        return $"/{tenantId}/Testing/TestRuns/{testrun.Id}";
    }
    public string GetUrl(Pipeline pipeline)
    {
        var tenantId = TenantResolver.ResolveTenantIdFromUrl(_navigationManager.Uri);
        return $"/{tenantId}/Testing/TestRuns/{pipeline.TestRunId}/Pipelines/{pipeline.Id}";
    }
    public string GetUrl(Pipeline pipeline, PipelineJob job)
    {
        var tenantId = TenantResolver.ResolveTenantIdFromUrl(_navigationManager.Uri);
        return $"/{tenantId}/Testing/TestRuns/{pipeline.TestRunId}/Pipelines/{pipeline.Id}/Jobs/{job.Id}";
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
    public string GetUrl(ApplicationUser user)
    {
        var tenantId = TenantResolver.ResolveTenantIdFromUrl(_navigationManager.Uri);
        return $"/{tenantId}/Settings/Users/{user.NormalizedUserName}";
    }
    public string GetUrl(TestEnvironment testEnvironment)
    {
        return $"{GetTestEnvironmentSettingsUrl()}/{testEnvironment.Id}";
    }
    public string GetUrl(TestResource resource)
    {
        var tenantId = TenantResolver.ResolveTenantIdFromUrl(_navigationManager.Uri);
        return $"/{tenantId}/Settings/Resources/{resource.Id}";
    }
    public string GetUrl(TestAccount account)
    {
        var tenantId = TenantResolver.ResolveTenantIdFromUrl(_navigationManager.Uri);
        return $"/{tenantId}/Settings/Accounts/{account.Id}";
    }

    public void NavigateTo(ApplicationUser user, bool forceLoad = false)
    {
        var url = GetUrl(user);
        _navigationManager.NavigateTo(url, forceLoad);
    }
    public void NavigateTo(TestAccount account, bool forceLoad = false)
    {
        var url = GetUrl(account);
        _navigationManager.NavigateTo(url, forceLoad);
    }
    public void NavigateTo(TestResource resource, bool forceLoad = false)
    {
        var url = GetUrl(resource);
        _navigationManager.NavigateTo(url, forceLoad);
    }
    public void NavigateTo(TestEnvironment testEnvironment, bool forceLoad = false)
    {
        var url = GetUrl(testEnvironment);
        _navigationManager.NavigateTo(url, forceLoad);
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
    public void NavigateTo(Pipeline pipeline, bool forceLoad = false)
    {
        var url = GetUrl(pipeline);
        _navigationManager.NavigateTo(url, forceLoad);
    }
    public void NavigateTo(Pipeline pipeline,  PipelineJob job, bool forceLoad = false)
    {
        var url = GetUrl(pipeline, job);
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
