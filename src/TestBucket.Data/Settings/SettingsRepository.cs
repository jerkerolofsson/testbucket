using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Domain.Settings;
using TestBucket.Domain.Settings.Models;

namespace TestBucket.Data.Settings;
internal class SettingsRepository : ISettingsProvider
{
    private readonly IDbContextFactory<ApplicationDbContext> _dbContextFactory;
    private static GlobalSettings? _settings;

    public SettingsRepository(IDbContextFactory<ApplicationDbContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }

    public async Task SaveGlobalSettingsAsync(GlobalSettings settings)
    {
        _settings = settings;
        settings.Revision++;

        using var dbContext = await _dbContextFactory.CreateDbContextAsync();

        await dbContext.GlobalSettings.ExecuteDeleteAsync();
        await dbContext.GlobalSettings.AddAsync(settings);
        await dbContext.SaveChangesAsync();
    }

    public async Task<GlobalSettings> LoadGlobalSettingsAsync()
    {
        if(_settings is not null)
        {
            return _settings;
        }

        // Read from database
        using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        _settings = await dbContext.GlobalSettings.AsNoTracking().FirstOrDefaultAsync();

        if (_settings is null)
        {
            _settings = new GlobalSettings();
        }
        return _settings;
    }
}
