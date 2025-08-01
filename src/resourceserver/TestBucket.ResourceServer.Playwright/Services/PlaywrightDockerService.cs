
using System.ComponentModel;

using Docker.DotNet.Models;

using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.Extensions.Diagnostics.HealthChecks;

using RedMaple.Orchestrator.Contracts.Containers;
using RedMaple.Orchestrator.DockerCompose;
using RedMaple.Orchestrator.DockerCompose.Models;

using TestBucket.Contracts.TestResources;
using TestBucket.ResourceServer.Services.Inform;
using TestBucket.ResourceServer.Utilities;
using TestBucket.Traits.Core;

namespace TestBucket.ResourceServer.Playwright.Services;

public class PlaywrightDockerService : BackgroundService, IProgress<string>
{
    private readonly ILogger<PlaywrightDockerService> _logger;
    private readonly IResourceInformer _informer;
    private readonly ILocalContainersClient _docker;
    private readonly IDockerCompose _compose;
    private readonly IServer _server;
    //private int _port = 33323;
    private const string _imageName = "mcr.microsoft.com/playwright/mcp";
    private readonly Dictionary<string, int> _ports = [];
    private readonly string[] _browsers = ["chromium", "msedge", "firefox"];
    private const string _containerNamePrefix = "tb-playwright";

    public PlaywrightDockerService(IResourceInformer informer, ILocalContainersClient docker, IDockerCompose compose, ILogger<PlaywrightDockerService> logger, IServer server)
    {
        _informer = informer;
        _docker = docker;
        _compose = compose;
        _logger = logger;
        _server = server;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while(!stoppingToken.IsCancellationRequested)
        {
            List<TestResourceDto> resources = [];
            foreach (var browser in _browsers)
            {
                var containerId = await EnsurePlaywrightInDockerIsRunningAsync(browser, stoppingToken);
                AddResource(resources, containerId, browser);
            }

            await InformAsync(resources, stoppingToken);
            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
        }
    }

    private void AddResource(List<TestResourceDto> resources, string? containerId, string browser)
    {
        var hostname = PublicHostnameDetector.GetPublicHostname() ?? "localhost";
        if (_ports.TryGetValue(browser, out var port))
        {
            var name = $"playwright-mcp-{browser}@{hostname}";
            resources.Add(new TestResourceDto
            {
                Name = name,
                Owner = ResourceServerOwner.Name,
                ResourceId = name,
                Model = "Playwright",
                Manufacturer = "Microsoft",
                Types = ["playwright-mcp", "mcp-server"],
                Health = containerId is not null ? HealthStatus.Healthy : HealthStatus.Unhealthy,
                Variables =
                {
                    [TestTraitNames.Browser] = browser,
                    ["docker.image"] = _imageName,
                    ["docker.container.id"] = containerId??"",
                    ["url"] = $"http://{hostname}:{port}/mcp"
                }
            });
        }
    }
    private async Task InformAsync(IEnumerable<TestResourceDto> resources, CancellationToken cancellationToken)
    {
       
        await _informer.InformAsync(resources.ToArray(), cancellationToken);
    }

    private async Task<string?> EnsurePlaywrightInDockerIsRunningAsync(string browser, CancellationToken cancellationToken)
    {
        string containerName = _containerNamePrefix + "-" + browser;

        var container = await _docker.GetContainerByNameAsync(containerName, cancellationToken);
        string startPortName = Environment.GetEnvironmentVariable("TB_PLAYWRIGHT_PORT") ?? "33323";
        if (!_ports.TryGetValue(browser, out var port))
        {
            if (int.TryParse(startPortName, out var startPort))
            {
                _ports[browser] = port = startPort + _ports.Count;
            }
            else
            {
                _ports[browser] = port = 33323 + _ports.Count;
            }
        }

        if (container is null)
        {
            _logger.LogInformation("Container not found for {BrowserName}, creating..", browser);
            var plan = CreateDockerComposePlan(containerName, browser, port);
            await _compose.UpAsync(this, plan, null, cancellationToken);

            container = await _docker.GetContainerByNameAsync(containerName, cancellationToken);
        }
        else if(!container.IsRunning)
        {
            _logger.LogInformation("Container found but is not running. Starting!");
            await _docker.StartAsync(container.Id, cancellationToken);
        }

        if(container is not null)
        {
            if (container.IsRunning)
            {
                return container.Id;
            }
            return null;
        }

        return null;
    }

    private DockerComposePlan CreateDockerComposePlan(string containerName, string browser, int port)
    {

        var yml = $"""
            name: {containerName}
            services:
              tb-playwright:
                container_name: {containerName}
                image: {_imageName}
                entrypoint: ["node", "cli.js", "--headless", "--browser", {browser}, "--no-sandbox", "--port", "4723"]
                restart: unless-stopped
                ports:
                  - {port}:4723
            """;

        var plan = DockerComposeParser.ParseYaml(yml);
        plan.Yaml = yml;

        // need to set com.docker.compose.project manually as we parse yaml and not loading a file (where it will be assigned from the folder name)
        plan.Labels = new Dictionary<string, string>
        {
            { "com.docker.compose.project", containerName }
        };
        return plan;
    }

    public void Report(string value)
    {
        _logger.LogDebug(value);
    }
}
