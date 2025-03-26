using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using TestBucket.Domain.Identity.Models;

using TestBucket.Domain.Identity;
using TestBucket.Domain.Tenants.Models;
using TestBucket.Contracts.Integrations;
using Microsoft.Extensions.Caching.Memory;
using TestBucket.Traits.Core;
using TestBucket.Contracts.Projects;
using TestBucket.Domain.Projects.Models;

namespace TestBucket.Domain.Projects;
internal class ProjectManager : IProjectManager
{
    private readonly IProjectRepository _projectRepository;
    private readonly IMemoryCache _memoryCache;
    private readonly List<IProjectDataSource> _projectDataSources;

    public ProjectManager(IProjectRepository projectRepository,
        IMemoryCache memoryCache,
        IEnumerable<IProjectDataSource> projectDataSources)
    {
        _projectDataSources = projectDataSources.ToList();
        _projectRepository = projectRepository;
        _memoryCache = memoryCache;
    }

    public Task AddAsync(ClaimsPrincipal principal, TestProject project)
    {
        principal.ThrowIfNotAdmin();
        var tenantId = principal.GetTenantIdOrThrow();

        project.TenantId = tenantId;
        //project.CreatedBy..

        throw new Exception("TODO");
    }

    public async Task<string[]?> GetFieldOptionsAsync(ClaimsPrincipal principal, long testProjectId, TraitType traitType, CancellationToken cancellationToken)
    {
        var integrations = (await GetProjectIntegrationsAsync(principal, testProjectId)).ToArray();
        var dtos = integrations.Select(x => new ExternalSystemDto
        {
            Name = x.Name,
            AccessToken = x.AccessToken,
            BaseUrl = x.BaseUrl,
            ExternalProjectId = x.ExternalProjectId,
            ReadOnly = x.ReadOnly

        }).ToArray();

        foreach (var dataSource in _projectDataSources)
        {
            if (dataSource.SupportedTraits.Contains(traitType))
            {
                var options = await dataSource.GetFieldOptionsAsync(dtos, traitType, cancellationToken);
                if(options.Length > 0)
                {
                    return options;
                }
            }
        }
        return null;
    }

    public async Task<IReadOnlyList<ExternalSystem>> GetProjectIntegrationsAsync(ClaimsPrincipal principal, string slug)
    {
        var tenantId = principal.GetTenantIdOrThrow();

        return await _projectRepository.GetProjectIntegrationsAsync(tenantId, slug);
    }
    public async Task<IReadOnlyList<ExternalSystem>> GetProjectIntegrationsAsync(ClaimsPrincipal principal, long testProjectId)
    {
        var tenantId = principal.GetTenantIdOrThrow();

        var key = "integration:" + testProjectId + ":" + tenantId;
        var result = await _memoryCache.GetOrCreateAsync(key, async (e) =>
        {
            e.AbsoluteExpiration = DateTimeOffset.UtcNow.AddHours(1);
            return await _projectRepository.GetProjectIntegrationsAsync(tenantId, testProjectId);
        });

        return result!;
    }

    public async Task SaveProjectIntegrationsAsync(ClaimsPrincipal principal, string slug, ExternalSystem system)
    {
        principal.ThrowIfNotAdmin();
        var tenantId = principal.GetTenantIdOrThrow();
        system.TenantId = tenantId;

        var project = await _projectRepository.GetBySlugAsync(tenantId, slug);
        if(project is null)
        {
            throw new InvalidOperationException("Project not found");
        }
        principal.ThrowIfEntityTenantIsDifferent(project.TenantId);

        // Remove cached integrations
        var key = "integration:" + project.Id + ":" + tenantId;
        _memoryCache.Remove(key);

        if (system.Id == 0)
        {
            await _projectRepository.AddProjectIntegrationsAsync(tenantId, slug, system);
        }
        else
        {
            await _projectRepository.UpdateProjectIntegrationsAsync(tenantId, slug, system);
        }
    }
}
