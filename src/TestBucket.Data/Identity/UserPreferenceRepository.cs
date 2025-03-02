using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Domain.Settings.Models;

namespace TestBucket.Data.Identity;
internal class UserPreferenceRepository : IUserPreferenceRepository
{
    private readonly IDbContextFactory<ApplicationDbContext> _dbContextFactory;

    public UserPreferenceRepository(IDbContextFactory<ApplicationDbContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }

    public async Task SaveUserPreferencesAsync(UserPreferences userPreferences)
    {
        using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        if (userPreferences.Id > 0)
        {
            dbContext.UserPreferences.Update(userPreferences);
        }
        else
        {
            await dbContext.UserPreferences.AddAsync(userPreferences);
        }
        await dbContext.SaveChangesAsync();
    }

    public async Task<UserPreferences?> GetUserPreferencesAsync(string tenantId, string userName)
    {
        using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        return await dbContext.UserPreferences.AsNoTracking()
            .Where(x => x.UserName == userName && x.TenantId == tenantId)
            .FirstOrDefaultAsync();
    }
}
