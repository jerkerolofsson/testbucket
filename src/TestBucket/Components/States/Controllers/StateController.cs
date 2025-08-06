
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Localization;

using TestBucket.Components.Projects.Dialogs;
using TestBucket.Components.States.Dialogs;
using TestBucket.Contracts.Issues.States;
using TestBucket.Contracts.Requirements.States;
using TestBucket.Contracts.States;
using TestBucket.Contracts.Testing.States;
using TestBucket.Domain.States;
using TestBucket.Domain.States.Models;
using TestBucket.Localization;

namespace TestBucket.Components.States.Controllers;

internal class StateController : TenantBaseService
{
    private readonly IStateService _stateService;
    private readonly IDialogService _dialogService;
    private readonly IStringLocalizer<SharedStrings> _loc;

    public StateController(AuthenticationStateProvider authenticationStateProvider, IStateService stateService, IDialogService dialogService, IStringLocalizer<SharedStrings> loc) : base(authenticationStateProvider)
    {
        _stateService = stateService;
        _dialogService = dialogService;
        _loc = loc;
    }

    public async Task AddStateAsync(StateDefinition stateDefinition, StateEntityType entityType)
    {
        var hasPermission = await ShowErrorIfNoPermissionAsync(_loc, _dialogService, PermissionEntityType.Project, PermissionLevel.Write);
        if (!hasPermission)
            return;

        BaseState newState = StateDefinition.Create(entityType);
        newState.Name = "New State";
        newState.Color = "#118AB2";

        var parameters = new DialogParameters<EditStateDialog>
        {
            { x=>x.State, newState }
        };
        var dialog = await _dialogService.ShowAsync<EditStateDialog>(null, parameters, DefaultBehaviors.DialogOptions);
        var result = await dialog.Result;
        if (result?.Data is BaseState state)
        {
            stateDefinition.Add(entityType, state);
        }
    }
    public async Task EditStateAsync(StateDefinition stateDefinition, StateEntityType entityType, BaseState state)
    {
        var hasPermission = await ShowErrorIfNoPermissionAsync(_loc, _dialogService, PermissionEntityType.Project, PermissionLevel.Write);
        if (!hasPermission)
            return;

        var parameters = new DialogParameters<EditStateDialog>
        {
            { x=>x.State, state }
        };
        var dialog = await _dialogService.ShowAsync<EditStateDialog>(null, parameters, DefaultBehaviors.DialogOptions);
        var result = await dialog.Result;
        if (result?.Data is BaseState editedState)
        {
            stateDefinition.Remove(entityType, state);
            stateDefinition.Add(entityType, editedState);
        }
    }

    public async Task SaveStateDefinitionAsync(StateDefinition stateDefinition)
    {
        var principal = await GetUserClaimsPrincipalAsync();
        await _stateService.SaveDefinitionAsync(principal, stateDefinition);
    }

    public async Task<StateDefinition> GetTenantStateDefinitionAsync()
    {
        var principal = await GetUserClaimsPrincipalAsync();
        return await _stateService.GetTenantDefinitionAsync(principal);
    }

    public async Task<StateDefinition> GetTeamStateDefinitionAsync(long teamId)
    {
        var principal = await GetUserClaimsPrincipalAsync();
        return await _stateService.GetTeamDefinitionAsync(principal, teamId);
    }
    public async Task<StateDefinition> GetProjectStateDefinitionAsync(long projectId)
    {
        var principal = await GetUserClaimsPrincipalAsync();
        return await _stateService.GetProjectDefinitionAsync(principal, projectId);
    }

    public async Task<IReadOnlyList<IssueState>> GetIssueStatesAsync(long projectId)
    {
        var principal = await GetUserClaimsPrincipalAsync();
        return await _stateService.GetIssueStatesAsync(principal, projectId);
    }
    public async Task<IReadOnlyList<RequirementState>> GetRequirementStatesAsync(long projectId)
    {
        var principal = await GetUserClaimsPrincipalAsync();
        return await _stateService.GetRequirementStatesAsync(principal, projectId);
    }
    public async Task<IReadOnlyList<TestState>> GetTestCaseStatesAsync(long projectId)
    {
        var principal = await GetUserClaimsPrincipalAsync();
        return await _stateService.GetTestCaseStatesAsync(principal, projectId);
    }
    public async Task<IReadOnlyList<TestState>> GetTestCaseRunStatesAsync(long projectId)
    {
        var principal = await GetUserClaimsPrincipalAsync();
        return await _stateService.GetTestCaseRunStatesAsync(principal, projectId);
    }
}
