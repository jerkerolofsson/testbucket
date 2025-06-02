using OneOf;

using TestBucket.Domain.Errors;

namespace TestBucket.Data.Tenants;
internal class TenantRepository : ITenantRepository
{
    private readonly IDbContextFactory<ApplicationDbContext> _dbContextFactory;

    public TenantRepository(IDbContextFactory<ApplicationDbContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }


    /// <summary>
    /// Creates a new tenant
    /// </summary>
    /// <param name="name">Name of tenant</param>
    /// <param name="tenantId">Tenant ID</param>
    /// <returns></returns>
    public async Task<OneOf<Tenant, AlreadyExistsError>> CreateAsync(string name, string tenantId)
    {
        var tenant = new Tenant()
        {
            Id = tenantId,
            Name = name,
            CanRegisterNewUsers = true,
            RequireConfirmedAccount = true,
        };

        if (await ExistsAsync(tenantId))
        {
            return new AlreadyExistsError();
        }
        using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        dbContext.Tenants.Add(tenant);
        await dbContext.SaveChangesAsync();

        return tenant;
    }

    /// <inheritdoc/>
    public async Task<Tenant?> GetTenantByIdAsync(string tenantId)
    {
        using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        return await dbContext.Tenants.Where(x => x.Id == tenantId).FirstOrDefaultAsync();

    }

    /// <inheritdoc/>
    public async Task<bool> ExistsAsync(string tenantId)
    {
        using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        return await dbContext.Tenants.Where(x => x.Id == tenantId).AnyAsync();

    }

    /// <inheritdoc/>
    public async Task<PagedResult<Tenant>> SearchAsync(SearchQuery query)
    {
        using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        var tenants = dbContext.Tenants.AsQueryable();

        // Apply filter
        if (query.Text is not null)
        {
            tenants = tenants.Where(x => x.Name.ToLower().Contains(query.Text.ToLower()));
        }

        long totalCount = await tenants.LongCountAsync();
        var items = tenants.OrderBy(x => x.Name).Skip(query.Offset).Take(query.Count);

        return new PagedResult<Tenant>
        {
            TotalCount = totalCount,
            Items = items.ToArray()
        };
    }

    public async Task UpdateTenantAsync(Tenant tenant)
    {
        using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        dbContext.Tenants.Update(tenant);
        await dbContext.SaveChangesAsync();
    }

    public async Task DeleteTenantAsync(string tenantId)
    {
        using var dbContext = await _dbContextFactory.CreateDbContextAsync();

        // Find the tenant
        var tenant = await dbContext.Tenants.FirstOrDefaultAsync(t => t.Id == tenantId);
        if (tenant == null)
        {
            return; // Tenant does not exist, nothing to delete
        }

        // Remove related entities (if needed, adjust as per cascade rules)
        dbContext.Teams.RemoveRange(dbContext.Teams.Where(t => t.TenantId == tenantId));
        dbContext.Projects.RemoveRange(dbContext.Projects.Where(p => p.TenantId == tenantId));
        dbContext.Files.RemoveRange(dbContext.Files.Where(f => f.TenantId == tenantId));
        dbContext.UserPreferences.RemoveRange(dbContext.UserPreferences.Where(u => u.TenantId == tenantId));
        dbContext.Users.RemoveRange(dbContext.Users.Where(u => u.TenantId == tenantId));

        // Remove the tenant itself
        dbContext.Tenants.Remove(tenant);

        await dbContext.SaveChangesAsync();
    }
}
