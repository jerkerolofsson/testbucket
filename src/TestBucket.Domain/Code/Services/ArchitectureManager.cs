using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Domain.Code.Models;
using TestBucket.Domain.Code.Specifications;
using TestBucket.Domain.Code.Yaml;
using TestBucket.Domain.Shared.Specifications;

namespace TestBucket.Domain.Code.Services;

internal class ArchitectureManager : IArchitectureManager
{
    private readonly IArchitectureRepository _repository;

    public ArchitectureManager(IArchitectureRepository repository)
    {
        _repository = repository;
    }

    public async Task AddSystemAsync(ClaimsPrincipal principal, ProductSystem system)
    {
        system.TenantId = principal.GetTenantIdOrThrow();
        await _repository.AddSystemAsync(system);
    }

    public async Task<IReadOnlyList<ProductSystem>> GetSystemsAsync(ClaimsPrincipal principal, long projectId)
    {
        var tenantId = principal.GetTenantIdOrThrow();
        FilterSpecification<ProductSystem>[] filters = [new FilterByProject<ProductSystem>(projectId), new FilterByTenant<ProductSystem>(tenantId)];
        
        PagedResult<ProductSystem> result = await _repository.SearchSystemsAsync(filters, 0, 200);
        return result.Items.ToList();
    }

    public async Task<ProductSystem?> GetSystemByNameAsync(ClaimsPrincipal principal, long projectId, string name)
    {
        var tenantId = principal.GetTenantIdOrThrow();
        FilterSpecification<ProductSystem>[] filters = [
            new FilterSystemByName(name),
            new FilterByProject<ProductSystem>(projectId), new FilterByTenant<ProductSystem>(tenantId)];

        PagedResult<ProductSystem> result = await _repository.SearchSystemsAsync(filters, 0, 1);
        if (result.Items.Length > 0)
        {
            return result.Items[0];
        }
        return null;
    }

    public async Task<ProjectArchitectureModel> GetProductArchitectureAsync(ClaimsPrincipal principal, TestProject project)
    {
        var model = new ProjectArchitectureModel();

        foreach (var system in await GetSystemsAsync(principal, project.Id))
        {
            model.Systems[system.Name] = new Yaml.Models.ArchitecturalComponent { Display = system.Display, Paths = system.GlobPatterns };
        }
        return model;
    }

    public async Task ImportProductArchitectureAsync(ClaimsPrincipal principal, TestProject project, ProjectArchitectureModel model)
    {
        if (model.Systems is not null)
        {
            foreach (var system in model.Systems)
            {
                var existingSystem = await GetSystemByNameAsync(principal, project.Id, system.Key);
                if(existingSystem is null)
                {
                    existingSystem = new ProductSystem
                    {
                        TenantId = principal.GetTenantIdOrThrow(),
                        Name = system.Key,
                        GlobPatterns = system.Value.Paths ?? [],
                        TestProjectId = project.Id,
                        TeamId = project.TeamId,
                        Display = system.Value.Display,
                        Created = DateTimeOffset.UtcNow,
                        Modified = DateTimeOffset.UtcNow,
                        CreatedBy = principal.Identity?.Name ?? throw new InvalidOperationException("Invalid identity"),
                        ModifiedBy = principal.Identity?.Name ?? throw new InvalidOperationException("Invalid identity"),
                    };
                    await _repository.AddSystemAsync(existingSystem);
                }
                else
                {
                    existingSystem.GlobPatterns = system.Value.Paths ?? existingSystem.GlobPatterns;
                    existingSystem.Display = system.Value.Display ?? existingSystem.Display;
                    await _repository.UpdateSystemAsync(existingSystem);
                }
            }
        }
    }
}
