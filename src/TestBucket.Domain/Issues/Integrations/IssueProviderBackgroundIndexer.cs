using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using TestBucket.Contracts.Integrations;
using TestBucket.Domain.Code.Mapping;
using TestBucket.Domain.Code.Models;
using TestBucket.Domain.Identity;
using TestBucket.Domain.Issues;
using TestBucket.Domain.Projects;
using TestBucket.Domain.Projects.Mapping;
using TestBucket.Domain.Projects.Models;
using TestBucket.Domain.Teams.Models;
using TestBucket.Domain.Tenants;
using TestBucket.Domain.Issues.Mapping;
using TestBucket.Domain.Issues.Search;
using TestBucket.Domain.Issues.Models;

namespace TestBucket.Domain.Code.Services;

/// <summary>
/// Indexes commits in all repos
/// </summary>
public class IssueProviderBackgroundIndexer : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<IssueProviderBackgroundIndexer> _logger;
    public IssueProviderBackgroundIndexer(IServiceProvider serviceProvider, ILogger<IssueProviderBackgroundIndexer> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Task.Delay(TimeSpan.FromSeconds(120), stoppingToken);

        while (!stoppingToken.IsCancellationRequested)
        {
            using var scope = _serviceProvider.CreateScope();
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
            await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
        }
    }

    private static async Task ProcessTenantAsync(IServiceScope scope, Tenant tenant, CancellationToken cancellationToken)
    {
        var principal = Impersonation.Impersonate(tenant.Id);

        var projectRepository = scope.ServiceProvider.GetRequiredService<IProjectRepository>();
        var projects = await projectRepository.SearchAsync(tenant.Id, new SearchQuery() { Offset = 0, Count = 100 });
        foreach (var project in projects.Items)
        {
            await ProcessProjectAsync(principal, scope, project, cancellationToken);
        }
    }

    private static async Task ProcessProjectAsync(ClaimsPrincipal principal, IServiceScope scope, TestProject project, CancellationToken cancellationToken)
    {
        if (project.ExternalSystems is not null)
        {
            var integrations = scope.ServiceProvider.GetRequiredService<IEnumerable<IExternalIssueProvider>>();

            foreach (var externalSystem in project.ExternalSystems.Where(x => (x.EnabledCapabilities & ExternalSystemCapability.GetIssues) == ExternalSystemCapability.GetIssues))
            {
                var externalSystemId = externalSystem.Id;
                var integration = integrations.Where(x => x.SystemName == externalSystem.Name).FirstOrDefault();
                if (integration is not null)
                {
                    await IndexIssuesAsync(principal, scope, project, integration, externalSystem, cancellationToken);
                }
            }
        }
    }

    private static async Task IndexIssuesAsync(ClaimsPrincipal principal, IServiceScope scope, TestProject project, IExternalIssueProvider integration, ExternalSystem externalSystem, CancellationToken cancellationToken)
    {
        DateTimeOffset? from = null;
        DateTimeOffset until = DateTimeOffset.UtcNow;
        var manager = scope.ServiceProvider.GetRequiredService<IIssueManager>();

        // We search for any issue since the last one from the same system
        var latest = await manager.SearchLocalIssuesAsync(principal, new SearchIssueQuery()
        {
            ProjectId = project.Id,
            ExternalSystemId = externalSystem.Id,
        }, 0, 1);
        if(latest.Items.Length > 0)
        {
            from = latest.Items[0].Created;
        }

        var issues = await integration.GetIssuesAsync(externalSystem.ToDto(), from, until, cancellationToken);
        foreach (var issue in issues)
        {
            cancellationToken.ThrowIfCancellationRequested();

            // Check if issue exists
            LocalIssue? existingIssue = await manager.FindLocalIssueFromExternalAsync(principal, project.Id, issue.ExternalSystemId, issue.ExternalId);
            if (existingIssue is null)
            {
                var dbo = issue.ToDbo();
                dbo.TestProjectId = project.Id;
                dbo.TeamId = project.TeamId;
                await manager.AddLocalIssueAsync(principal, dbo);
            }
            else
            {
                if(existingIssue.Modified >= issue.Modified)
                {
                    // Skip if the local issue is newer
                    continue;
                }
                IssueMapper.CopyTo(issue, existingIssue);
                await manager.UpdateLocalIssueAsync(principal, existingIssue);
            }
        }
    }
}
