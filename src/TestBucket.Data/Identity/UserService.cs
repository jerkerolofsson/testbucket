using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Data.Identity.Models;

namespace TestBucket.Data.Identity;
internal class UserService : IUserService
{
    private readonly IDbContextFactory<ApplicationDbContext> _dbContextFactory;

    public UserService(IDbContextFactory<ApplicationDbContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
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
