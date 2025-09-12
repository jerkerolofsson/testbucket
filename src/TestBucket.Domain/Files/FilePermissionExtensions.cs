using TestBucket.Domain.Files.Models;

namespace TestBucket.Domain.Files;

public static class FilePermissionExtensions
{
    public static void ThrowIfNoPermission(this ClaimsPrincipal principal, FileResource file)
    {
        var level = PermissionLevel.Read;
        ThrowIfNoPermission(principal, file, level);
    }

    public static bool HasPermission(this ClaimsPrincipal principal, FileResource file, PermissionLevel level)
    {
        var tenantId = principal.GetTenantIdOrThrow();
        if(tenantId != file.TenantId)
        {
            return false;
        }

        if (file.TestCaseId is not null)
        {
            if(!principal.HasPermission(PermissionEntityType.TestCase, level))
            {
                return false;
            }
        }
        if (file.TestRunId is not null)
        {
            if(!principal.HasPermission(PermissionEntityType.TestRun, level))
            {
                return false;
            }
        }
        if (file.TestCaseRunId is not null)
        {
            if(!principal.HasPermission(PermissionEntityType.TestCaseRun, level))
            {
                return false;
            }
        }
        if (file.TestSuiteId is not null || file.TestSuiteFolderId is not null)
        {
            if(!principal.HasPermission(PermissionEntityType.TestSuite, level))
            {
                return false;
            }
        }
        if (file.LocalIssueId is not null)
        {
            if(!principal.HasPermission(PermissionEntityType.Issue, level))
            {
                return false;
            }
        }
        if (file.RequirementId is not null)
        {
            if(!principal.HasPermission(PermissionEntityType.Requirement, level))
            {
                return false;
            }
        }
        if (file.RequirementSpecificationId is not null)
        {
            if(!principal.HasPermission(PermissionEntityType.RequirementSpecification, level))
            {
                return false;
            }
        }
        if (file.FeatureId is not null || file.ComponentId is not null || file.SystemId is not null || file.LayerId is not null)
        {
            if(!principal.HasPermission(PermissionEntityType.Architecture, level))
            {
                return false;
            }
        }
        if (file.TestProjectId is not null)
        {
            if(!principal.HasPermission(PermissionEntityType.Project, level))
            {
                return false;
            }
        }

        return true;
    }

    public static void ThrowIfNoPermission(this ClaimsPrincipal principal, FileResource file, PermissionLevel level)
    {
        if(!HasPermission(principal, file, level))
        {
            throw new UnauthorizedAccessException($"The user {principal.Identity?.Name} does not have the required access ({level}) for the file resource {file.Name} ({file.Id})");
        }
    }
}
