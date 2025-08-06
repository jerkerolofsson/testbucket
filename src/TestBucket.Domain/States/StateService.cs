using Mediator;

using TestBucket.Contracts.Issues.States;
using TestBucket.Contracts.Issues.Types;
using TestBucket.Contracts.Requirements.States;
using TestBucket.Contracts.Requirements.Types;
using TestBucket.Contracts.Testing.States;
using TestBucket.Domain.Projects;
using TestBucket.Domain.States.Caching;
using TestBucket.Domain.States.Models;

namespace TestBucket.Domain.States;

/// <summary>
/// Manages the available states of entities
/// 
/// - Test states
/// </summary>
public class StateService : IStateService
{
    private readonly IMediator _mediator;
    private readonly ProjectStateCache _cache;
    private readonly IProjectRepository _projectRepository;
    private readonly IStateRepository _stateRepository;

    public StateService(IMediator mediator, ProjectStateCache cache, IStateRepository stateRepository, IProjectRepository projectRepository)
    {
        _mediator = mediator;
        _cache = cache;
        _stateRepository = stateRepository;
        _projectRepository = projectRepository;
    }

    /// <summary>
    /// Returns the initial state for test cases
    /// </summary>
    /// <param name="principal"></param>
    /// <param name="projectId"></param>
    /// <returns></returns>
    public async Task<TestState> GetTestCaseInitialStateAsync(ClaimsPrincipal principal, long projectId)
    {
        var states = await GetTestCaseRunStatesAsync(principal, projectId);
        states ??= DefaultStates.GetDefaultTestCaseStates();
        var state = states.Where(x => x.IsInitial).FirstOrDefault();
        return state ?? new TestState() { Name = TestCaseStates.Draft, MappedState = MappedTestState.Draft, IsFinal = false, IsInitial = true };
    }
    /// <summary>
    /// Returns the initial state for test case runs
    /// </summary>
    /// <param name="principal"></param>
    /// <param name="projectId"></param>
    /// <returns></returns>
    public async Task<TestState> GetTestCaseRunInitialStateAsync(ClaimsPrincipal principal, long projectId)
    {
        var states = await GetTestCaseRunStatesAsync(principal, projectId);
        states ??= DefaultStates.GetDefaultTestCaseRunStates();
        var state = states.Where(x => x.IsInitial).FirstOrDefault();
        return state ?? new TestState() { Name = TestCaseRunStates.NotStarted, MappedState = MappedTestState.NotStarted, IsFinal = false, IsInitial = true };
    }

    /// <summary>
    /// Returns the final state for test case runs
    /// </summary>
    /// <param name="principal"></param>
    /// <param name="projectId"></param>
    /// <returns></returns>
    public async Task<TestState> GetTestCaseRunFinalStateAsync(ClaimsPrincipal principal, long projectId)
    {
        var states = await GetTestCaseRunStatesAsync(principal, projectId);
        states ??= DefaultStates.GetDefaultTestCaseRunStates();
        var state = states.Where(x => x.IsFinal).FirstOrDefault();
        return state ?? new TestState() { Name = TestCaseRunStates.Completed, MappedState = MappedTestState.Completed, IsFinal = true, IsInitial = false };
    }

    /// <summary>
    /// State for test case runs
    /// </summary>
    /// <param name="principal"></param>
    /// <param name="projectId"></param>
    /// <returns></returns>
    public async Task<IReadOnlyList<TestState>> GetTestCaseStatesAsync(ClaimsPrincipal principal, long projectId)
    {
        var tenantId = principal.GetTenantIdOrThrow();
        principal.ThrowIfNoPermission(PermissionEntityType.Project, PermissionLevel.Read);

        if (!_cache.TryGetValue(projectId, out ProjectStateCacheEntry? entry))
        {
            entry = await RefreshAsync(principal, projectId);
        }
        return entry?.TestCaseStates ?? DefaultStates.GetDefaultTestCaseStates();
    }


    /// <summary>
    /// State for test case runs
    /// </summary>
    /// <param name="principal"></param>
    /// <param name="projectId"></param>
    /// <returns></returns>
    public async Task<IReadOnlyList<TestState>> GetTestCaseRunStatesAsync(ClaimsPrincipal principal, long projectId)
    {
        var tenantId = principal.GetTenantIdOrThrow();
        principal.ThrowIfNoPermission(PermissionEntityType.Project, PermissionLevel.Read);

        if(!_cache.TryGetValue(projectId, out ProjectStateCacheEntry? entry))
        {
            entry = await RefreshAsync(principal, projectId);
        }
        return entry?.TestCaseRunStates ?? DefaultStates.GetDefaultTestCaseRunStates();
    }

    private async Task<ProjectStateCacheEntry> RefreshAsync(ClaimsPrincipal principal, long projectId)
    {
        var tenantId = principal.GetTenantIdOrThrow();
        var project = await _projectRepository.GetProjectByIdAsync(tenantId, projectId);

        var teamId = project?.TeamId ?? 0;
        var request = new RefreshProjectStateCacheRequest(principal, projectId, teamId);
        return await _mediator.Send(request);
    }

    /// <summary>
    /// States for issues
    /// </summary>
    /// <param name="principal"></param>
    /// <param name="projectId"></param>
    /// <returns></returns>
    public async Task<IReadOnlyList<IssueState>> GetIssueStatesAsync(ClaimsPrincipal principal, long projectId)
    {
        var tenantId = principal.GetTenantIdOrThrow();
        principal.ThrowIfNoPermission(PermissionEntityType.Project, PermissionLevel.Read);

        if (!_cache.TryGetValue(projectId, out ProjectStateCacheEntry? entry))
        {
            entry = await RefreshAsync(principal, projectId);
        }
        return entry?.IssueStates ?? DefaultStates.GetDefaultIssueStates();
    }

    /// <summary>
    /// States for requirements
    /// </summary>
    /// <param name="principal"></param>
    /// <param name="projectId"></param>
    /// <returns></returns>
    public async Task<IReadOnlyList<RequirementState>> GetRequirementStatesAsync(ClaimsPrincipal principal, long projectId)
    {
        var tenantId = principal.GetTenantIdOrThrow();
        principal.ThrowIfNoPermission(PermissionEntityType.Project, PermissionLevel.Read);

        if (!_cache.TryGetValue(projectId, out ProjectStateCacheEntry? entry))
        {
            entry = await RefreshAsync(principal, projectId);
        }
        return entry?.RequirementStates ?? DefaultStates.GetDefaultRequirementStates();
    }

    /// <summary>
    /// Types of issues
    /// </summary>
    /// <param name="principal"></param>
    /// <param name="projectId"></param>
    /// <returns></returns>
    public Task<IReadOnlyList<IssueType>> GetIssueTypesAsync(ClaimsPrincipal principal, long projectId)
    {
        var tenantId = principal.GetTenantIdOrThrow();
        principal.ThrowIfNoPermission(PermissionEntityType.Project, PermissionLevel.Read);

        var list = new List<IssueType>();
        foreach (var name in IssueTypes.AllTypes)
        {
            var mapped = IssueTypeConverter.GetMappedIssueTypeFromString(name);
            list.Add(new IssueType { MappedType = mapped, Name = name, Color = IssueTypeColors.GetDefault(mapped) });
        }
        return Task.FromResult<IReadOnlyList<IssueType>>(list);
    }

    /// <summary>
    /// Types of requirements
    /// </summary>
    /// <param name="principal"></param>
    /// <param name="projectId"></param>
    /// <returns></returns>
    public Task<IReadOnlyList<RequirementType>> GetRequirementTypesAsync(ClaimsPrincipal principal, long projectId)
    {
        var tenantId = principal.GetTenantIdOrThrow();
        principal.ThrowIfNoPermission(PermissionEntityType.Project, PermissionLevel.Read);

        var list = new List<RequirementType>();
        foreach(var name in RequirementTypes.AllTypes)
        {
            var mapped = RequirementTypeConverter.GetMappedRequirementTypeFromString(name);
            list.Add(new RequirementType { MappedType = mapped, Name = name });
        }
        return Task.FromResult<IReadOnlyList<RequirementType>>(list);
    }

    public async Task<StateDefinition> GetTenantDefinitionAsync(ClaimsPrincipal principal)
    {
        var tenantId = principal.GetTenantIdOrThrow();
        principal.ThrowIfNoPermission(PermissionEntityType.Project, PermissionLevel.Read);

        var stateDefinition = await _stateRepository.GetTenantStateDefinitionAsync(tenantId);
        if (stateDefinition is null)
        {
            stateDefinition = DefaultStates.CreateDefaultTenantDefinition(tenantId);
            await _stateRepository.AddStateDefinitionAsync(stateDefinition);
            _cache.Clear();
        }
        return stateDefinition;
    }

    public async Task<StateDefinition> GetTeamDefinitionAsync(ClaimsPrincipal principal, long teamId)
    {
        var tenantId = principal.GetTenantIdOrThrow();
        principal.ThrowIfNoPermission(PermissionEntityType.Project, PermissionLevel.Read);

        var stateDefinition = await _stateRepository.GetTeamStateDefinitionAsync(tenantId, teamId);
        if (stateDefinition is null)
        {
            stateDefinition = DefaultStates.CreateDefaultTeamDefinition(tenantId, teamId);
            await _stateRepository.AddStateDefinitionAsync(stateDefinition);
            _cache.Clear();
        }
        return stateDefinition;
    }
    public async Task<StateDefinition> GetProjectDefinitionAsync(ClaimsPrincipal principal, long projectId)
    {
        var tenantId = principal.GetTenantIdOrThrow();
        principal.ThrowIfNoPermission(PermissionEntityType.Project, PermissionLevel.Read);

        var stateDefinition = await _stateRepository.GetProjectStateDefinitionAsync(tenantId, projectId);
        if(stateDefinition is null)
        {
            stateDefinition = DefaultStates.CreateDefaultProjectDefinition(tenantId, projectId);
            await _stateRepository.AddStateDefinitionAsync(stateDefinition);
            _cache.Clear();
        }
        return stateDefinition;
    }

    public async Task DeleteDefinitionAsync(ClaimsPrincipal principal, StateDefinition stateDefinition)
    {
        var tenantId = principal.GetTenantIdOrThrow();
        stateDefinition.TenantId = tenantId;
        principal.ThrowIfNoPermission(PermissionEntityType.Project, PermissionLevel.Delete);

        await _stateRepository.DeleteStateDefinitionAsync(stateDefinition);
        _cache.Clear();
    }
    public async Task SaveDefinitionAsync(ClaimsPrincipal principal, StateDefinition stateDefinition)
    {
        var tenantId = principal.GetTenantIdOrThrow();
        stateDefinition.TenantId = tenantId;
        principal.ThrowIfNoPermission(PermissionEntityType.Project, PermissionLevel.Write);

        await _stateRepository.UpdateStateDefinitionAsync(stateDefinition);
        _cache.Clear();
    }
}
