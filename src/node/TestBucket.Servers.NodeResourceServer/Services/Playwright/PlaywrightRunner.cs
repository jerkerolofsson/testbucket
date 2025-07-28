
using System.Diagnostics;

using TestBucket.ResourceServer.Utilities;
using TestBucket.Servers.NodeResourceServer.Models;
using TestBucket.Servers.NodeResourceServer.Services.Inform;

namespace TestBucket.Servers.NodeResourceServer.Services.Playwright;

public class PlaywrightRunner : BackgroundService
{
    private readonly IServiceInformer _informer;
    private readonly ILogger<PlaywrightRunner> _logger;
    public PlaywrightRunner(IServiceInformer informer, ILogger<PlaywrightRunner> logger)
    {
        _informer = informer;
        _logger = logger;
    }

    private async Task InformAsync(List<NodeService> services, CancellationToken cancellationToken)
    {
        try
        {
            await _informer.InformAsync(services.Select(x => x.Description).ToArray(), cancellationToken);
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, "Error informing about Playwright services");
        }
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (Npx.GetIsInstalled() == false)
        {
            return;
        }

        try
        {
            await RunAsync(stoppingToken);
        }
        catch (OperationCanceledException) { }
        finally
        {

        }
    }

    private async Task RunAsync(CancellationToken stoppingToken)
    {
        List<NodeService> services = [];
        List<Task> tasks = [];
        var instances = Environment.GetEnvironmentVariable("TB_PLAYWRIGHT_INSTANCES");
        var owner = Environment.GetEnvironmentVariable("SERVER_UUID") ?? "no-uuid-configured";
        var hostname = PublicHostnameDetector.GetPublicHostname();

        if (instances is not null && int.TryParse(instances, out var count) && count > 0)
        {
            for (int i = 0; i < count; i++)
            {
                int port = 8930 + i;
                services.Add(new NodeService
                {
                    Port = port,
                    Description = new Contracts.TestResources.TestResourceDto
                    {
                        Name = "Playwright MCP server @ " + port,
                        Owner = owner,
                        ResourceId = $"playwright-{port}-" + hostname,
                        Types = ["playwright-mcp", "mcp-server"],
                        Manufacturer = "Microsoft",
                        Model = "playwright_mcp",
                        Health = Microsoft.Extensions.Diagnostics.HealthChecks.HealthStatus.Healthy,
                        Variables = new Dictionary<string, string> {
                            { 
                                "url", $"http://{hostname}:{port}/mcp"
                            }
                        },
                    },
                });

                var task = PlaywrightService.RunAsync(port, _logger, stoppingToken);
                tasks.Add(task);
            }
        }

        while (!stoppingToken.IsCancellationRequested && tasks.Count > 0)
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

                    int port = services[i].Port;
                    Kill(port);
                    var task = PlaywrightService.RunAsync(port, _logger, stoppingToken);
                    tasks[i] = task;
                }
            }
            await InformAsync(services, stoppingToken);
            await Task.Delay(5000);
        }
    }

    /// <summary>
    /// This only works if running as admin
    /// </summary>
    /// <param name="port"></param>
    /// <returns></returns>
    private void Kill(int port)
    {
        foreach(var process in Process.GetProcesses())
        {
            if(process.ProcessName == "node" || process.ProcessName == "npx")
            {
                try
                {
                    var cmdLine = process.StartInfo.Arguments;
                    if (cmdLine.Contains($"--port {port}"))
                    {
                        _logger.LogInformation("Killing process {ProcessId} for port {Port}", process.Id, port);
                        process.Kill();
                    }
                }
                catch (InvalidOperationException)
                { 
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error killing process {ProcessId} for port {Port}", process.Id, port);
                }
            }
        }
    }
}
