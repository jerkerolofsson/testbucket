
using System.Text.Json;

namespace TestBucket.Runner.Settings;

public class SettingsManager
{
    public async Task<LocalRunnerSettings> LoadSettingsAsync()
    {
        string path = GetSettingsPath();
        LocalRunnerSettings? settings = null;
        if (File.Exists(path))
        {
            var json = await File.ReadAllTextAsync(path);
            settings = System.Text.Json.JsonSerializer.Deserialize<LocalRunnerSettings>(json);
        }

        if (settings is null)
        {
            settings = new();
        }
        var changed = AssignDefaults(settings);
        if (changed)
        {
            await SaveSettingsAsync(settings);
        }

        return settings;
    }

    private static bool AssignDefaults(LocalRunnerSettings settings)
    {
        bool changed = false;
        if (settings.Id is null)
        {
            settings.Id = Environment.GetEnvironmentVariable("TB_RUNNER_INSTANCE") ?? Guid.NewGuid().ToString();
            changed = true;
        }
        if (settings.Name is null)
        {
            settings.Name = Environment.GetEnvironmentVariable("TB_RUNNER_NAME") ?? "runner";
            changed = true;
        }
        if (Environment.GetEnvironmentVariable("TB_ACCESS_TOKEN") is string accessToken)
        {
            if (settings.AccessToken != accessToken)
            {
                settings.AccessToken = accessToken;
                changed = true;
            }
        }

        return changed;
    }

    private async Task SaveSettingsAsync(LocalRunnerSettings settings)
    {
        string path = GetSettingsPath();
        await File.WriteAllTextAsync(path, JsonSerializer.Serialize(settings));
    }

    private string GetSettingsPath()
    {
        string instance = Environment.GetEnvironmentVariable("TB_RUNNER_INSTANCE") ?? "instance0";

        string directoryPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "test-bucket", "runners", instance);
        if(!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }
        return Path.Combine(directoryPath, "settings.json");
    }
}
