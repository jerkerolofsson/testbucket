using System.Text.Json;
using System.Net.Http.Json;

using TestBucket.AdbProxy.Models;

namespace TestBucket.AdbProxy.Inform;
internal class DeviceInformer(HttpClient httpClient) : IDeviceInformer
{
    private InformSettings? _settings;

    private string GetSettingsPath()
    {
        var directoryPath = Path.Combine(System.Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "testbucket-adbproxy");
        Directory.CreateDirectory(directoryPath);
        return Path.Combine(directoryPath, "inform.json");
    }

    public async Task SaveInformSettingsAsync(InformSettings settings)
    {
        _settings = settings;
        var path = GetSettingsPath();
        await File.WriteAllTextAsync(path, JsonSerializer.Serialize(settings));
    }
    
    public async Task<InformSettings> LoadInformSettingsAsync()
    {
        if(_settings is not null)
        {
            return _settings;
        }

        var informUrl = Environment.GetEnvironmentVariable("ADB_PROXY_INFORM_URL");
        var auth = Environment.GetEnvironmentVariable("ADB_PROXY_AUTH_HEADER");

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

    public async Task InformAsync(AdbDevice[] devices, CancellationToken cancellationToken)
    {
        var settings = await LoadInformSettingsAsync();

        if(string.IsNullOrEmpty(settings.Url))
        {
            return;
        }

        using var request = new HttpRequestMessage(HttpMethod.Post, settings.Url);

        if (!string.IsNullOrEmpty(settings.AuthHeader))
        {
            bool success = request.Headers.TryAddWithoutValidation("Authorization", settings.AuthHeader);
        }

        request.Content = new StringContent(JsonSerializer.Serialize(devices), System.Text.Encoding.UTF8, "application/json");
        using var response = await httpClient.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();
    }
}
