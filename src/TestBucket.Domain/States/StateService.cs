using Mediator;

using TestBucket.Contracts.Issues.States;
using TestBucket.Contracts.Issues.Types;
using TestBucket.Contracts.Requirements.States;
using TestBucket.Contracts.Requirements.Types;
using TestBucket.Contracts.Testing.States;
using TestBucket.Domain.States.Caching;

namespace TestBucket.Domain.States;

/// <summary>
/// Manages the available states 
/// </summary>
public class StateService : IStateService
{
    private readonly IMediator _mediator;
    private readonly ProjectStateCache _cache;

    public StateService(IMediator mediator, ProjectStateCache cache)
    {
        _mediator = mediator;
        _cache = cache;
    }


    public async Task<TestState> GetProjectInitialStateAsync(ClaimsPrincipal principal, long projectId)
    {
        var states = await GetTestCaseRunStatesAsync(principal, projectId);
        states ??= DefaultStates.GetDefaultTestCaseRunStates();
        var state = states.Where(x => x.IsInitial).FirstOrDefault();
        return state ?? new TestState() { Name = TestStates.NotStarted, MappedState = MappedTestState.NotStarted, IsFinal = false, IsInitial = true };
    }

    public async Task<TestState> GetProjectFinalStateAsync(ClaimsPrincipal principal, long projectId)
    {
        var states = await GetTestCaseRunStatesAsync(principal, projectId);
        states ??= DefaultStates.GetDefaultTestCaseRunStates();
        var state = states.Where(x => x.IsFinal).FirstOrDefault();
        return state ?? new TestState() { Name = TestStates.Completed, MappedState = MappedTestState.Completed, IsFinal = true, IsInitial = false };
    }

    public async Task<IReadOnlyList<TestState>> GetTestCaseRunStatesAsync(ClaimsPrincipal principal, long projectId)
    {
        var tenantId = principal.GetTenantIdOrThrow();
        principal.ThrowIfNoPermission(PermissionEntityType.Project, PermissionLevel.Read);

        if(!_cache.TryGetValue(projectId, out ProjectStateCacheEntry? entry))
        {
            entry = await _mediator.Send(new RefreshProjectStateCacheRequest(principal, projectId));
        }
        return entry?.TestStates ?? DefaultStates.GetDefaultTestCaseRunStates();
    }
    public async Task<IReadOnlyList<IssueState>> GetIssueStatesAsync(ClaimsPrincipal principal, long projectId)
    {
        var tenantId = principal.GetTenantIdOrThrow();
        principal.ThrowIfNoPermission(PermissionEntityType.Project, PermissionLevel.Read);

        if (!_cache.TryGetValue(projectId, out ProjectStateCacheEntry? entry))
        {
            entry = await _mediator.Send(new RefreshProjectStateCacheRequest(principal, projectId));
        }
        return entry?.IssueStates ?? DefaultStates.GetDefaultIssueStates();
    }

    public async Task<IReadOnlyList<RequirementState>> GetRequirementStatesAsync(ClaimsPrincipal principal, long projectId)
    {
        var tenantId = principal.GetTenantIdOrThrow();
        principal.ThrowIfNoPermission(PermissionEntityType.Project, PermissionLevel.Read);

        if (!_cache.TryGetValue(projectId, out ProjectStateCacheEntry? entry))
        {
            entry = await _mediator.Send(new RefreshProjectStateCacheRequest(principal, projectId));
        }
        return entry?.RequirementStates ?? DefaultStates.GetDefaultRequirementStates();
    }

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
}
