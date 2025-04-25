using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Mediator;

using TestBucket.Domain.Projects;

namespace TestBucket.Domain.States.Caching;

public record class GetProjectStateCacheRequest(ClaimsPrincipal Principal, long ProjectId) : IRequest<ProjectStateCacheEntry?>;

public class GetProjectStateCacheHandler : IRequestHandler<GetProjectStateCacheRequest, ProjectStateCacheEntry?>
{
    private readonly IProjectRepository _projectRepository;
    private readonly ProjectStateCache _stateCache;

    public GetProjectStateCacheHandler(IProjectRepository projectRepository, ProjectStateCache stateCache)
    {
        _projectRepository = projectRepository;
        _stateCache = stateCache;
    }

    public async ValueTask<ProjectStateCacheEntry?> Handle(GetProjectStateCacheRequest request, CancellationToken cancellationToken)
    {
        var principal = request.Principal;
        var tenantId = principal.GetTenantIdOrThrow();
        principal.ThrowIfNoPermission(PermissionEntityType.Project, PermissionLevel.Read);

        if(_stateCache.TryGetValue(request.ProjectId, out ProjectStateCacheEntry? entry))
        {
            return entry;
        }

        // Refresh the cache
        var project = await _projectRepository.GetProjectByIdAsync(tenantId, request.ProjectId);
        if (project is not null)
        {
            return _stateCache.Update(project);
        }

        // Project does not exist.
        return null;
    }

}
