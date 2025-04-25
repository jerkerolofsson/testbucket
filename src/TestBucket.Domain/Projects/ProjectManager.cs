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
using TestBucket.Domain.Projects.Models;
using TestBucket.Domain.Identity.Permissions;
using TestBucket.Domain.Projects.Mapping;
using Mediator;
using TestBucket.Domain.Projects.Events;

namespace TestBucket.Domain.Projects;
internal class ProjectManager : IProjectManager
{
    private readonly IProjectRepository _projectRepository;
    private readonly IMemoryCache _memoryCache;
    private readonly List<IProjectDataSource> _projectDataSources;
    private readonly IMediator _mediator;

    public ProjectManager(IProjectRepository projectRepository,
        IMemoryCache memoryCache,
        IEnumerable<IProjectDataSource> projectDataSources,
        IMediator mediator)
    {
        _projectDataSources = projectDataSources.ToList();
        _projectRepository = projectRepository;
        _memoryCache = memoryCache;
        _mediator = mediator;
    }

    public async Task UpdateProjectAsync(ClaimsPrincipal principal, TestProject project)
    {
        project.TenantId = principal.GetTenantIdOrThrow();
        principal.ThrowIfNoPermission(PermissionEntityType.Project, PermissionLevel.Write);

        //project.Modified = DateTimeOffset.UtcNow;
        //project.ModifiedBy = principal.Identity?.Name ?? throw new Exception("No identity in current principal");

        await _projectRepository.UpdateProjectAsync(project);
        await _mediator.Publish(new ProjectUpdated(project));
    }

    public async Task<OneOf<TestProject, AlreadyExistsError>> AddAsync(ClaimsPrincipal principal, TestProject project)
    {
        project.TenantId = principal.GetTenantIdOrThrow();
        principal.ThrowIfNoPermission(PermissionEntityType.Project, PermissionLevel.Write);

        var result = await _projectRepository.AddAsync(project);

        if(result.IsT0)
        {
            await _mediator.Publish(new ProjectCreated(project));
        }

        return result;

        //project.Created = DateTimeOffset.UtcNow;
        //project.Modified = DateTimeOffset.UtcNow;
        //project.CreatedBy = principal.Identity?.Name ?? throw new Exception("No identity in current principal");
        //project.ModifiedBy = principal.Identity?.Name ?? throw new Exception("No identity in current principal");
    }

    public async Task<string[]?> GetFieldOptionsAsync(ClaimsPrincipal principal, long testProjectId, TraitType traitType, CancellationToken cancellationToken)
    {
        var integrations = (await GetProjectIntegrationsAsync(principal, testProjectId)).ToArray();
        var dtos = integrations.Select(x => x.ToDto()).ToArray();

        foreach (var dataSource in _projectDataSources)
        {
            if (dataSource.SupportedTraits.Contains(traitType))
            {
                var dto = dtos.Where(x => x.Name == dataSource.SystemName && x.Enabled).FirstOrDefault();
                if (dto is not null)
                {
                    try
                    {
                        // Verify that the capability is enabled for the data source
                        if (traitType == TraitType.Release && 
                            (dto.EnabledCapabilities&ExternalSystemCapability.GetReleases) != ExternalSystemCapability.GetReleases)
                        {
                            continue;
                        }
                        if (traitType == TraitType.Milestone && (dto.EnabledCapabilities & ExternalSystemCapability.GetMilestones) != ExternalSystemCapability.GetMilestones)
                        {
                            continue;
                        }

                        var options = await dataSource.GetFieldOptionsAsync(dto, traitType, cancellationToken);
                        if (options.Length > 0)
                        {
                            return options;
                        }
                    }
                    catch { }
                }
            }
        }
        return null;
    }

    public async Task<PagedResult<TestProject>> BrowseTestProjectsAsync(ClaimsPrincipal principal, int offset, int count)
    {
        principal.ThrowIfNoPermission(PermissionEntityType.Project, PermissionLevel.Read);
        var tenantId = principal.GetTenantIdOrThrow();
        return await _projectRepository.SearchAsync(tenantId, new SearchQuery() { Offset = offset, Count = count });
    }


    public async Task<TestProject?> GetTestProjectByIdAsync(ClaimsPrincipal principal, long projectId)
    {
        principal.ThrowIfNoPermission(PermissionEntityType.Project, PermissionLevel.Read);
        var tenantId = principal.GetTenantIdOrThrow();
        return await _projectRepository.GetProjectByIdAsync(tenantId, projectId);
    }

    #region Project Integrations

    /// <summary>
    /// Deletes a project integration / external system
    /// </summary>
    /// <param name="principal"></param>
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task DeleteProjectIntegrationAsync(ClaimsPrincipal principal, long id)
    {
        principal.ThrowIfNoPermission(PermissionEntityType.Project, PermissionLevel.Write);
        var tenantId = principal.GetTenantIdOrThrow();
        await _projectRepository.DeleteProjectIntegrationAsync(tenantId, id);
    }

    public async Task<IReadOnlyList<ExternalSystem>> GetProjectIntegrationsAsync(ClaimsPrincipal principal, string slug)
    {
        principal.ThrowIfNoPermission(PermissionEntityType.Project, PermissionLevel.Read);
        var tenantId = principal.GetTenantIdOrThrow();
        return await _projectRepository.GetProjectIntegrationsAsync(tenantId, slug);
    }
    public async Task<IReadOnlyList<ExternalSystem>> GetProjectIntegrationsAsync(ClaimsPrincipal principal, long testProjectId)
    {
        principal.ThrowIfNoPermission(PermissionEntityType.Project, PermissionLevel.Read);
        var tenantId = principal.GetTenantIdOrThrow();

        var key = "integration:" + testProjectId + ":" + tenantId;
        var result = await _memoryCache.GetOrCreateAsync(key, async (e) =>
        {
            e.AbsoluteExpiration = DateTimeOffset.UtcNow.AddHours(1);
            return await _projectRepository.GetProjectIntegrationsAsync(tenantId, testProjectId);
        });

        return result!;
    }

    /// <summary>
    /// Upsert: Saves changes made to an integration or adds a new one
    /// </summary>
    /// <param name="principal"></param>
    /// <param name="slug"></param>
    /// <param name="system"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException">Invalid project</exception>
    /// <exception cref="ArgumentException"></exception>
    public async Task SaveProjectIntegrationAsync(ClaimsPrincipal principal, string slug, ExternalSystem system)
    {
        principal.ThrowIfNoPermission(PermissionEntityType.Project, PermissionLevel.Write);
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

        system.Modified = DateTimeOffset.UtcNow;
        system.ModifiedBy = principal.Identity?.Name ?? throw new ArgumentException("provided principal has no identity name");
        if (system.Id == 0)
        {
            system.Created = DateTimeOffset.UtcNow;
            system.CreatedBy = principal.Identity?.Name;
            await _projectRepository.AddProjectIntegrationsAsync(tenantId, slug, system);
        }
        else
        {
            await _projectRepository.UpdateProjectIntegrationAsync(tenantId, slug, system);
        }
    }

    public async Task<TestProject?> GetTestProjectBySlugAsync(ClaimsPrincipal principal, string slug)
    {
        principal.ThrowIfNoPermission(PermissionEntityType.Project, PermissionLevel.Read);
        var tenantId = principal.GetTenantIdOrThrow();

        return await _projectRepository.GetBySlugAsync(tenantId, slug);
    }

    public async Task DeleteAsync(ClaimsPrincipal principal, TestProject project)
    {
        principal.ThrowIfNoPermission(PermissionEntityType.Project, PermissionLevel.Delete);
        principal.ThrowIfEntityTenantIsDifferent(project.TenantId);
        await _projectRepository.DeleteProjectAsync(project);
    }

    #endregion Project Integrations
}
