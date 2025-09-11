
using Quartz;

namespace TestBucket.Domain.Code.CodeCoverage.Import;

public class ImportCodeCoverageResourceJob : IJob
{
    private readonly CodeCoverageImporter _codeCoverageImporter;

    public ImportCodeCoverageResourceJob(CodeCoverageImporter codeCoverageImporter)
    {
        _codeCoverageImporter = codeCoverageImporter;
    }

    public Task Execute(IJobExecutionContext context)
    {
        var tenantId = context.MergedJobDataMap.GetString("TenantId");
        var resourceId = context.MergedJobDataMap.GetLongValue("ResourceId");
        return Task.CompletedTask;
    }
}
