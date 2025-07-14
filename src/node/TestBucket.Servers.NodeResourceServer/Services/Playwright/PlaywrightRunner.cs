
using TestBucket.ResourceServer.Utilities;
using TestBucket.Servers.NodeResourceServer.Models;
using TestBucket.Servers.NodeResourceServer.Services.Inform;

namespace TestBucket.Servers.NodeResourceServer.Services.Playwright;

public class PlaywrightRunner : BackgroundService
{
    private readonly IServiceInformer _informer;

    public PlaywrightRunner(IServiceInformer informer)
    {
        _informer = informer;
    }

    private async Task InformAsync(List<NodeService> services, CancellationToken cancellationToken)
    {
        await _informer.InformAsync(services.Select(x=>x.Description).ToArray(), cancellationToken);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if(Npx.GetIsInstalled() == false)
        {
            return;
        }

        List<NodeService> services = [];
        List<Task> tasks = [];
        var instances  = Environment.GetEnvironmentVariable("TB_PLAYWRIGHT_INSTANCES");
        var owner = Environment.GetEnvironmentVariable("SERVER_UUID") ?? "no-uuid-configured";
        var hostname = PublicHostnameDetector.GetPublicHostname();

        if (instances is not null && int.TryParse(instances, out var count) && count > 0)
        {
            for (int i = 0; i < count; i++)
            {
                int port = 8930 + i;

                // Todo: as we launch npx, there may be child processes running, so should we kill them??

                services.Add(new NodeService
                {
                    Port = port,
                    Description = new Contracts.TestResources.TestResourceDto
                    {
                        Name = "Playwright MCP server @ " + port,
                        Owner = owner,
                        ResourceId = $"playwright-{port}-" + hostname,
                        Types = ["playwright-mcp"],
                        Manufacturer = "Microsoft",
                        Model = "playwright-mcp",
                        Health = Microsoft.Extensions.Diagnostics.HealthChecks.HealthStatus.Healthy,
                        Variables = new Dictionary<string, string> {
                            { "url", $"http://{hostname}:{port}/mcp" }
                        },
                    },
                });

                var task = PlaywrightService.RunAsync(port, stoppingToken);
                tasks.Add(task);
            }
        }

        while(!stoppingToken.IsCancellationRequested && tasks.Count > 0)
        {
            // Check the state and update..
            for (int i = 0; i < tasks.Count; i++)
            {
                if (!tasks[i].IsCompleted)
                {
                    services[i].Description.Health = Microsoft.Extensions.Diagnostics.HealthChecks.HealthStatus.Healthy;
                }
                else
                {
                    services[i].Description.Health = Microsoft.Extensions.Diagnostics.HealthChecks.HealthStatus.Unhealthy;
                }
            }
            await InformAsync(services, stoppingToken);

            await Task.WhenAny(tasks);

            // Restart any completed tasks
            for (int i = 0; i < tasks.Count; i++)
            {
                if (tasks[i].IsCompleted)
                {
                    services[i].Description.Health = Microsoft.Extensions.Diagnostics.HealthChecks.HealthStatus.Unhealthy;
                    await InformAsync(services, stoppingToken);

                    int port = services[i].Port;
                    var task = PlaywrightService.RunAsync(port, stoppingToken);
                    tasks[i] = task;
                }
            }
            await Task.Delay(5000);
        }
    }
}
