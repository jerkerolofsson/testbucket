using Mediator;

using Quartz;

using TestBucket.CodeCoverage;
using TestBucket.Domain.Code.CodeCoverage.Models;
using TestBucket.Domain.Files;
using TestBucket.Domain.Files.IntegrationEvents;

namespace TestBucket.Domain.Code.CodeCoverage.Import;

internal class AutoImportCodeCoverageWhenFileAdded : INotificationHandler<FileResourceAddedNotification>
{
    private readonly ISchedulerFactory _scheduler;
    private readonly ISettingsProvider _settingsProvider;

    public AutoImportCodeCoverageWhenFileAdded(ISchedulerFactory scheduler, ISettingsProvider settingsProvider)
    {
        _scheduler = scheduler;
        _settingsProvider = settingsProvider;
    }

    public async ValueTask Handle(FileResourceAddedNotification notification, CancellationToken cancellationToken)
    {
        var file = notification.Resource;
        var principal = notification.Principal;

        var settings = await _settingsProvider.GetDomainSettingsAsync<CodeCoverageSettings>(file.TenantId, file.TestProjectId);
        if(settings is null || settings.AutomaticallyImportTestRunCodeCoverageReports == false)
        {
            return;
        }

        if (file.TestRunId is not null && principal.HasPermission(file, PermissionLevel.Read))
        {
            if(CodeCoverageMediaTypes.IsCodeCoverageFile(file.ContentType))
            {
                var scheduler = await _scheduler.GetScheduler();
                var jobData = new JobDataMap
                {
                    { "ResourceId", file.Id.ToString() },
                    { "TenantId", file.TenantId },
                    { "Email", principal.Identity?.Name ?? "system" }
                };
                await scheduler.TriggerJob(new JobKey(nameof(ImportCodeCoverageResourceJob)), jobData);
            }
        }
    }
}
