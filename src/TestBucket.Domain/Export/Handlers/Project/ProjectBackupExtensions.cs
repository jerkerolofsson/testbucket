using TestBucket.Domain.Code.Models;
using TestBucket.Domain.Export.Models;
using TestBucket.Domain.Issues.Models;
using TestBucket.Domain.Labels.Models;
using TestBucket.Domain.Requirements.Models;
using TestBucket.Domain.Testing.Models;

namespace TestBucket.Domain.Export.Handlers.Project;
public static class ProjectBackupExtensions
{
    public static async Task<Stream> CreateBackupAsync(this IBackupManager manager, ClaimsPrincipal principal, TestProject project)
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
                if (entity is TestCase testCase && testCase.TestProjectId == project.Id)
                {
                    return true;
                }
            }
            if (principal.HasPermission(PermissionEntityType.TestSuite, PermissionLevel.Read))
            {
                if (entity is TestSuite entitySpec && entitySpec.TestProjectId == project.Id)
                {
                    return true;
                }
            }
            if (principal.HasPermission(PermissionEntityType.RequirementSpecification, PermissionLevel.Read))
            {
                if (entity is RequirementSpecification entitySpec && entitySpec.TestProjectId == project.Id)
                {
                    return true;
                }
            }
            if (principal.HasPermission(PermissionEntityType.Requirement, PermissionLevel.Read))
            {
                if (entity is Requirement entitySpec && entitySpec.TestProjectId == project.Id)
                {
                    return true;
                }
            }
            if (principal.HasPermission(PermissionEntityType.Architecture, PermissionLevel.Read))
            {
                if (entity is Component component && component.TestProjectId == project.Id)
                {
                    return true;
                }
                if (entity is Feature feature && feature.TestProjectId == project.Id)
                {
                    return true;
                }
                if (entity is ArchitecturalLayer layer && layer.TestProjectId == project.Id)
                {
                    return true;
                }
                if (entity is ProductSystem productSystem && productSystem.TestProjectId == project.Id)
                {
                    return true;
                }
            }
            if (principal.HasPermission(PermissionEntityType.Issue, PermissionLevel.Read))
            {
                if (entity is Label label && label.TestProjectId == project.Id)
                {
                    return true;
                }
                if (entity is LocalIssue entitySpec && entitySpec.TestProjectId == project.Id)
                {
                    return true;
                }
                if (entity is Milestone milestone && milestone.TestProjectId == project.Id)
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
