using Mediator;

using Microsoft.Extensions.Logging;

using TestBucket.Domain.AI.Embeddings;
using TestBucket.Domain.Code.Models;
using TestBucket.Domain.Code.Specifications;
using TestBucket.Domain.Issues.Models;
using TestBucket.Domain.Shared.Specifications;

namespace TestBucket.Domain.Code.Services;

internal class ArchitectureManager : IArchitectureManager
{
    private readonly IArchitectureRepository _repository;
    private readonly ILogger<ArchitectureManager> _logger;
    private readonly IMediator _mediator;

    public ArchitectureManager(IArchitectureRepository repository, ILogger<ArchitectureManager> logger, IMediator mediator)
    {
        _repository = repository;
        _logger = logger;
        _mediator = mediator;
    }

    #region Systems
    public async Task UpdateSystemAsync(ClaimsPrincipal principal, ProductSystem system)
    {
        principal.ThrowIfEntityTenantIsDifferent(system);
        principal.ThrowIfNoPermission(PermissionEntityType.Architecture, PermissionLevel.Write);

        var existingItem = await _repository.GetSystemAsync(system.Id);
        if (existingItem is null)
        {
            throw new ArgumentException("Item does not exist");
        }
        principal.ThrowIfEntityTenantIsDifferent(existingItem);
        if (existingItem.Name != system.Name || existingItem.Description != system.Description)
        {
            await GenerateEmbeddingAsync(system);
        }

        await _repository.UpdateSystemAsync(system);
    }

    public async Task DeleteSystemAsync(ClaimsPrincipal principal, ProductSystem system)
    {
        principal.ThrowIfEntityTenantIsDifferent(system);
        principal.ThrowIfNoPermission(PermissionEntityType.Architecture, PermissionLevel.Delete);
        await _repository.DeleteSystemAsync(system.Id);
    }

    /// <summary>
    /// Adds a system
    /// </summary>
    /// <param name="principal"></param>
    /// <param name="system"></param>
    /// <returns></returns>
    public async Task AddSystemAsync(ClaimsPrincipal principal, ProductSystem system)
    {
        principal.ThrowIfNoPermission(PermissionEntityType.Architecture, PermissionLevel.Write);
        system.TenantId = principal.GetTenantIdOrThrow();

        await GenerateEmbeddingAsync(system);

        await _repository.AddSystemAsync(system);
    }

    /// <summary>
    /// Returns all architectural systems for a project
    /// </summary>
    /// <param name="principal"></param>
    /// <param name="projectId"></param>
    /// <returns></returns>
    public async Task<IReadOnlyList<ProductSystem>> GetSystemsAsync(ClaimsPrincipal principal, long projectId)
    {
        principal.ThrowIfNoPermission(PermissionEntityType.Architecture, PermissionLevel.Read);
        var tenantId = principal.GetTenantIdOrThrow();
        FilterSpecification<ProductSystem>[] filters = [new FilterByProject<ProductSystem>(projectId), new FilterByTenant<ProductSystem>(tenantId)];

        PagedResult<ProductSystem> result = await _repository.SearchSystemsAsync(filters, 0, 10_000);
        return result.Items.ToList();
    }

    /// <summary>
    /// Returns all architectural components for a project
    /// </summary>
    /// <param name="principal"></param>
    /// <param name="projectId"></param>
    /// <returns></returns>
    public async Task<IReadOnlyList<Component>> GetComponentsAsync(ClaimsPrincipal principal, long projectId)
    {
        principal.ThrowIfNoPermission(PermissionEntityType.Architecture, PermissionLevel.Read);
        var tenantId = principal.GetTenantIdOrThrow();
        FilterSpecification<Component>[] filters = [new FilterByProject<Component>(projectId), new FilterByTenant<Component>(tenantId)];

        PagedResult<Component> result = await _repository.SearchComponentsAsync(filters, 0, 10_000);
        return result.Items.ToList();
    }


    /// <summary>
    /// Returns a system by name
    /// </summary>
    /// <param name="principal"></param>
    /// <param name="projectId"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    public async Task<ProductSystem?> GetSystemByNameAsync(ClaimsPrincipal principal, long projectId, string name)
    {
        principal.ThrowIfNoPermission(PermissionEntityType.Architecture, PermissionLevel.Read);
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
    #endregion Systems

    #region Layers

    public async Task UpdateLayerAsync(ClaimsPrincipal principal, ArchitecturalLayer layer)
    {
        principal.ThrowIfEntityTenantIsDifferent(layer);
        principal.ThrowIfNoPermission(PermissionEntityType.Architecture, PermissionLevel.Write);
        var existingItem = await _repository.GetLayerAsync(layer.Id);
        if (existingItem is null)
        {
            throw new ArgumentException("Item does not exist");
        }
        principal.ThrowIfEntityTenantIsDifferent(existingItem);
        if (existingItem.Name != layer.Name || existingItem.Description != layer.Description)
        {
            await GenerateEmbeddingAsync(layer);
        }
        await _repository.UpdateLayerAsync(layer);
    }

    public async Task DeleteLayerAsync(ClaimsPrincipal principal, ArchitecturalLayer layer)
    {
        principal.ThrowIfEntityTenantIsDifferent(layer);
        principal.ThrowIfNoPermission(PermissionEntityType.Architecture, PermissionLevel.Delete);
        await _repository.DeleteLayerAsync(layer.Id);
    }

    /// <summary>
    /// Adds a layer
    /// </summary>
    /// <param name="principal"></param>
    /// <param name="component"></param>
    /// <returns></returns>
    public async Task AddLayerAsync(ClaimsPrincipal principal, ArchitecturalLayer component)
    {
        principal.ThrowIfNoPermission(PermissionEntityType.Architecture, PermissionLevel.Write);
        component.TenantId = principal.GetTenantIdOrThrow();
        await GenerateEmbeddingAsync(component);
        await _repository.AddLayerAsync(component);
    }

    /// <summary>
    /// Returns all architectural layers for a project
    /// </summary>
    /// <param name="principal"></param>
    /// <param name="projectId"></param>
    /// <returns></returns>
    public async Task<IReadOnlyList<ArchitecturalLayer>> GetLayersAsync(ClaimsPrincipal principal, long projectId)
    {
        principal.ThrowIfNoPermission(PermissionEntityType.Architecture, PermissionLevel.Read);
        var tenantId = principal.GetTenantIdOrThrow();
        FilterSpecification<ArchitecturalLayer>[] filters = [new FilterByProject<ArchitecturalLayer>(projectId), new FilterByTenant<ArchitecturalLayer>(tenantId)];

        PagedResult<ArchitecturalLayer> result = await _repository.SearchLayersAsync(filters, 0, 200);
        return result.Items.ToList();
    }

    /// <summary>
    /// Returns a architectural layer by name
    /// </summary>
    /// <param name="principal"></param>
    /// <param name="projectId"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    public async Task<ArchitecturalLayer?> GetLayerByNameAsync(ClaimsPrincipal principal, long projectId, string name)
    {
        principal.ThrowIfNoPermission(PermissionEntityType.Architecture, PermissionLevel.Read);
        var tenantId = principal.GetTenantIdOrThrow();
        FilterSpecification<ArchitecturalLayer>[] filters = [
            new FilterLayerByName(name),
            new FilterByProject<ArchitecturalLayer>(projectId), new FilterByTenant<ArchitecturalLayer>(tenantId)];

        PagedResult<ArchitecturalLayer> result = await _repository.SearchLayersAsync(filters, 0, 1);
        if (result.Items.Length > 0)
        {
            return result.Items[0];
        }
        return null;
    }
    #endregion Layers

    #region Components

    public async Task DeleteComponentAsync(ClaimsPrincipal principal, Component component)
    {
        principal.ThrowIfEntityTenantIsDifferent(component);
        principal.ThrowIfNoPermission(PermissionEntityType.Architecture, PermissionLevel.Delete);
        await _repository.DeleteComponentAsync(component.Id);

    }
    /// <summary>
    /// Adds a component
    /// </summary>
    /// <param name="principal"></param>
    /// <param name="component"></param>
    /// <returns></returns>
    public async Task AddComponentAsync(ClaimsPrincipal principal, Component component)
    {
        principal.ThrowIfNoPermission(PermissionEntityType.Architecture, PermissionLevel.Write);
        component.TenantId = principal.GetTenantIdOrThrow();
        await GenerateEmbeddingAsync(component);
        await _repository.AddComponentAsync(component);
    }
    public async Task UpdateComponentAsync(ClaimsPrincipal principal, Component component)
    {
        principal.ThrowIfNoPermission(PermissionEntityType.Architecture, PermissionLevel.Write);
        component.TenantId = principal.GetTenantIdOrThrow();
        var existingItem = await _repository.GetComponentAsync(component.Id);
        if (existingItem is null)
        {
            throw new ArgumentException("Item does not exist");
        }
        principal.ThrowIfEntityTenantIsDifferent(existingItem);
        if (existingItem.Name != component.Name || existingItem.Description != component.Description)
        {
            await GenerateEmbeddingAsync(component);
        }

        await _repository.UpdateComponentAsync(component);
    }

    /// <summary>
    /// Returns all architectural component for a project
    /// </summary>
    /// <param name="principal"></param>
    /// <param name="projectId"></param>
    /// <returns></returns>
    public async Task<IReadOnlyList<Component>> GetComponentAsync(ClaimsPrincipal principal, long projectId)
    {
        principal.ThrowIfNoPermission(PermissionEntityType.Architecture, PermissionLevel.Read);
        var tenantId = principal.GetTenantIdOrThrow();
        FilterSpecification<Component>[] filters = [new FilterByProject<Component>(projectId), new FilterByTenant<Component>(tenantId)];

        PagedResult<Component> result = await _repository.SearchComponentsAsync(filters, 0, 200);
        return result.Items.ToList();
    }


    /// <summary>
    /// Searches for features
    /// </summary>
    /// <param name="principal"></param>
    /// <param name="projectId"></param>
    /// <returns></returns>
    public async Task<IReadOnlyList<Component>> SearchComponentsAsync(ClaimsPrincipal principal, long projectId, string text, int offset, int count)
    {
        principal.ThrowIfNoPermission(PermissionEntityType.Architecture, PermissionLevel.Read);
        var tenantId = principal.GetTenantIdOrThrow();
        FilterSpecification<Component>[] filters = [
            new SearchComponentWithText(text ?? ""),
            new FilterByProject<Component>(projectId),
            new FilterByTenant<Component>(tenantId)
        ];

        PagedResult<Component> result = await _repository.SearchComponentsAsync(filters, offset, count);
        return result.Items.ToList();
    }

    /// <summary>
    /// Returns a system by name
    /// </summary>
    /// <param name="principal"></param>
    /// <param name="projectId"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    public async Task<Component?> GetComponentByNameAsync(ClaimsPrincipal principal, long projectId, string name)
    {
        principal.ThrowIfNoPermission(PermissionEntityType.Architecture, PermissionLevel.Read);
        var tenantId = principal.GetTenantIdOrThrow();
        FilterSpecification<Component>[] filters = [
            new FilterComponentByName(name),
            new FilterByProject<Component>(projectId), new FilterByTenant<Component>(tenantId)];

        PagedResult<Component> result = await _repository.SearchComponentsAsync(filters, 0, 1);
        if (result.Items.Length > 0)
        {
            return result.Items[0];
        }
        return null;
    }
    #endregion Components

    #region Features


    /// <summary>
    /// Adds a feature
    /// </summary>
    /// <param name="principal"></param>
    /// <param name="feature"></param>
    /// <returns></returns>
    public async Task AddFeatureAsync(ClaimsPrincipal principal, Feature feature)
    {
        principal.ThrowIfNoPermission(PermissionEntityType.Architecture, PermissionLevel.Write);
        feature.TenantId = principal.GetTenantIdOrThrow();
        await GenerateEmbeddingAsync(feature);
        await _repository.AddFeatureAsync(feature);
    }

    /// <summary>
    /// Searches for features
    /// </summary>
    /// <param name="principal"></param>
    /// <param name="projectId"></param>
    /// <returns></returns>
    public async Task<IReadOnlyList<Feature>> SearchFeaturesAsync(ClaimsPrincipal principal, long projectId, string text, int offset, int count)
    {
        principal.ThrowIfNoPermission(PermissionEntityType.Architecture, PermissionLevel.Read);
        var tenantId = principal.GetTenantIdOrThrow();
        FilterSpecification<Feature>[] filters = [
            new SearchFeatureWithText(text),
            new FilterByProject<Feature>(projectId), 
            new FilterByTenant<Feature>(tenantId)
        ];

        PagedResult<Feature> result = await _repository.SearchFeaturesAsync(filters, offset, count);
        return result.Items.ToList();
    }

    /// <summary>
    /// Returns all defined feature for a project
    /// </summary>
    /// <param name="principal"></param>
    /// <param name="projectId"></param>
    /// <returns></returns>
    public async Task<IReadOnlyList<Feature>> GetFeaturesAsync(ClaimsPrincipal principal, long projectId)
    {
        principal.ThrowIfNoPermission(PermissionEntityType.Architecture, PermissionLevel.Read);
        var tenantId = principal.GetTenantIdOrThrow();
        FilterSpecification<Feature>[] filters = [new FilterByProject<Feature>(projectId), new FilterByTenant<Feature>(tenantId)];

        PagedResult<Feature> result = await _repository.SearchFeaturesAsync(filters, 0, 200);
        return result.Items.ToList();
    }

    /// <summary>
    /// Returns a feature by name
    /// </summary>
    /// <param name="principal"></param>
    /// <param name="projectId"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    public async Task<Feature?> GetFeatureByNameAsync(ClaimsPrincipal principal, long projectId, string name)
    {
        principal.ThrowIfNoPermission(PermissionEntityType.Architecture, PermissionLevel.Read);
        var tenantId = principal.GetTenantIdOrThrow();
        FilterSpecification<Feature>[] filters = [
            new FilterFeatureByName(name),
            new FilterByProject<Feature>(projectId), new FilterByTenant<Feature>(tenantId)];

        PagedResult<Feature> result = await _repository.SearchFeaturesAsync(filters, 0, 1);
        if (result.Items.Length > 0)
        {
            return result.Items[0];
        }
        return null;
    }
    #endregion Features
    private async Task GenerateEmbeddingAsync(AritecturalComponentProjectEntity item)
    {
        if (item.TestProjectId is null)
        {
            return;
        }

        try
        {
            var text = $"{item.Name} {item.Description}";
            var response = await _mediator.Send(new GenerateEmbeddingRequest(item.TestProjectId.Value, text));
            if (response.EmbeddingVector is not null)
            {
                item.Embedding = new Pgvector.Vector(response.EmbeddingVector.Value);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating embedding for component {AritecturalComponentProjectEntityName}", item.Name);
        }
    }

    public async Task<ProjectArchitectureModel> GetProductArchitectureAsync(ClaimsPrincipal principal, TestProject project)
    {
        principal.ThrowIfNoPermission(PermissionEntityType.Architecture, PermissionLevel.Read);
        var model = new ProjectArchitectureModel();

        foreach (var system in await GetSystemsAsync(principal, project.Id))
        {
            model.Systems[system.Name] = CreateComponent(system);
        }

        foreach (var feature in await GetFeaturesAsync(principal, project.Id))
        {
            model.Features[feature.Name] = CreateComponent(feature);
        }
        foreach (var layer in await GetLayersAsync(principal, project.Id))
        {
            model.Layers[layer.Name] = CreateComponent(layer);
        }
        foreach (var component in await GetComponentsAsync(principal, project.Id))
        {
            model.Components[component.Name] = CreateComponent(component);
        }
        return model;
    }

    private static ArchitecturalComponent CreateComponent(AritecturalComponentProjectEntity system)
    {
        return new ArchitecturalComponent { Display = system.Display, Paths = system.GlobPatterns, TestLead = system.TestLead, DevLead = system.DevLead, Description = system.Description };
    }

    /// <summary>
    /// Imports a model of the project architecture, adding systems, components, features
    /// </summary>
    /// <param name="principal"></param>
    /// <param name="project"></param>
    /// <param name="model"></param>
    /// <returns></returns>

    public async Task ImportProductArchitectureAsync(ClaimsPrincipal principal, TestProject project, ProjectArchitectureModel model)
    {
        principal.ThrowIfNoPermission(PermissionEntityType.Architecture, PermissionLevel.Write);
        if (model.Systems is not null)
        {
            await ImportSystemsAsync(principal, project, model);
        }
        if (model.Components is not null)
        {
            await ImportComponentsAsync(principal, project, model);
        }
        if (model.Features is not null)
        {
            await ImportFeaturesAsync(principal, project, model);
        }
        if (model.Layers is not null)
        {
            await ImportLayersAsync(principal, project, model);
        }
    }

    private async Task ImportFeaturesAsync(ClaimsPrincipal principal, TestProject project, ProjectArchitectureModel model)
    {
        principal.ThrowIfNoPermission(PermissionEntityType.Architecture, PermissionLevel.Write);
        foreach (var feature in model.Features)
        {
            var existingFeature = await GetFeatureByNameAsync(principal, project.Id, feature.Key);
            if (existingFeature is null)
            {
                existingFeature = new Feature
                {
                    TenantId = principal.GetTenantIdOrThrow(),
                    Name = feature.Key,
                    GlobPatterns = feature.Value.Paths ?? [],
                    TestProjectId = project.Id,
                    TeamId = project.TeamId,
                    Display = feature.Value.Display,
                    Created = DateTimeOffset.UtcNow,
                    Modified = DateTimeOffset.UtcNow,
                    Description = feature.Value.Description,
                    TestLead = feature.Value.TestLead,
                    DevLead = feature.Value.DevLead,

                    CreatedBy = principal.Identity?.Name ?? throw new InvalidOperationException("Invalid identity"),
                    ModifiedBy = principal.Identity?.Name ?? throw new InvalidOperationException("Invalid identity"),
                };
                await GenerateEmbeddingAsync(existingFeature);
                await _repository.AddFeatureAsync(existingFeature);
            }
            else
            {
                await UpdateComponentAsync(principal, feature, existingFeature);
                existingFeature.TestProjectId = project.Id;
                await _repository.UpdateFeatureAsync(existingFeature);
            }
        }
    }

    public async Task UpdateFeatureAsync(ClaimsPrincipal principal, Feature feature)
    {
        principal.ThrowIfEntityTenantIsDifferent(feature);
        principal.ThrowIfNoPermission(PermissionEntityType.Architecture, PermissionLevel.Write);

        var existingFeature = await _repository.GetFeatureAsync(feature.Id);
        if(existingFeature is null)
        {
            throw new ArgumentException("Item does not exist");
        }
        if(existingFeature.Name != feature.Name || existingFeature.Description != feature.Description)
        {
            await GenerateEmbeddingAsync(feature);
        }

        feature.Modified = DateTimeOffset.UtcNow;
        feature.ModifiedBy = principal.Identity?.Name ?? throw new InvalidOperationException("Invalid identity");
        await _repository.UpdateFeatureAsync(feature);
    }

    public async Task DeleteFeatureAsync(ClaimsPrincipal principal, Feature feature)
    {
        principal.ThrowIfEntityTenantIsDifferent(feature);
        principal.ThrowIfNoPermission(PermissionEntityType.Architecture, PermissionLevel.Delete);
        await _repository.DeleteFeatureAsync(feature.Id);
    }

    private async Task ImportLayersAsync(ClaimsPrincipal principal, TestProject project, ProjectArchitectureModel model)
    {
        principal.ThrowIfNoPermission(PermissionEntityType.Architecture, PermissionLevel.Write);
        foreach (var layer in model.Layers)
        {
            var existingFeature = await GetLayerByNameAsync(principal, project.Id, layer.Key);
            if (existingFeature is null)
            {
                existingFeature = new ArchitecturalLayer
                {
                    TenantId = principal.GetTenantIdOrThrow(),
                    Name = layer.Key,
                    GlobPatterns = layer.Value.Paths ?? [],
                    TestProjectId = project.Id,
                    TeamId = project.TeamId,
                    Display = layer.Value.Display,
                    Created = DateTimeOffset.UtcNow,
                    Modified = DateTimeOffset.UtcNow,
                    Description = layer.Value.Description,
                    TestLead = layer.Value.TestLead,
                    DevLead = layer.Value.DevLead,

                    CreatedBy = principal.Identity?.Name ?? throw new InvalidOperationException("Invalid identity"),
                    ModifiedBy = principal.Identity?.Name ?? throw new InvalidOperationException("Invalid identity"),
                };
                await GenerateEmbeddingAsync(existingFeature);
                await _repository.AddLayerAsync(existingFeature);
            }
            else
            {
                await UpdateComponentAsync(principal, layer, existingFeature);
                existingFeature.TestProjectId = project.Id;
                await _repository.UpdateLayerAsync(existingFeature);
            }
        }
    }

    private async Task ImportComponentsAsync(ClaimsPrincipal principal, TestProject project, ProjectArchitectureModel model)
    {
        principal.ThrowIfNoPermission(PermissionEntityType.Architecture, PermissionLevel.Write);
        foreach (var component in model.Components)
        {
            var existingComponent = await GetComponentByNameAsync(principal, project.Id, component.Key);
            if (existingComponent is null)
            {
                existingComponent = new Component
                {
                    TenantId = principal.GetTenantIdOrThrow(),
                    Name = component.Key,
                    GlobPatterns = component.Value.Paths ?? [],
                    TestProjectId = project.Id,
                    TeamId = project.TeamId,
                    Display = component.Value.Display,
                    Created = DateTimeOffset.UtcNow,
                    Modified = DateTimeOffset.UtcNow,
                    Description = component.Value.Description,
                    TestLead = component.Value.TestLead,
                    DevLead = component.Value.DevLead,

                    CreatedBy = principal.Identity?.Name ?? throw new InvalidOperationException("Invalid identity"),
                    ModifiedBy = principal.Identity?.Name ?? throw new InvalidOperationException("Invalid identity"),
                };
                await GenerateEmbeddingAsync(existingComponent);
                await _repository.AddComponentAsync(existingComponent);
            }
            else
            {
                await UpdateComponentAsync(principal, component, existingComponent);
                existingComponent.TestProjectId = project.Id;
                await _repository.UpdateComponentAsync(existingComponent);
            }
        }
    }

    private async Task ImportSystemsAsync(ClaimsPrincipal principal, TestProject project, ProjectArchitectureModel model)
    {
        principal.ThrowIfNoPermission(PermissionEntityType.Architecture, PermissionLevel.Write);
        foreach (var system in model.Systems)
        {
            var existingSystem = await GetSystemByNameAsync(principal, project.Id, system.Key);
            if (existingSystem is null)
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
                    Description = system.Value.Description,
                    TestLead = system.Value.TestLead,
                    DevLead = system.Value.DevLead,
                    CreatedBy = principal.Identity?.Name ?? throw new InvalidOperationException("Invalid identity"),
                    ModifiedBy = principal.Identity?.Name ?? throw new InvalidOperationException("Invalid identity"),
                };
                await GenerateEmbeddingAsync(existingSystem);
                await _repository.AddSystemAsync(existingSystem);
            }
            else
            {
                await UpdateComponentAsync(principal, system, existingSystem);
                existingSystem.TestProjectId = project.Id;

                await _repository.UpdateSystemAsync(existingSystem);
            }
        }
    }

    private async Task UpdateComponentAsync(
        ClaimsPrincipal principal,
        KeyValuePair<string, ArchitecturalComponent> system, AritecturalComponentProjectEntity existingSystem)
    {
        bool hasDescriptionChanged = existingSystem.Description != system.Value.Description;

        existingSystem.Modified = DateTimeOffset.UtcNow;
        existingSystem.ModifiedBy = principal.Identity?.Name ?? throw new InvalidOperationException("Invalid identity");

        existingSystem.GlobPatterns = system.Value.Paths ?? existingSystem.GlobPatterns;
        existingSystem.Display = system.Value.Display ?? existingSystem.Display;
        existingSystem.DevLead = system.Value.DevLead ?? existingSystem.DevLead;
        existingSystem.TestLead = system.Value.DevLead ?? existingSystem.TestLead;
        existingSystem.Description = system.Value.Description ?? existingSystem.Description;
        if (hasDescriptionChanged)
        {
            await GenerateEmbeddingAsync(existingSystem);
        }
    }
}
