using TestBucket.Domain.Shared.Specifications;
using TestBucket.Domain.Testing.Heuristics;
using TestBucket.Domain.Testing.Heuristics.Models;

namespace TestBucket.Data.Testing;

/// <summary>
/// Repository for managing <see cref="Heuristic"/> entities in the database.
/// </summary>
internal class HeuristicsRepository : IHeuristicsRepository
{
    private readonly IDbContextFactory<ApplicationDbContext> _dbContextFactory;

    /// <summary>
    /// Initializes a new instance of the <see cref="HeuristicsRepository"/> class.
    /// </summary>
    /// <param name="dbContextFactory">Factory for creating <see cref="ApplicationDbContext"/> instances.</param>
    public HeuristicsRepository(IDbContextFactory<ApplicationDbContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }

    /// <summary>
    /// Adds a new <see cref="Heuristic"/> to the database.
    /// </summary>
    /// <param name="heuristic">The heuristic to add.</param>
    public async Task AddAsync(Heuristic heuristic)
    {
        var dbContext = await _dbContextFactory.CreateDbContextAsync();
        dbContext.Heuristics.Add(heuristic);
        await dbContext.SaveChangesAsync();
    }

    /// <summary>
    /// Deletes a <see cref="Heuristic"/> by its identifier.
    /// </summary>
    /// <param name="id">The ID of the heuristic to delete.</param>
    public async Task DeleteAsync(long id)
    {
        var dbContext = await _dbContextFactory.CreateDbContextAsync();
        await dbContext.Heuristics.Where(x => x.Id == id).ExecuteDeleteAsync();
    }

    /// <summary>
    /// Retrieves a <see cref="Heuristic"/> by tenant and identifier.
    /// </summary>
    /// <param name="tenantId">The tenant ID.</param>
    /// <param name="id">The heuristic ID.</param>
    /// <returns>The matching <see cref="Heuristic"/>, or <c>null</c> if not found.</returns>
    public async Task<Heuristic?> GetByIdAsync(string tenantId, long id)
    {
        var dbContext = await _dbContextFactory.CreateDbContextAsync();
        return await dbContext.Heuristics.AsNoTracking().Where(x => x.Id == id && x.TenantId == tenantId).FirstOrDefaultAsync();
    }

    /// <summary>
    /// Searches for <see cref="Heuristic"/> entities using the specified filters and paging.
    /// </summary>
    /// <param name="filters">Array of filter specifications to apply.</param>
    /// <param name="offset">The number of items to skip.</param>
    /// <param name="count">The number of items to take.</param>
    /// <returns>A paged result containing the matching heuristics.</returns>
    public async Task<PagedResult<Heuristic>> SearchAsync(FilterSpecification<Heuristic>[] filters, int offset, int count)
    {
        var dbContext = await _dbContextFactory.CreateDbContextAsync();

        var heuristics = dbContext.Heuristics.AsNoTracking().AsQueryable();

        foreach (var filter in filters)
        {
            heuristics = heuristics.Where(filter.Expression);
        }

        var totalCount = await heuristics.LongCountAsync();

        return new PagedResult<Heuristic>
        {
            TotalCount = totalCount,
            Items = await heuristics.Skip(offset).Take(count).ToArrayAsync()
        };
    }

    /// <summary>
    /// Updates an existing <see cref="Heuristic"/> in the database.
    /// </summary>
    /// <param name="heuristic">The heuristic to update.</param>
    public async Task UpdateAsync(Heuristic heuristic)
    {
        var dbContext = await _dbContextFactory.CreateDbContextAsync();
        dbContext.Heuristics.Update(heuristic);
        await dbContext.SaveChangesAsync();
    }
}