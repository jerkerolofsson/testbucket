using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Mediator;

using TestBucket.Domain.Projects;

namespace TestBucket.Domain.States.Caching;

public record class RefreshProjectStateCacheRequest(ClaimsPrincipal Principal, long ProjectId) : IRequest<ProjectStateCacheEntry?>;

public class RefreshProjectStateCacheHandler : IRequestHandler<RefreshProjectStateCacheRequest, ProjectStateCacheEntry?>
{
    private readonly IProjectRepository _projectRepository;
    private readonly ProjectStateCache _stateCache;

    public RefreshProjectStateCacheHandler(IProjectRepository projectRepository, ProjectStateCache stateCache)
    {
        _projectRepository = projectRepository;
        _stateCache = stateCache;
    }

    public async ValueTask<ProjectStateCacheEntry?> Handle(RefreshProjectStateCacheRequest request, CancellationToken cancellationToken)
    {
        var principal = request.Principal;
        var tenantId = principal.GetTenantIdOrThrow();
        principal.ThrowIfNoPermission(PermissionEntityType.Project, PermissionLevel.Read);

        var project = await _projectRepository.GetProjectByIdAsync(tenantId, request.ProjectId);

        if (project is not null)
        {
            return _stateCache.Update(project);
        }

        return null;
    }
}
