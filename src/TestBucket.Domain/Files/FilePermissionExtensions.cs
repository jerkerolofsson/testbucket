using TestBucket.Domain.Files.Models;

namespace TestBucket.Domain.Files;

public static class FilePermissionExtensions
{
    public static void ThrowIfNoPermission(this ClaimsPrincipal principal, FileResource file)
    {
        var level = PermissionLevel.Read;
        principal.ThrowIfEntityTenantIsDifferent(file.TenantId);

        if (file.TestCaseId is not null)
        {
            principal.ThrowIfNoPermission(PermissionEntityType.TestCase, level);
        }
        if (file.TestRunId is not null)
        {
            principal.ThrowIfNoPermission(PermissionEntityType.TestRun, level);
        }
        if (file.TestCaseRunId is not null)
        {
            principal.ThrowIfNoPermission(PermissionEntityType.TestCaseRun, level);
        }
        if (file.TestSuiteId is not null || file.TestSuiteFolderId is not null)
        {
            principal.ThrowIfNoPermission(PermissionEntityType.TestSuite, level);
        }

        if (file.LocalIssueId is not null)
        {
            principal.ThrowIfNoPermission(PermissionEntityType.Issue, level);
        }

        if (file.RequirementId is not null)
        {
            principal.ThrowIfNoPermission(PermissionEntityType.Requirement, level);
        }
        if (file.RequirementSpecificationId is not null)
        {
            principal.ThrowIfNoPermission(PermissionEntityType.RequirementSpecification, level);
        }
        if (file.FeatureId is not null || file.ComponentId is not null || file.SystemId is not null || file.LayerId is not null)
        {
            principal.ThrowIfNoPermission(PermissionEntityType.Architecture, level);
        }
        
        if (file.TestProjectId is not null)
        {
            principal.ThrowIfNoPermission(PermissionEntityType.Project, level);
        }
    }
}
