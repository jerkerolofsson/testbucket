using TestBucket.Contracts.TestResources;

namespace TestBucket.ResourceServer.Services.Inform;

public interface IResourceInformer
{
    /// <summary>
    /// Informs another system about device changes
    /// </summary>
    /// <param name="devices"></param>
    /// <returns></returns>
    Task InformAsync(TestResourceDto[] services, CancellationToken cancellationToken);

    /// <summary>
    /// Loads settings
    /// </summary>
    /// <returns></returns>
    Task<InformSettings> LoadInformSettingsAsync();

    /// <summary>
    /// Saves settings
    /// </summary>
    /// <param name="settings"></param>
    /// <returns></returns>
    Task SaveInformSettingsAsync(InformSettings settings);
}
