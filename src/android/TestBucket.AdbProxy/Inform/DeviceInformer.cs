using System.Text.Json;

using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.Extensions.Diagnostics.HealthChecks;

using TestBucket.AdbProxy.Models;
using TestBucket.Contracts.TestResources;
using TestBucket.ResourceServer.Utilities;
using TestBucket.Traits.Core;

namespace TestBucket.AdbProxy.Inform;

/// <summary>
/// Provides functionality to inform an external system about connected Android devices,
/// manage inform settings, and persist configuration for device reporting.
/// </summary>
internal class DeviceInformer(HttpClient httpClient, IServer Server) : IDeviceInformer
{
    private InformSettings? _settings;

    /// <summary>
    /// Gets the file path for storing inform settings in the user's application data directory.
    /// </summary>
    /// <returns>The full file path to the inform settings JSON file.</returns>
    private string GetSettingsPath()
    {
        var directoryPath = Path.Combine(System.Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "testbucket-adbproxy");
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

    /// <summary>
    /// Informs an external system about the current state of connected Android devices.
    /// </summary>
    /// <param name="devices">The array of <see cref="AdbDevice"/> objects to report.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous inform operation.</returns>
    public async Task InformAsync(AdbDevice[] devices, CancellationToken cancellationToken)
    {
        var settings = await LoadInformSettingsAsync();

        if (string.IsNullOrEmpty(settings.Url))
        {
            return;
        }

        List<TestResourceDto> resources = new();
        var owner = ResourceServerOwner.Name;
        AddDevicesAsResources(devices, resources, owner);
        AddMcpResource(resources, owner);

        using var request = new HttpRequestMessage(HttpMethod.Post, settings.Url);

        if (!string.IsNullOrEmpty(settings.AuthHeader))
        {
            bool success = request.Headers.TryAddWithoutValidation("Authorization", settings.AuthHeader);
        }

        request.Content = new StringContent(JsonSerializer.Serialize(resources), System.Text.Encoding.UTF8, "application/json");
        using var response = await httpClient.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();
    }

    private void AddMcpResource(List<TestResourceDto> resources, string owner)
    {
        var hostname = PublicHostnameDetector.GetPublicHostname() ?? "localhost";
        var publicPort = PublicHostnameDetector.GetPublicPort();

        var addressFeature = Server.Features.Get<IServerAddressesFeature>();
        if (addressFeature != null)
        {
            foreach (var address in addressFeature.Addresses)
            {
                if (address.StartsWith("http:"))
                {
                    if (Uri.TryCreate(address, UriKind.Absolute, out var uri))
                    {
                        string baseUrl = "http://" + hostname + ":" + publicPort ?? uri.Port.ToString();

                        resources.Add(new TestResourceDto
                        {
                            Name = $"adb-mcp-server@{hostname}",
                            Owner = owner,
                            ResourceId = baseUrl + "-adb-mcp-server",
                            Model = "AdbProxy",
                            Manufacturer = "TestBucket",
                            Types = ["appium-mcp", "adb-mcp", "mcp-server"],
                            Health = HealthStatus.Healthy,
                            Variables =
                            {
                                ["url"] = address.TrimEnd('/') + "/mcp"
                            }
                        });
                    }
                }
            }
        }
    }

    private static void AddDevicesAsResources(AdbDevice[] devices, List<TestResourceDto> resources, string owner)
    {
        foreach (var device in devices)
        {
            HealthStatus status = device.Status switch
            {
                "device" => HealthStatus.Healthy,
                "unauhorized" => HealthStatus.Unhealthy,
                "offline" => HealthStatus.Unhealthy,
                _ => HealthStatus.Unhealthy
            };

            var resource = new TestResourceDto
            {
                Name = device.ModelInfo.Manufacturer + " " + device.ModelInfo.Name,
                Types = ["phone", "android"],
                Health = status,
                Owner = owner,
                ResourceId = device.DeviceId,
                Model = device.ModelInfo.Name,
                Manufacturer = device.ModelInfo.Manufacturer,
            };
            resource.Variables["api-level"] = device.ApiLevel.ToString();
            if (device.Url is not null)
            {
                resource.Variables["url"] = device.Url;
            }
            if (device.AppiumUrl is not null)
            {
                resource.Variables["appium.url"] = device.AppiumUrl;
            }
            if (device.AppiumPort > 0)
            {
                resource.Variables["appium.port"] = device.AppiumPort.ToString();
            }

            resource.Variables[TargetTraitNames.SutOperatingSystemName] = "Android";
            if (device.Version.VersionNumber is not null)
            {
                resource.Variables[TargetTraitNames.SutOperatingSystemVersion] = device.Version.VersionNumber;
            }
            if (device.SocModel is not null)
            {
                resource.Variables[TargetTraitNames.SutPlatformName] = device.SocModel;
            }
            if (device.SoftwareVersion is not null)
            {
                resource.Variables[TargetTraitNames.SoftwareVersion] = device.SoftwareVersion;
            }
            resources.Add(resource);
        }
    }
}