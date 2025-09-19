using Quartz;

namespace TestBucket.Domain.Fields.Jobs;

public static class FieldJobExtensions
{
    public static void AddFieldJobs(this IServiceCollectionQuartzConfigurator q)
    {
        q.AddJob<FieldChangedJob>(j => j
           .StoreDurably()
           .WithIdentity(nameof(FieldChangedJob))
           .WithDescription("Generates mediator events for modified fields")
       );

    }
}
