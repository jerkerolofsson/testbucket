namespace TestBucket.Domain.Settings;
public interface ISettingsProvider
{
    /// <summary>
    /// Loads global settings
    /// </summary>
    /// <returns></returns>
    Task<GlobalSettings> LoadGlobalSettingsAsync();

    /// <summary>
    /// Saves global settings
    /// </summary>
    /// <param name="settings"></param>
    /// <returns></returns>
    Task SaveGlobalSettingsAsync(GlobalSettings settings);

    Task SaveDomainSettingsAsync<T>(string tenantId, long? projectId, T setting) where T : class;
    Task<T?> GetDomainSettingsAsync<T>(string tenantId, long? projectId) where T : class;
}
