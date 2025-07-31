using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using TestBucket.Domain.Features.Archiving.Models;
using TestBucket.Domain.Projects;
using TestBucket.Domain.Tenants;

namespace TestBucket.Domain.Features.Archiving;
internal class TestRunArchivingBackgroundService : BackgroundService
{
    private readonly ILogger<TestRunArchivingBackgroundService> _logger;
    private readonly IServiceProvider _serviceProvider;

    public TestRunArchivingBackgroundService(
        ILogger<TestRunArchivingBackgroundService> logger,
        IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(TimeSpan.FromSeconds(210), stoppingToken);

            using var scope = _serviceProvider.CreateScope();
            var settingsProvider = scope.ServiceProvider.GetRequiredService<ISettingsProvider>();
            var tenantRepo = scope.ServiceProvider.GetRequiredService<ITenantRepository>();
            var projectRepo = scope.ServiceProvider.GetRequiredService<IProjectRepository>();
            var testRepo = scope.ServiceProvider.GetRequiredService<ITestCaseRepository>();

            await foreach (var tenant in tenantRepo.EnumerateAsync(stoppingToken))
            {
                await foreach (var project in projectRepo.EnumerateAsync(tenant.Id, stoppingToken))
                {
                    var archivingSettings = await settingsProvider.GetDomainSettingsAsync<ArchiveSettings>(tenant.Id, project.Id);
                    await TestRunArchiver.ArchiveProjectRunsAsync(testRepo, archivingSettings, tenant.Id, project.Id);
                }
            }
        }
    }
}
