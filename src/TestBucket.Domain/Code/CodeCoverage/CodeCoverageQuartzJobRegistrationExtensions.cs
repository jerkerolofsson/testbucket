using Quartz;

using TestBucket.Domain.Code.CodeCoverage.Import;

namespace TestBucket.Domain.Code.CodeCoverage;

public static class CodeCoverageQuartzJobRegistrationExtensions
{
    public static void AddCodeCoverageJobs(this IServiceCollectionQuartzConfigurator q)
    {
        q.AddJob<ImportCodeCoverageResourceJob>(j => j
           .StoreDurably()
           .WithIdentity(nameof(ImportCodeCoverageResourceJob))
           .WithDescription("Imports code coverage resources (files)")
       );

    }
}
