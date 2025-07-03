using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using TestBucket.Contracts.Integrations;
using TestBucket.Domain.Code.Mapping;
using TestBucket.Domain.Code.Models;
using TestBucket.Domain.ExtensionManagement;
using TestBucket.Domain.Identity;
using TestBucket.Domain.Projects;
using TestBucket.Domain.Projects.Mapping;
using TestBucket.Domain.Projects.Models;
using TestBucket.Domain.Teams.Models;
using TestBucket.Domain.Tenants;

namespace TestBucket.Domain.Code.Services;

/// <summary>
/// Indexes commits in all repos
/// </summary>
public class CodeRepoCommmitBackgroundIndexer : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<CodeRepoCommmitBackgroundIndexer> _logger;
    public CodeRepoCommmitBackgroundIndexer(IServiceProvider serviceProvider, ILogger<CodeRepoCommmitBackgroundIndexer> logger)
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
                await foreach(var tenant in tenantRepository.EnumerateAsync(stoppingToken))
                {
                    await ProcessTenantAsync(scope, tenant, stoppingToken);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing tenant");
            }
            await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
        }
    }

    private async Task ProcessTenantAsync(IServiceScope scope, Tenant tenant, CancellationToken cancellationToken)
    {
        var principal = Impersonation.Impersonate(configure =>
        {
            configure.TenantId = tenant.Id;
            configure.UserName = "commit-bot";
            configure.Email = "admin@admin.com";
            configure.AddAllPermissions();
        });

        var projectRepository = scope.ServiceProvider.GetRequiredService<IProjectRepository>();
        await foreach (var project in projectRepository.EnumerateAsync(tenant.Id, cancellationToken))
        { 
            await ProcessProjectAsync(principal, scope, project, cancellationToken);
        }
    }

    private async Task ProcessProjectAsync(ClaimsPrincipal principal, IServiceScope scope, TestProject project, CancellationToken cancellationToken)
    {
        if (project.ExternalSystems is not null)
        {
            var integrations = scope.ServiceProvider.GetRequiredService<IEnumerable<IExternalCodeRepository>>();

            foreach (var externalSystem in project.ExternalSystems.Where(x => (x.EnabledCapabilities & ExternalSystemCapability.ReadCodeRepository) == ExternalSystemCapability.ReadCodeRepository))
            {
                var externalSystemId = externalSystem.Id;
                var integration = integrations.Where(x => x.SystemName == externalSystem.Name).FirstOrDefault();
                if (integration is not null)
                {
                    try
                    {
                        var repo = await GetOrCreateRepositoryAsync(principal, scope, project, externalSystem, integration, cancellationToken);
                        if (repo is not null)
                        {
                            await IndexRepositoryAsync(principal, scope, project, integration, repo, externalSystem, cancellationToken);
                        }
                        else
                        {
                            _logger.LogError("Failed to read repository from Name={ExternalSystemName} Project={ExternalProjectId}", externalSystem.Name, externalSystem.ExternalProjectId);

                            // Todo: Logging so it can be shown in UI?
                        }
                    }
                    catch(Exception ex)
                    {
                        _logger.LogError(ex, "Failed to read index commits from Name={ExternalSystemName} Project={ExternalProjectId}", externalSystem.Name, externalSystem.ExternalProjectId);

                        // Todo: Logging so it can be shown in UI?
                    }
                }
            }
        }
    }

    private async Task IndexRepositoryAsync(ClaimsPrincipal principal, IServiceScope scope, TestProject project, IExternalCodeRepository integration, Repository repo, ExternalSystem externalSystem, CancellationToken cancellationToken)
    {
        DateTimeOffset until = DateTimeOffset.UtcNow;
        var manager = scope.ServiceProvider.GetRequiredService<ICommitManager>();

        var from = repo.LastIndexTimestamp ?? new DateTimeOffset(2025, 1, 1, 0, 0, 0, 0, TimeSpan.Zero);

        // Debug:
        from = from.AddMinutes(-30);

        while(from < until)
        {
            var untilPage = from.AddDays(7);
            if(untilPage > until)
            {
                untilPage = until;
            }

            // Index week
            var commits = await integration.GetCommitsAsync(externalSystem.ToDto(), null, from, untilPage, cancellationToken);
            foreach (var commit in commits)
            {
                cancellationToken.ThrowIfCancellationRequested();

                if (commit.Files is null)
                {
                    var commitWithFiles = await integration.GetCommitAsync(externalSystem.ToDto(), commit.Ref, cancellationToken);
                    var commitDbo = commitWithFiles.ToDbo(repo);
                    commitDbo.TestProjectId = project.Id;
                    commitDbo.TeamId = project.TeamId;

                    await manager.AddCommitAsync(principal, commitDbo);
                }
                else
                {
                    var commitDbo = commit.ToDbo(repo);
                    commitDbo.TestProjectId = project.Id;
                    commitDbo.TeamId = project.TeamId;
                    await manager.AddCommitAsync(principal, commitDbo);
                }
            }

            // Update timestamp for indexing
            repo.LastIndexTimestamp = until;
            await manager.UpdateRepositoryAsync(principal, repo);

            from = untilPage;
        }

    }

    /// <summary>
    /// Gets a repository from the local database or requests one from the integration and creates one in the local DB
    /// </summary>
    /// <param name="scope"></param>
    /// <param name="project"></param>
    /// <param name="externalSystem"></param>
    /// <param name="integration"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    private async Task<Repository?> GetOrCreateRepositoryAsync(
        ClaimsPrincipal principal,
        IServiceScope scope, 
        TestProject project, 
        ExternalSystem externalSystem, 
        IExternalCodeRepository integration, 
        CancellationToken cancellationToken)
    {
        var manager = scope.ServiceProvider.GetRequiredService<ICommitManager>();

        Repository? repo = await manager.GetRepoByExternalSystemAsync(principal, externalSystem.Id);
        if (repo is null)
        {
            var repoDto = await integration.GetRepositoryAsync(externalSystem.ToDto(), cancellationToken);
            if (repoDto is null)
            {
                return null;
            }
            repo = new Repository
            {
                Url = repoDto.Url,
                ExternalId = repoDto.ExternalId,
                TestProjectId = project.Id,
                TeamId = project.TeamId,
                TenantId = project.TenantId,
                ExternalSystemId = externalSystem.Id
            };
            await manager.AddRepositoryAsync(principal, repo);
        }
        return repo;
    }
}
