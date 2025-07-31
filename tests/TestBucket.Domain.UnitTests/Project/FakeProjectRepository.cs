using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OneOf;

using TestBucket.Contracts;
using TestBucket.Domain.Errors;
using TestBucket.Domain.Projects;
using TestBucket.Domain.Projects.Models;

namespace TestBucket.Domain.UnitTests.Project;
internal class FakeProjectRepository : IProjectRepository
{
    private readonly List<TestProject> _projects = new();
    private readonly List<ExternalSystem> _externalSystems = new();

    public Task<OneOf<TestProject, AlreadyExistsError>> AddAsync(TestProject project)
    {
        if (_projects.Any(p => p.TenantId == project.TenantId && p.Name == project.Name))
        {
            return Task.FromResult<OneOf<TestProject, AlreadyExistsError>>(new AlreadyExistsError());
        }

        project.Slug = GenerateSlug(project.Name);
        project.ShortName = GenerateShortName(project.Slug);
        project.Id = _projects.Count > 0 ? _projects.Max(p => p.Id) + 1 : 1;
        _projects.Add(project);

        return Task.FromResult<OneOf<TestProject, AlreadyExistsError>>(project);
    }

    public Task AddProjectIntegrationsAsync(string tenantId, string slug, ExternalSystem system)
    {
        var project = _projects.FirstOrDefault(p => p.TenantId == tenantId && p.Slug == slug);
        if (project == null)
        {
            throw new InvalidOperationException("Project not found: " + slug);
        }

        system.TestProjectId = project.Id;
        _externalSystems.Add(system);
        return Task.CompletedTask;
    }

    public Task DeleteProjectAsync(TestProject project)
    {
        _projects.Remove(project);
        _externalSystems.RemoveAll(es => es.TestProjectId == project.Id);
        return Task.CompletedTask;
    }

    public Task DeleteProjectIntegrationAsync(string tenantId, long id)
    {
        _externalSystems.RemoveAll(es => es.TenantId == tenantId && es.Id == id);
        return Task.CompletedTask;
    }

    public Task<string> GenerateShortNameAsync(string slug, string tenantId)
    {
        return Task.FromResult(GenerateShortName(slug));
    }

    public string GenerateSlug(string name)
    {
        return new Slugify.SlugHelper().GenerateSlug(name);
    }

    public Task<TestProject?> GetBySlugAsync(string tenantId, string slug)
    {
        var project = _projects.FirstOrDefault(p => p.TenantId == tenantId && p.Slug == slug);
        return Task.FromResult(project);
    }

    public Task<TestProject?> GetProjectByIdAsync(string tenantId, long projectId)
    {
        var project = _projects.FirstOrDefault(p => p.TenantId == tenantId && p.Id == projectId);
        return Task.FromResult(project);
    }

    public Task<IReadOnlyList<ExternalSystem>> GetProjectIntegrationsAsync(string tenantId, string projectSlug)
    {
        var project = _projects.FirstOrDefault(p => p.TenantId == tenantId && p.Slug == projectSlug);
        if (project == null)
        {
            throw new InvalidOperationException("Project not found: " + projectSlug);
        }

        var integrations = _externalSystems.Where(es => es.TestProjectId == project.Id).ToList();
        return Task.FromResult<IReadOnlyList<ExternalSystem>>(integrations);
    }

    public Task<IReadOnlyList<ExternalSystem>> GetProjectIntegrationsAsync(string tenantId, long projectId)
    {
        var integrations = _externalSystems.Where(es => es.TenantId == tenantId && es.TestProjectId == projectId).ToList();
        return Task.FromResult<IReadOnlyList<ExternalSystem>>(integrations);
    }

    public Task<bool> NameExistsAsync(string tenantId, string name)
    {
        var exists = _projects.Any(p => p.TenantId == tenantId && p.Name == name);
        return Task.FromResult(exists);
    }

    public Task<PagedResult<TestProject>> SearchAsync(string tenantId, SearchQuery query)
    {
        var filteredProjects = _projects.Where(p => p.TenantId == tenantId);

        if (query.TeamId.HasValue)
        {
            filteredProjects = filteredProjects.Where(p => p.TeamId == query.TeamId);
        }

        if (!string.IsNullOrEmpty(query.Text))
        {
            filteredProjects = filteredProjects.Where(p => p.Name.Contains(query.Text, StringComparison.OrdinalIgnoreCase));
        }

        var totalCount = filteredProjects.Count();
        var items = filteredProjects.Skip(query.Offset).Take(query.Count).ToArray();

        return Task.FromResult(new PagedResult<TestProject>
        {
            TotalCount = totalCount,
            Items = items
        });
    }

    public Task<bool> SlugExistsAsync(string tenantId, string slug)
    {
        var exists = _projects.Any(p => p.TenantId == tenantId && p.Slug == slug);
        return Task.FromResult(exists);
    }

    public Task UpdateProjectAsync(TestProject project)
    {
        var existingProject = _projects.FirstOrDefault(p => p.Id == project.Id);
        if (existingProject != null)
        {
            _projects.Remove(existingProject);
            _projects.Add(project);
        }
        return Task.CompletedTask;
    }

    public Task UpdateProjectIntegrationAsync(string tenantId, string slug, ExternalSystem system)
    {
        var existingIntegration = _externalSystems.FirstOrDefault(es => es.TenantId == tenantId && es.Id == system.Id);
        if (existingIntegration != null)
        {
            _externalSystems.Remove(existingIntegration);
            _externalSystems.Add(system);
        }
        return Task.CompletedTask;
    }

    private string GenerateShortName(string slug)
    {
        return new string(slug.Split('-').Select(word => word[0]).ToArray()).ToUpper();
    }
}
