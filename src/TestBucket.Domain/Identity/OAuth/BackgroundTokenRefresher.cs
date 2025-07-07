using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using TestBucket.Contracts.Integrations;
using TestBucket.Domain.Projects;
using TestBucket.Domain.Projects.Mapping;
using TestBucket.Domain.Requirements;
using TestBucket.Domain.Requirements.Models;
using TestBucket.Domain.Requirements.RequirementExtensions;
using TestBucket.Domain.Tenants;

namespace TestBucket.Domain.Identity.OAuth;

/// <summary>
/// Performs token refresh for integrations where there is a valid refresh token and an expiry timestamp set
/// </summary>
public class BackgroundTokenRefresher : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<BackgroundTokenRefresher> _logger;

    public BackgroundTokenRefresher(IServiceProvider serviceProvider, ILogger<BackgroundTokenRefresher> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }


    private ClaimsPrincipal Impersonate(string? tenantId)
    {
        return Impersonation.Impersonate(tenantId);
    }
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Task.Delay(TimeSpan.FromSeconds(120), stoppingToken);

        while (!stoppingToken.IsCancellationRequested)
        {
            using var scope = _serviceProvider.CreateScope();
            var tenantRepository = scope.ServiceProvider.GetRequiredService<ITenantRepository>();
            var projectManager = scope.ServiceProvider.GetRequiredService<IProjectManager>();

            try
            {
                await foreach (var tenant in tenantRepository.EnumerateAsync(stoppingToken))
                {
                    await ProcessTenantAsync(scope, projectManager, tenant, stoppingToken);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing tenant");
            }
            await Task.Delay(TimeSpan.FromMinutes(10), stoppingToken);
        }
    }

    private async Task ProcessTenantAsync(IServiceScope scope, IProjectManager projectManager, Tenant tenant, CancellationToken stoppingToken)
    {
        ClaimsPrincipal principal = Impersonate(tenant.Id);

        // Loop over all projects
        var response = await projectManager.BrowseTestProjectsAsync(principal, 0, 100);
        foreach (var project in response.Items)
        {
            await ProcessProjectAsync(scope, projectManager, principal, project, stoppingToken);
        }
    }

    private async Task ProcessProjectAsync(IServiceScope scope, IProjectManager projectManager, ClaimsPrincipal principal, TestProject project, CancellationToken stoppingToken)
    {
        var configs = await projectManager.GetProjectIntegrationsAsync(principal, project.Id);
        foreach (var config in configs.Where(x => x.RefreshToken is not null && x.TokenExpiry is not null))
        {
            if(config.ClientId is null || config.ClientSecret is null || config.RefreshToken is null || config.TokenEndpoint is null)
            {
                continue;
            }

            var expiresIn = config.TokenExpiry!.Value-DateTimeOffset.UtcNow;   
            if(expiresIn > TimeSpan.FromMinutes(20))
            {
                continue;
            }

            // Refresh the token now
            var oauthManager = scope.ServiceProvider.GetRequiredService<OAuthAuthManager>();

            var state = new OAuthAuthState(principal, config.ClientId, config.ClientSecret, config.AuthEndpoint??"", config.TokenEndpoint, config.Name, async (s) =>
                {
                    // Update the project configuration with the new tokens
                    config.AccessToken = s.AccessToken;
                    config.RefreshToken = s.RefreshToken;
                    config.TokenExpiry = DateTimeOffset.UtcNow.AddSeconds(s.ExpiresIn);
                    await projectManager.SaveProjectIntegrationAsync(principal, project.Slug, config);
                });
            state.RefreshToken = config.RefreshToken;

            try
            {
                await oauthManager.RefreshTokenAsync(state);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Failed to refresh token for project {ProjectName} ({ProjectId}) with service {ServiceName}", 
                    project.Name, project.Id, config.Name);
            }
        }
    }
}
