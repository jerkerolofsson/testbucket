using System.Text.Json;
using TestBucket.Contracts.TestResources;
using TestBucket.Servers.NodeResourceServer.Models;

namespace TestBucket.Servers.NodeResourceServer.Services.Inform;

/// <summary>
/// Provides functionality to inform an external system about connected Android devices,
/// manage inform settings, and persist configuration for device reporting.
/// </summary>
internal class ServiceInformer(HttpClient httpClient) : IServiceInformer
{
    private InformSettings? _settings;

    /// <summary>
    /// Gets the file path for storing inform settings in the user's application data directory.
    /// </summary>
    /// <returns>The full file path to the inform settings JSON file.</returns>
    private string GetSettingsPath()
    {
        var directoryPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "testbucket-noderesourceserver");
        Directory.CreateDirectory(directoryPath);
        return Path.Combine(directoryPath, "inform.json");
    }

    /// <summary>
    /// Saves the inform settings to disk and updates the in-memory settings.
    /// </summary>
    /// <param name="settings">The <see cref="InformSettings"/> to save.</param>
    /// <returns>A task representing the asynchronous save operation.</returns>
    public async Task SaveInformSettingsAsync(InformSettings settings)
    {
        _settings = settings;
        var path = GetSettingsPath();
        await File.WriteAllTextAsync(path, JsonSerializer.Serialize(settings));
    }

    /// <summary>
    /// Loads the inform settings from disk or environment variables, or creates defaults if not found.
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous load operation. The task result contains the loaded <see cref="InformSettings"/>.
    /// </returns>
    public async Task<InformSettings> LoadInformSettingsAsync()
    {
        if (_settings is not null)
        {
            return _settings;
        }

        var informUrl = Environment.GetEnvironmentVariable("TB_INFORM_URL");
        var auth = Environment.GetEnvironmentVariable("TB_AUTH_HEADER");

        var path = GetSettingsPath();
        if (File.Exists(path))
        {
            var json = await File.ReadAllTextAsync(path);
            try
            {
                _settings = JsonSerializer.Deserialize<InformSettings>(json);
                if (_settings != null)
                {
                    if (!string.IsNullOrEmpty(informUrl))
                    {
                        _settings.Url = informUrl;
                    }
                    if (!string.IsNullOrEmpty(auth))
                    {
                        _settings.AuthHeader = auth;
                    }

                    return _settings;
                }
            }
            catch (JsonException) { }
        }

        // Create default
        _settings = new InformSettings()
        {
            Url = informUrl,
            AuthHeader = auth,
        };
        return _settings;
    }

    public async Task InformAsync(TestResourceDto[] resources, CancellationToken cancellationToken)
    {
        var settings = await LoadInformSettingsAsync();

        if (string.IsNullOrEmpty(settings.Url))
        {
            return;
        }

        var owner = Environment.GetEnvironmentVariable("SERVER_UUID") ?? "no-uuid-configured";
        using var request = new HttpRequestMessage(HttpMethod.Post, settings.Url);

        if (!string.IsNullOrEmpty(settings.AuthHeader))
        {
            bool success = request.Headers.TryAddWithoutValidation("Authorization", settings.AuthHeader);
        }

        request.Content = new StringContent(JsonSerializer.Serialize(resources), System.Text.Encoding.UTF8, "application/json");
        using var response = await httpClient.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();
    }
}