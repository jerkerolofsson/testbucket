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

    /// <summary>
    /// Saves domain settings
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="tenantId"></param>
    /// <param name="projectId"></param>
    /// <param name="setting"></param>
    /// <returns></returns>
    Task SaveDomainSettingsAsync<T>(string tenantId, long? projectId, T setting) where T : class;

    /// <summary>
    /// Loads domain settings
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="tenantId"></param>
    /// <param name="projectId"></param>
    /// <returns></returns>
    Task<T?> GetDomainSettingsAsync<T>(string tenantId, long? projectId) where T : class;
}
