using Microsoft.AspNetCore.Components.Authorization;

using OneOf;

using TestBucket.Components.Shared;
using TestBucket.Contracts;
using TestBucket.Data.Errors;
using TestBucket.Data.Projects.Models;
using TestBucket.Data.Testing;

namespace TestBucket.Components.Projects;

internal class ProjectService : TenantBaseService
{
    private readonly IProjectRepository _projectRepository;

    public ProjectService(
        IProjectRepository projectRepository,
        AuthenticationStateProvider authenticationStateProvider) : base(authenticationStateProvider)
    {
        _projectRepository = projectRepository;
    }


    /// <summary>
    /// Generates a short name
    /// </summary>
    /// <param name="dbContext"></param>
    /// <param name="slug"></param>
    /// <param name="tenantId"></param>
    /// <returns></returns>
    public async Task<string> GenerateShortNameAsync(string slug)
    {
        var tenantId = await GetTenantIdAsync();
        return await _projectRepository.GenerateShortNameAsync(slug, tenantId);
    }

    /// <summary>
    /// Generates a slug for a name
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public string GenerateSlug(string name)
    {
        return _projectRepository.GenerateSlug(name);
    }

    public async Task<PagedResult<TestProject>> SearchAsync(SearchQuery query)
    {
        var tenantId = await GetTenantIdAsync();
        return await _projectRepository.SearchAsync(tenantId, query);
    }
    public async Task<OneOf<TestProject, AlreadyExistsError>> CreateAsync(string name)
    {
        var tenantId = await GetTenantIdAsync();
        return await _projectRepository.CreateAsync(tenantId, name);   
    }
}
