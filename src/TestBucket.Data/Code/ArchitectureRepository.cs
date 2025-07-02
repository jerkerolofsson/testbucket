using TestBucket.Domain.Code;
using TestBucket.Domain.Code.Models;
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

    public async Task<PagedResult<ProductSystem>> SearchSystemsAsync(FilterSpecification<ProductSystem>[] filters, int offset, int count)
    {
        using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        var systesm = dbContext.ProductSystems.AsNoTracking();
        foreach (var filter in filters)
        {
            systesm = systesm.Where(filter.Expression);
        }
        long totalCount = await systesm.LongCountAsync();
        var items = systesm.OrderBy(x => x.Created).Skip(offset).Take(count);

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

    public async Task<PagedResult<ArchitecturalLayer>> SearchLayersAsync(FilterSpecification<ArchitecturalLayer>[] filters, int offset, int count)
    {
        using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        var layers = dbContext.ArchitecturalLayers.AsNoTracking();
        foreach (var filter in filters)
        {
            layers = layers.Where(filter.Expression);
        }
        long totalCount = await layers.LongCountAsync();
        var items = layers.OrderBy(x => x.Created).Skip(offset).Take(count);

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

    public async Task<PagedResult<Component>> SearchComponentsAsync(FilterSpecification<Component>[] filters, int offset, int count)
    {
        using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        var components = dbContext.Components.AsNoTracking();
        foreach (var filter in filters)
        {
            components = components.Where(filter.Expression);
        }
        long totalCount = await components.LongCountAsync();
        var items = components.OrderBy(x => x.Created).Skip(offset).Take(count);

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

    public async Task<PagedResult<Feature>> SearchFeaturesAsync(FilterSpecification<Feature>[] filters, int offset, int count)
    {
        using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        var features = dbContext.Features.AsNoTracking();
        foreach (var filter in filters)
        {
            features = features.Where(filter.Expression);
        }
        long totalCount = await features.LongCountAsync();
        var items = features.OrderBy(x => x.Created).Skip(offset).Take(count);

        return new PagedResult<Feature>
        {
            TotalCount = totalCount,
            Items = items.ToArray()
        };
    }
    #endregion Features
}
