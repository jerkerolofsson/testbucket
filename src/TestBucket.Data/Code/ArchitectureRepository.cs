using Pgvector.EntityFrameworkCore;

using TestBucket.Domain.Code;
using TestBucket.Domain.Code.Models;
using TestBucket.Domain.Issues.Models;
using TestBucket.Domain.Shared.Specifications;

namespace TestBucket.Data.Code;
public class ArchitectureRepository : IArchitectureRepository
{
    private readonly IDbContextFactory<ApplicationDbContext> _dbContextFactory;

    public ArchitectureRepository(IDbContextFactory<ApplicationDbContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }

    #region Systems
    public async Task<ProductSystem?> GetSystemAsync(long id)
    {
        using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        return await dbContext.ProductSystems.AsNoTracking().Where(x => x.Id == id).FirstOrDefaultAsync();
    }
    public async Task DeleteSystemAsync(long id)
    {
        using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        await dbContext.ProductSystems.AsNoTracking().Where(x => x.Id == id).ExecuteDeleteAsync();
    }
    public async Task AddSystemAsync(ProductSystem system)
    {
        using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        dbContext.ProductSystems.Add(system);
        await dbContext.SaveChangesAsync();
    }
    public async Task UpdateSystemAsync(ProductSystem system)
    {
        using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        dbContext.ProductSystems.Update(system);
        await dbContext.SaveChangesAsync();
    }
    public async Task<PagedResult<ProductSystem>> SemanticSearchSystemsAsync(ReadOnlyMemory<float> embeddingVector, FilterSpecification<ProductSystem>[] filters, int offset, int count)
    {
        var vector = new Pgvector.Vector(embeddingVector);

        using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        var components = dbContext.ProductSystems.AsNoTracking();
        foreach (var filter in filters)
        {
            components = components.Where(filter.Expression);
        }
        var totalCount = components.LongCount();
        components = components.Where(x => x.Embedding != null);

        var page = components.Select(x => new { Issue = x, Distance = x.Embedding!.CosineDistance(vector) })
            .Where(x => x.Distance < 1.0)
            .OrderBy(x => x.Distance)
            .Skip(offset)
            .Take(count)
            .Select(x => x.Issue);

        return new PagedResult<ProductSystem>()
        {
            TotalCount = totalCount,
            Items = await page.ToArrayAsync()
        };
    }
    public async Task<PagedResult<ProductSystem>> SearchSystemsAsync(FilterSpecification<ProductSystem>[] filters, int offset, int count)
    {
        using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        var systesm = dbContext.ProductSystems.AsNoTracking();
        foreach (var filter in filters)
        {
            systesm = systesm.Where(filter.Expression);
        }
        long totalCount = await systesm.LongCountAsync();
        var items = systesm.OrderBy(x => x.Name).Skip(offset).Take(count);

        return new PagedResult<ProductSystem>
        {
            TotalCount = totalCount,
            Items = items.ToArray()
        };
    }
    #endregion Systems

    #region Layers

    public async Task<ArchitecturalLayer?> GetLayerAsync(long id)
    {
        using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        return await dbContext.ArchitecturalLayers.AsNoTracking().Where(x => x.Id == id).FirstOrDefaultAsync();
    }

    public async Task DeleteLayerAsync(long id)
    {
        using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        await dbContext.ArchitecturalLayers.AsNoTracking().Where(x => x.Id == id).ExecuteDeleteAsync();
    }
    public async Task AddLayerAsync(ArchitecturalLayer layer)
    {
        using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        dbContext.ArchitecturalLayers.Add(layer);
        await dbContext.SaveChangesAsync();
    }
    public async Task UpdateLayerAsync(ArchitecturalLayer layer)
    {
        using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        dbContext.ArchitecturalLayers.Update(layer);
        await dbContext.SaveChangesAsync();
    }
   
    public async Task<PagedResult<ArchitecturalLayer>> SemanticSearchLayersAsync(ReadOnlyMemory<float> embeddingVector, FilterSpecification<ArchitecturalLayer>[] filters, int offset, int count)
    {
        var vector = new Pgvector.Vector(embeddingVector);

        using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        var components = dbContext.ArchitecturalLayers.AsNoTracking();
        foreach (var filter in filters)
        {
            components = components.Where(filter.Expression);
        }
        var totalCount = components.LongCount();
        components = components.Where(x => x.Embedding != null);

        var page = components.Select(x => new { Issue = x, Distance = x.Embedding!.CosineDistance(vector) })
            .Where(x => x.Distance < 1.0)
            .OrderBy(x => x.Distance)
            .Skip(offset)
            .Take(count)
            .Select(x => x.Issue);

        return new PagedResult<ArchitecturalLayer>()
        {
            TotalCount = totalCount,
            Items = await page.ToArrayAsync()
        };
    }
    public async Task<PagedResult<ArchitecturalLayer>> SearchLayersAsync(FilterSpecification<ArchitecturalLayer>[] filters, int offset, int count)
    {
        using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        var layers = dbContext.ArchitecturalLayers.AsNoTracking();
        foreach (var filter in filters)
        {
            layers = layers.Where(filter.Expression);
        }
        long totalCount = await layers.LongCountAsync();
        var items = layers.OrderBy(x => x.Name).Skip(offset).Take(count);

        return new PagedResult<ArchitecturalLayer>
        {
            TotalCount = totalCount,
            Items = items.ToArray()
        };
    }
    #endregion Layers


    #region Components
    public async Task<Component?> GetComponentAsync(long id)
    {
        using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        return await dbContext.Components.AsNoTracking().Where(x => x.Id == id).FirstOrDefaultAsync();
    }
    public async Task DeleteComponentAsync(long id)
    {
        using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        await dbContext.Components.AsNoTracking().Where(x => x.Id == id).ExecuteDeleteAsync();
    }

    public async Task AddComponentAsync(Component component)
    {
        using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        dbContext.Components.Add(component);
        await dbContext.SaveChangesAsync();
    }
    public async Task UpdateComponentAsync(Component component)
    {
        using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        dbContext.Components.Update(component);
        await dbContext.SaveChangesAsync();
    }
    public async Task<PagedResult<Component>> SemanticSearchComponentsAsync(ReadOnlyMemory<float> embeddingVector, FilterSpecification<Component>[] filters, int offset, int count)
    {
        var vector = new Pgvector.Vector(embeddingVector);

        using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        var components = dbContext.Components.AsNoTracking();
        foreach (var filter in filters)
        {
            components = components.Where(filter.Expression);
        }
        var totalCount = components.LongCount();
        components = components.Where(x => x.Embedding != null);

        var page = components.Select(x => new { Issue = x, Distance = x.Embedding!.CosineDistance(vector) })
            .Where(x => x.Distance < 1.0)
            .OrderBy(x => x.Distance)
            .Skip(offset)
            .Take(count)
            .Select(x => x.Issue);

        return new PagedResult<Component>()
        {
            TotalCount = totalCount,
            Items = await page.ToArrayAsync()
        };
    }
    public async Task<PagedResult<Component>> SearchComponentsAsync(FilterSpecification<Component>[] filters, int offset, int count)
    {
        using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        var components = dbContext.Components.AsNoTracking();
        foreach (var filter in filters)
        {
            components = components.Where(filter.Expression);
        }
        long totalCount = await components.LongCountAsync();
        var items = components.OrderBy(x => x.Name).Skip(offset).Take(count);

        return new PagedResult<Component>
        {
            TotalCount = totalCount,
            Items = items.ToArray()
        };
    }
    #endregion Components

    #region Features
    public async Task<Feature?> GetFeatureAsync(long id)
    {
        using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        return await dbContext.Features.AsNoTracking().Where(x => x.Id == id).FirstOrDefaultAsync();
    }

    public async Task AddFeatureAsync(Feature feature)
    {
        using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        dbContext.Features.Add(feature);
        await dbContext.SaveChangesAsync();
    }
    public async Task UpdateFeatureAsync(Feature feature)
    {
        using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        dbContext.Features.Update(feature);
        await dbContext.SaveChangesAsync();
    }
    public async Task DeleteFeatureAsync(long id)
    {
        using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        await dbContext.Features.AsNoTracking().Where(x=>x.Id == id).ExecuteDeleteAsync();
    }
    public async Task<PagedResult<Feature>> SemanticSearchFeaturesAsync(ReadOnlyMemory<float> embeddingVector, FilterSpecification<Feature>[] filters, int offset, int count)
    {
        var vector = new Pgvector.Vector(embeddingVector);
        using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        var components = dbContext.Features.AsNoTracking();
        foreach (var filter in filters)
        {
            components = components.Where(filter.Expression);
        }
        var totalCount = components.LongCount();
        components = components.Where(x => x.Embedding != null);

        var page = components.Select(x => new { Component = x, Distance = x.Embedding!.CosineDistance(vector) })
            .Where(x => x.Distance < 1.0)
            .OrderBy(x => x.Distance)
            .Skip(offset)
            .Take(count)
            .Select(x => x.Component);

        return new PagedResult<Feature>()
        {
            TotalCount = totalCount,
            Items = await page.ToArrayAsync()
        };
    }
    public async Task<PagedResult<Feature>> SearchFeaturesAsync(FilterSpecification<Feature>[] filters, int offset, int count)
    {
        using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        var features = dbContext.Features.AsNoTracking();
        foreach (var filter in filters)
        {
            features = features.Where(filter.Expression);
        }
        long totalCount = await features.LongCountAsync();
        var items = features.OrderBy(x => x.Name).Skip(offset).Take(count);

        return new PagedResult<Feature>
        {
            TotalCount = totalCount,
            Items = items.ToArray()
        };
    }
    #endregion Features
}
