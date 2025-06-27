using TestBucket.Domain.Export.Models;
using TestBucket.Domain.Requirements.Models;

namespace TestBucket.Domain.Export.Handlers.Requirements;
public static class BackupExtensions
{
    public static async Task<Stream> CreateBackupAsync(this IBackupManager manager, ClaimsPrincipal principal, RequirementSpecification specification)
    {
        var destination = new MemoryStream();
        var options = new ExportOptions();
        options.DestinationType = ExportDestinationType.Stream;
        options.Destination = null;
        options.DestinationStream = destination;

        options.Filter = (entity) =>
        {
            if (entity is Requirement requirement && requirement.RequirementSpecificationId == specification.Id)
            {
                return true;
            }
            if (entity is RequirementSpecification entitySpec && entitySpec.Id == specification.Id)
            {
                return true;
            }

            return false;
        };

        await manager.CreateBackupAsync(principal, options);

        destination.Seek(0, SeekOrigin.Begin);
        return destination;
    }
}
