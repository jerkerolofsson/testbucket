
using Microsoft.Extensions.Logging;

using Quartz;

namespace TestBucket.Domain.Code.CodeCoverage.Import;

[DisallowConcurrentExecution]
public class ImportCodeCoverageResourceJob : IJob
{
    private readonly CodeCoverageImporter _codeCoverageImporter;
    private readonly ILogger<ImportCodeCoverageResourceJob> _logger;

    public ImportCodeCoverageResourceJob(CodeCoverageImporter codeCoverageImporter, ILogger<ImportCodeCoverageResourceJob> logger)
    {
        _codeCoverageImporter = codeCoverageImporter;
        _logger = logger;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        var tenantId = context.MergedJobDataMap.GetString("TenantId");
        if(tenantId is null)
        {
            _logger.LogError("Error, no tenant ID specified in job");
            return;
        }
        var email = context.MergedJobDataMap.GetString("Email");
        if (email is null)
        {
            _logger.LogError("Error, no use remail specified in job");
            return;
        }

        var resourceId = context.MergedJobDataMap.GetLongValue("ResourceId");
        await _codeCoverageImporter.ImportAsync(email, tenantId, resourceId, context.CancellationToken);
    }
}
