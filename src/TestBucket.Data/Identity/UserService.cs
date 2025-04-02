using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace TestBucket.Data.Identity;
internal class UserService : IUserService
{
    private readonly IDbContextFactory<ApplicationDbContext> _dbContextFactory;

    public UserService(IDbContextFactory<ApplicationDbContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }

    #region API Keys
    public async Task AddApiKeyAsync(ApplicationUserApiKey apiKey)
    {
        using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        await dbContext.ApiKeys.AddAsync(apiKey);
        await dbContext.SaveChangesAsync();
    }

    public async Task DeleteApiKeyAsync(string userId, string tenantId, long apiKeyId)
    {
        using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        await dbContext.ApiKeys.Where(x => x.ApplicationUserId == userId && x.TenantId == tenantId && x.Id == apiKeyId).ExecuteDeleteAsync();
    }

    public async Task<ApplicationUserApiKey[]> GetApiKeysAsync(string userId, string tenantId)
    {
        using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        return await dbContext.ApiKeys.AsNoTracking().Where(x => x.ApplicationUserId == userId && x.TenantId == tenantId).ToArrayAsync();
    }
    #endregion

    public async Task<ApplicationUser?> FindAsync(ClaimsPrincipal principal, string tenantId)
    {
        var email = principal.Claims.Where(x => x.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress").FirstOrDefault() ?? throw new Exception("principal has no email");

        using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        return await dbContext.Users.AsNoTracking().Where(x => x.TenantId == tenantId && x.Email == email.Value).FirstOrDefaultAsync();
    }


    public async Task<PagedResult<ApplicationUser>> SearchAsync(string tenantId, SearchQuery query, Predicate<ApplicationUser> predicate, CancellationToken cancellationToken = default)
    {
        using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        var users = dbContext.Users.AsNoTracking().Where(x => x.TenantId == tenantId);
        long totalCount = await users.LongCountAsync();

        // Apply filter
        if (query.Text is not null)
        {
            users = users.Where(x => x.UserName != null && x.UserName.ToLower().Contains(query.Text.ToLower()));
        }
        users = users.Where(x => predicate(x));

        var items = users.OrderBy(x => x.UserName).Skip(query.Offset).Take(query.Count);
        return new PagedResult<ApplicationUser>
        {
            TotalCount = totalCount,
            Items = users.ToArray()
        };
    }
    public async Task<PagedResult<ApplicationUser>> SearchAsync(string tenantId, SearchQuery query, CancellationToken cancellationToken = default)
    {
        using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        var users = dbContext.Users.AsNoTracking().Where(x => x.TenantId == tenantId);
        long totalCount = await users.LongCountAsync();

        // Apply filter
        if (query.Text is not null)
        {
            users = users.Where(x => x.UserName != null && x.UserName.ToLower().Contains(query.Text.ToLower()));
        }
        var items = users.OrderBy(x => x.UserName).Skip(query.Offset).Take(query.Count);
        return new PagedResult<ApplicationUser>
        {
            TotalCount = totalCount,
            Items = users.ToArray()
        };
    }
}
