
using Microsoft.AspNetCore.Mvc;

using MudBlazor;
using NGitLab.Models;

using TestBucket.Components.Environments.Dialogs;
using TestBucket.Components.Projects.Dialogs;
using TestBucket.Data.Migrations;
using TestBucket.Domain.Environments;
using TestBucket.Domain.Environments.Models;
using TestBucket.Domain.Teams.Models;

namespace TestBucket.Components.Environments.Services;

internal class TestEnvironmentController : TenantBaseService
{
    private readonly IDialogService _dialogService;
    private readonly ITestEnvironmentManager _manager;

    public TestEnvironmentController(
        IDialogService dialogService, 
        ITestEnvironmentManager manager,
        AuthenticationStateProvider provider) : base(provider)
    {
        _dialogService = dialogService;
        _manager = manager;
    }

    public async Task<TestEnvironment?> GetTestEnvironmentByIdAsync(long id)
    {
        var principal = await GetUserClaimsPrincipalAsync();
        return await _manager.GetTestEnvironmentByIdAsync(principal, id);
    }

    public async Task<TestEnvironment?> GetDefaultEnvironmentAsync(long projectId)
    {
        var principal = await GetUserClaimsPrincipalAsync();
        return await _manager.GetDefaultTestEnvironmentAsync(principal, projectId);
    }

    public async Task<IReadOnlyList<TestEnvironment>> SetDefaultEnvironmentAsync(long projectId, TestEnvironment testEnvironment)
    {
        var principal = await GetUserClaimsPrincipalAsync();
        testEnvironment.Default = true;
        await UpdateTestEnvironmentAsync(testEnvironment);

        var environments = await GetProjectTestEnvironmentsAsync(projectId);
        foreach (var environment in environments)
        {
            if (environment.Default && environment.Id != testEnvironment.Id)
            {
                environment.Default = false;
                await UpdateTestEnvironmentAsync(environment);
            }
        }
        return environments;
    }

    public async Task UpdateTestEnvironmentAsync(TestEnvironment testEnvironment)
    {
        var principal = await GetUserClaimsPrincipalAsync();
        await _manager.UpdateTestEnvironmentAsync(principal, testEnvironment);
    }

    /// <summary>
    /// Shows a dialog where the user cna add a new test environment
    /// </summary>
    /// <param name="team"></param>
    /// <param name="project"></param>
    /// <returns></returns>
    public async Task<TestEnvironment?> AddTestEnvironmentAsync(Team? team, TestProject? project)
    {
        var principal = await GetUserClaimsPrincipalAsync();

        var parameters = new DialogParameters<AddEnvironmentDialog>
        {
            { x=>x.Team, team },
            { x=>x.Project, project },
        };
        var dialog = await _dialogService.ShowAsync<AddEnvironmentDialog>(null, parameters, DefaultBehaviors.DialogOptions);
        var result = await dialog.Result;
        if (result?.Data is TestEnvironment testEnvironment)
        {
            await _manager.AddTestEnvironmentAsync(principal, testEnvironment);
            return testEnvironment;
        }
        return null;
    }

    /// <summary>
    /// Returns all environments for the current tenant
    /// </summary>
    /// <returns></returns>
    public async Task<IReadOnlyList<TestEnvironment>> GetTestEnvironmentsAsync()
    {
        var principal = await GetUserClaimsPrincipalAsync();
        return await _manager.GetTestEnvironmentsAsync(principal);  
    }

    /// <summary>
    /// Returns all environments for the specified project
    /// </summary>
    /// <returns></returns>
    public async Task<IReadOnlyList<TestEnvironment>> GetProjectTestEnvironmentsAsync(long projectId)
    {
        var principal = await GetUserClaimsPrincipalAsync();
        return await _manager.GetProjectTestEnvironmentsAsync(principal, projectId);
    }
}
