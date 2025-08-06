using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Mediator;

using TestBucket.Domain.Projects;

namespace TestBucket.Domain.States.Caching;

public record class RefreshProjectStateCacheRequest(ClaimsPrincipal Principal, long ProjectId, long TeamId) : IRequest<ProjectStateCacheEntry>;

public class RefreshProjectStateCacheHandler : IRequestHandler<RefreshProjectStateCacheRequest, ProjectStateCacheEntry>
{
    private readonly IStateRepository _repository;
    private readonly ProjectStateCache _stateCache;

    public RefreshProjectStateCacheHandler(IStateRepository projectRepository, ProjectStateCache stateCache)
    {
        _repository = projectRepository;
        _stateCache = stateCache;
    }

    public async ValueTask<ProjectStateCacheEntry> Handle(RefreshProjectStateCacheRequest request, CancellationToken cancellationToken)
    {
        var principal = request.Principal;
        var tenantId = principal.GetTenantIdOrThrow();
        principal.ThrowIfNoPermission(PermissionEntityType.Project, PermissionLevel.Read);

        var projectDefinition = await _repository.GetProjectStateDefinitionAsync(tenantId, request.ProjectId);
        var teamDefinition = await _repository.GetTeamStateDefinitionAsync(tenantId, request.TeamId);
        var tenantDefinition = await _repository.GetTenantStateDefinitionAsync(tenantId);

        return _stateCache.Update(projectDefinition, teamDefinition, tenantDefinition, request.ProjectId);
    }
}
