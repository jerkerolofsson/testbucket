
using Microsoft.Extensions.Logging;

using Quartz;

using TestBucket.Domain.Jobs;

namespace TestBucket.Domain.Code.CodeCoverage.Import;

[DisallowConcurrentExecution]
public class ImportCodeCoverageResourceJob : UserJob
{
    private readonly CodeCoverageImporter _codeCoverageImporter;
    private readonly ILogger<ImportCodeCoverageResourceJob> _logger;

    public ImportCodeCoverageResourceJob(CodeCoverageImporter codeCoverageImporter, ILogger<ImportCodeCoverageResourceJob> logger)
    {
        _codeCoverageImporter = codeCoverageImporter;
        _logger = logger;
    }

    public override async Task Execute(IJobExecutionContext context)
    {
        var user = GetUser(context);
        var userName = user.Identity?.Name ?? throw new UnauthorizedAccessException("No identity name");
        var tenantId = user.GetTenantIdOrThrow();
        var resourceId = context.MergedJobDataMap.GetLongValue("ResourceId");
        await _codeCoverageImporter.ImportAsync(userName, tenantId, resourceId, context.CancellationToken);
    }
}
