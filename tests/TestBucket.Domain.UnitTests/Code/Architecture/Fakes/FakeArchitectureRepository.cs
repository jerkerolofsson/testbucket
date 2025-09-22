using System.Collections.Concurrent;

using TestBucket.Contracts;
using TestBucket.Domain.Code;
using TestBucket.Domain.Code.Models;

internal class FakeArchitectureRepository : IArchitectureRepository
{
    private readonly ConcurrentDictionary<long, ProductSystem> _systems = new();
    private readonly ConcurrentDictionary<long, ArchitecturalLayer> _layers = new();
    private readonly ConcurrentDictionary<long, Component> _components = new();
    private readonly ConcurrentDictionary<long, Feature> _features = new();

    private long _systemId = 1;
    private long _layerId = 1;
    private long _componentId = 1;
    private long _featureId = 1;

    public Task AddSystemAsync(ProductSystem system)
    {
        system.Id = _systemId++;
        _systems[system.Id] = system;
        return Task.CompletedTask;
    }

    public Task<PagedResult<ProductSystem>> SemanticSearchSystemsAsync(ReadOnlyMemory<float> embeddingVector, FilterSpecification<ProductSystem>[] filters, int offset, int count)
        => Task.FromResult(SearchPaged(_systems.Values, filters, offset, count));

    public Task<PagedResult<ProductSystem>> SearchSystemsAsync(FilterSpecification<ProductSystem>[] filters, int offset, int count)
        => Task.FromResult(SearchPaged(_systems.Values, filters, offset, count));

    public Task UpdateSystemAsync(ProductSystem system)
    {
        _systems[system.Id] = system;
        return Task.CompletedTask;
    }

    public Task<ProductSystem?> GetSystemAsync(long id)
        => Task.FromResult(_systems.TryGetValue(id, out var s) ? s : null);

    public Task DeleteSystemAsync(long id)
    {
        _systems.TryRemove(id, out _);
        return Task.CompletedTask;
    }

    public Task AddLayerAsync(ArchitecturalLayer component)
    {
        component.Id = _layerId++;
        _layers[component.Id] = component;
        return Task.CompletedTask;
    }

    public Task<PagedResult<ArchitecturalLayer>> SemanticSearchLayersAsync(ReadOnlyMemory<float> embeddingVector, FilterSpecification<ArchitecturalLayer>[] filters, int offset, int count)
        => Task.FromResult(SearchPaged(_layers.Values, filters, offset, count));

    public Task<PagedResult<ArchitecturalLayer>> SearchLayersAsync(FilterSpecification<ArchitecturalLayer>[] filters, int offset, int count)
        => Task.FromResult(SearchPaged(_layers.Values, filters, offset, count));

    public Task UpdateLayerAsync(ArchitecturalLayer component)
    {
        _layers[component.Id] = component;
        return Task.CompletedTask;
    }

    public Task<ArchitecturalLayer?> GetLayerAsync(long id)
        => Task.FromResult(_layers.TryGetValue(id, out var l) ? l : null);

    public Task DeleteLayerAsync(long id)
    {
        _layers.TryRemove(id, out _);
        return Task.CompletedTask;
    }

    public Task AddComponentAsync(Component component)
    {
        component.Id = _componentId++;
        _components[component.Id] = component;
        return Task.CompletedTask;
    }

    public Task<PagedResult<Component>> SemanticSearchComponentsAsync(ReadOnlyMemory<float> embeddingVector, FilterSpecification<Component>[] filters, int offset, int count)
        => Task.FromResult(SearchPaged(_components.Values, filters, offset, count));

    public Task<PagedResult<Component>> SearchComponentsAsync(FilterSpecification<Component>[] filters, int offset, int count)
        => Task.FromResult(SearchPaged(_components.Values, filters, offset, count));

    public Task UpdateComponentAsync(Component component)
    {
        _components[component.Id] = component;
        return Task.CompletedTask;
    }

    public Task<Component?> GetComponentAsync(long id)
        => Task.FromResult(_components.TryGetValue(id, out var c) ? c : null);

    public Task DeleteComponentAsync(long id)
    {
        _components.TryRemove(id, out _);
        return Task.CompletedTask;
    }

    public Task<Feature?> GetFeatureAsync(long id)
        => Task.FromResult(_features.TryGetValue(id, out var f) ? f : null);

    public Task AddFeatureAsync(Feature feature)
    {
        feature.Id = _featureId++;
        _features[feature.Id] = feature;
        return Task.CompletedTask;
    }

    public Task<PagedResult<Feature>> SemanticSearchFeaturesAsync(ReadOnlyMemory<float> embeddingVector, FilterSpecification<Feature>[] filters, int offset, int count)
        => Task.FromResult(SearchPaged(_features.Values, filters, offset, count));

    public Task<PagedResult<Feature>> SearchFeaturesAsync(FilterSpecification<Feature>[] filters, int offset, int count)
        => Task.FromResult(SearchPaged(_features.Values, filters, offset, count));

    public Task UpdateFeatureAsync(Feature feature)
    {
        _features[feature.Id] = feature;
        return Task.CompletedTask;
    }

    public Task DeleteFeatureAsync(long id)
    {
        _features.TryRemove(id, out _);
        return Task.CompletedTask;
    }

    private static PagedResult<T> SearchPaged<T>(IEnumerable<T> source, FilterSpecification<T>[] filters, int offset, int count)
    {
        var filtered = source;
        if (filters != null)
        {
            foreach (var filter in filters)
                filtered = filtered.Where(filter.IsMatch);
        }
        var arr = filtered.Skip(offset).Take(count).ToArray();
        return new PagedResult<T> { TotalCount = filtered.Count(), Items = arr };
    }
}