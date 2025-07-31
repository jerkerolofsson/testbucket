
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

namespace TestBucket.ResourceServer.Playwright.Services;

public class PlaywrightDockerService : BackgroundService, IProgress<string>
{
    private readonly ILogger<PlaywrightDockerService> _logger;
    private readonly IResourceInformer _informer;
    private readonly ILocalContainersClient _docker;
    private readonly IDockerCompose _compose;
    private readonly IServer _server;
    private int _port = 33323;
    private const string _containerName = "tb-playwright";

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
            await EnsurePlaywrightInDockerIsRunningAsync(stoppingToken);
            await InformAsync(stoppingToken);

            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
        }
    }

    private async Task InformAsync(CancellationToken cancellationToken)
    {
        var hostname = PublicHostnameDetector.GetPublicHostname() ?? "localhost";
        List<TestResourceDto> resources = [];
        resources.Add(new TestResourceDto
        {
            Name = $"playwright-mcp-server@{hostname}",
            Owner = ResourceServerOwner.Name,
            ResourceId = hostname + "-playwright-mcp-server",
            Model = "Playwright",
            Manufacturer = "Microsoft",
            Types = ["playwright-mcp", "mcp-server"],
            Health = HealthStatus.Healthy,
            Variables =
            {
                ["url"] = $"http://{hostname}:{_port}/mcp"
            }
        });

        await _informer.InformAsync(resources.ToArray(), cancellationToken);
    }

    private async Task EnsurePlaywrightInDockerIsRunningAsync(CancellationToken cancellationToken)
    {
        var container = await _docker.GetContainerByNameAsync(_containerName, cancellationToken);

        if(container is null)
        {
            _logger.LogInformation("container not found, creating..");
            string portNumber = Environment.GetEnvironmentVariable("TB_PLAYWRIGHT_PORT") ?? "33323";
            if(!int.TryParse(portNumber, out var port))
            {
                _port = port;
            }
            var plan = CreateDockerComposePlan(_port);
            await _compose.UpAsync(this, plan, null, cancellationToken);
        }
        else if(!container.IsRunning)
        {
            _logger.LogInformation("Container found but is not running. Starting!");
            await _docker.StartAsync(container.Id, cancellationToken);
        }
    }

    private DockerComposePlan CreateDockerComposePlan(int port)
    {

        var yml = $"""
            name: {_containerName}
            services:
              tb-playwright:
                container_name: {_containerName}
                image: mcr.microsoft.com/playwright/mcp
                entrypoint: ["node", "cli.js", "--headless", "--browser", "chromium", "--no-sandbox", "--port", "4723"]
                restart: unless-stopped
                ports:
                  - {port}:4723
            """;

        var plan = DockerComposeParser.ParseYaml(yml);
        plan.Yaml = yml;

        // need to set com.docker.compose.project manually as we parse yaml and not loading a file (where it will be assigned from the folder name)
        plan.Labels = new Dictionary<string, string>
        {
            { "com.docker.compose.project", _containerName }
        };
        return plan;
    }

    public void Report(string value)
    {
        _logger.LogDebug(value);
    }
}
