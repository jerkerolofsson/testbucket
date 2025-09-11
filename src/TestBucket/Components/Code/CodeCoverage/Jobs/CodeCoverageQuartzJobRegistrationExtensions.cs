using Quartz;

namespace TestBucket.Components.Code.CodeCoverage.Jobs;

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
