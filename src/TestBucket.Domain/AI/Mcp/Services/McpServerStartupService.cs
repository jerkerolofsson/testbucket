using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using TestBucket.Domain.Identity;
using TestBucket.Domain.Projects;
using TestBucket.Domain.Tenants;

namespace TestBucket.Domain.AI.Mcp.Services;

/// <summary>
/// A background server that starts MCP servers when starting
/// </summary>
internal class McpServerStartupService : BackgroundService
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly ILogger<McpServerStartupService> _logger;

    public McpServerStartupService(IServiceScopeFactory serviceProvider, ILogger<McpServerStartupService> logger)
    {
        _serviceScopeFactory = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var tenantRepository = scope.ServiceProvider.GetRequiredService<ITenantRepository>();

            try
            {
                await foreach (var tenant in tenantRepository.EnumerateAsync(stoppingToken))
                {
                    await ProcessTenantAsync(scope, tenant, stoppingToken);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing tenant");
            }
        }
        catch(Exception ex) 
        { 
            _logger.LogError(ex, "Error starting MCP servers");
        }
    }

    private async Task ProcessTenantAsync(IServiceScope scope, Tenant tenant, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var principal = Impersonation.Impersonate(configure =>
        {
            configure.TenantId = tenant.Id;
            configure.UserName = "mcp-registered-servers-starter-bot";
            configure.Email = "admin@admin.com";
            configure.AddAllPermissions();
        });

        var projectRepository = scope.ServiceProvider.GetRequiredService<IProjectRepository>();
        await foreach (var project in projectRepository.EnumerateAsync(tenant.Id, cancellationToken))
        {
            await ProcessProjectAsync(scope, tenant.Id, project.Id, cancellationToken);
        }
    }

    private async Task ProcessProjectAsync(IServiceScope scope, string tenantId, long projectId, CancellationToken cancellationToken)
    {
        var serverManager = scope.ServiceProvider.GetRequiredService<IMcpServerManager>();
        var serverRunnerManager = scope.ServiceProvider.GetRequiredService<McpServerRunnerManager>();
        foreach (var registration in await serverManager.GetAllMcpServerRegistationsAsync(Impersonation.Impersonate(tenantId, projectId), projectId))
        {
            cancellationToken.ThrowIfCancellationRequested();
            await serverRunnerManager.StartServerAsync(registration, cancellationToken);
        }

    }
}
