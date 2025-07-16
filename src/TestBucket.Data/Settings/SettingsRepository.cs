using System.Text.Json;

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
        dbContext.GlobalSettings.Add(settings);
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

    public async Task SaveDomainSettingsAsync<T>(string tenantId, long? projectId, T setting) where T : class
    {
        using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        string type = typeof(T).FullName ?? "";

        var existingSetting = await dbContext.DomainSettings
            .Where(s => s.TenantId == tenantId && s.TestProjectId == projectId && s.Type == type)
            .FirstOrDefaultAsync();

        if (existingSetting is not null)
        {
            existingSetting.Json = JsonSerializer.Serialize(setting);
            dbContext.DomainSettings.Update(existingSetting);
        }
        else
        {
            existingSetting = new DomainSettings
            {
                Type = type,
                TestProjectId = projectId,
                TenantId = tenantId,
                Json = JsonSerializer.Serialize(setting)
            };
            dbContext.DomainSettings.Add(existingSetting);
        }
        await dbContext.SaveChangesAsync();
    }

    public async Task<T?> GetDomainSettingsAsync<T>(string tenantId, long? projectId) where T : class
    {
        using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        string type = typeof(T).FullName ?? "";

        var existingSetting = await dbContext.DomainSettings
            .Where(s => s.TenantId == tenantId && s.TestProjectId == projectId && s.Type == type)
            .FirstOrDefaultAsync();

        if(existingSetting is null)
        {
            return default(T);
        }

        return JsonSerializer.Deserialize<T>(existingSetting.Json) ?? default(T);
    }

}
