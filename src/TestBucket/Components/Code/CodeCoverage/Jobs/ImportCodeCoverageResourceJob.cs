
using Quartz;

namespace TestBucket.Components.Code.CodeCoverage.Jobs;

public class ImportCodeCoverageResourceJob : IJob
{
    public Task Execute(IJobExecutionContext context)
    {
        var tenantId = context.MergedJobDataMap.GetString("TenantId");
        var resourceId = context.MergedJobDataMap.GetLongValue("ResourceId");
        return Task.CompletedTask;
    }
}
