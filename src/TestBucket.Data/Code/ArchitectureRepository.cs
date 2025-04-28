using System;
using System.Linq;

using TestBucket.Domain.Code;
using TestBucket.Domain.Code.Models;
using TestBucket.Domain.Requirements.Models;
using TestBucket.Domain.Shared.Specifications;
using TestBucket.Domain.Tenants.Models;

using UglyToad.PdfPig.Filters;

namespace TestBucket.Data.Code;
public class ArchitectureRepository : IArchitectureRepository
{
    private readonly IDbContextFactory<ApplicationDbContext> _dbContextFactory;

    public ArchitectureRepository(IDbContextFactory<ApplicationDbContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
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
        var commits = dbContext.ProductSystems.AsNoTracking();
        foreach (var filter in filters)
        {
            commits = commits.Where(filter.Expression);
        }
        long totalCount = await commits.LongCountAsync();
        var items = commits.OrderBy(x => x.Created).Skip(offset).Take(count);

        return new PagedResult<ProductSystem>
        {
            TotalCount = totalCount,
            Items = items.ToArray()
        };
    }
}
