using TestBucket.Domain.Export.Models;
using TestBucket.Domain.Testing.Models;

namespace TestBucket.Domain.Export.Handlers.TestSuites;
public static class TestSuiteBackupExtensions
{
    public static async Task<Stream> CreateBackupAsync(this IBackupManager manager, ClaimsPrincipal principal, TestSuite testSuite)
    {
        var destination = new MemoryStream();
        var options = new ExportOptions();
        options.DestinationType = ExportDestinationType.Stream;
        options.Destination = null;
        options.DestinationStream = destination;

        options.Filter = (entity) =>
        {
            if (principal.HasPermission(PermissionEntityType.TestCase, PermissionLevel.Read))
            {
                if (entity is TestCase testCase && testCase.TestSuiteId == testSuite.Id)
                {
                    return true;
                }
            }
            if (principal.HasPermission(PermissionEntityType.TestSuite, PermissionLevel.Read))
            {
                if (entity is TestSuite entitySpec && entitySpec.Id == testSuite.Id)
                {
                    return true;
                }
            }

            return false;
        };

        await manager.CreateBackupAsync(principal, options);

        destination.Seek(0, SeekOrigin.Begin);
        return destination;
    }
}
