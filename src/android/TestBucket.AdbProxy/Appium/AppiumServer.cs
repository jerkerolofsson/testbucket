
using Docker.DotNet.Models;

using Microsoft.Extensions.Logging;

using RedMaple.Orchestrator.DockerCompose;
using RedMaple.Orchestrator.DockerCompose.Models;

using TestBucket.AdbProxy.Models;
using TestBucket.AdbProxy.Proxy;

namespace TestBucket.AdbProxy.Appium;

/// <summary>
/// Brings up or down a appium container for the specific device
/// </summary>
public class AppiumServer : IAsyncDisposable, IProgress<string>
{
    private readonly AdbDevice _device;
    private readonly IPortGenerator _portGenerator;
    private readonly IDockerCompose _dockerCompose;
    private readonly ILogger<AppiumServer> _logger;

    public AppiumServer(AdbDevice device, IPortGenerator portGenerator, IDockerCompose dockerCompose, ILogger<AppiumServer> logger)
    {
        _device = device;
        _portGenerator = portGenerator;
        _dockerCompose = dockerCompose;
        _logger = logger;
    }

    public int Port { get; private set; }

    public async ValueTask DisposeAsync()
    {
        DockerComposePlan plan = CreateDockerComposePlan(this.Port);
        await _dockerCompose.DownAsync(this, plan, null, default);
    }

    public void Report(string value)
    {
        _logger.LogInformation(value);
    }

    internal async Task StartAsync(CancellationToken cancellationToken)
    {
        int port = _portGenerator.GetNextPort();
        Port = _device.AppiumPort = port;

        DockerComposePlan plan = CreateDockerComposePlan(port);

        await _dockerCompose.UpAsync(this, plan, null, cancellationToken);
    }

    public static string GetProjectName(string deviceId) => $"tb-appium-{deviceId}";

    private DockerComposePlan CreateDockerComposePlan(int port)
    {
        if (string.IsNullOrEmpty(_device.Url))
        {
            throw new ArgumentNullException("_device.Url must be set");
        }

        // As we are running appium on the same host as the device, use host.docker.internal to access the host machine
        var url = _device.Url;
        var portSeparator = url.IndexOf(':');
        if(portSeparator > 0)
        {
            var portString = url.Substring(portSeparator + 1);
            if(int.TryParse(portString, out int parsedPort))
            {
                var hostnameFromContainer = Environment.GetEnvironmentVariable("TB_APPIUM_HOSTNAME_FOR_ADBPROXY") ?? "host.docker.internal:";
                url = hostnameFromContainer + parsedPort;
            }
            else
            {
                _logger.LogWarning("Failed to parse port from device URL '{Url}'", url);
            }
        }


        var containerName = GetProjectName(_device.DeviceId);
        var projectName = containerName;

        var yml = $"""
            name: {containerName}
            services:
              tb-appium:
                container_name: {containerName}
                restart: unless-stopped
                image: appium/appium
                ports:
                  - {port}:4723
                environment:
                  - REMOTE_ADB=true
                  - ANDROID_DEVICES={url}
                  - REMOTE_ADB_POLLING_SEC=60
            """;

        var plan = DockerComposeParser.ParseYaml(yml);
        plan.Yaml = yml;

        // need to set com.docker.compose.project manually as we parse yaml and not loading a file (where it will be assigned from the folder name)
        plan.Labels = new Dictionary<string, string>
        {
            { "com.docker.compose.project", projectName },
            { "testbucket.adbproxy.appium", "true" },
            { "testbucket.adbproxy.deviceid", _device.DeviceId }
        };
        return plan;
    }
}
