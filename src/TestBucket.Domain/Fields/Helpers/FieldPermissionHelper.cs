using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Contracts.Fields;

namespace TestBucket.Domain.Fields.Helpers;
public static class FieldPermissionHelper
{
    public static void ThrowIfNoPermission(this ClaimsPrincipal principal, FieldDefinition? fieldDefinition)
    {
        if (!HasPermission(principal, fieldDefinition))
        {
            throw new UnauthorizedAccessException($"The user {principal.Identity?.Name} does not have the required access to edit this field");
        }
    }

    public static bool HasPermission(this ClaimsPrincipal principal, FieldDefinition? field)
    {
        if(field is null)
        {
            return false;
        }

        if(field.RequiredPermission == PermissionLevel.None)
        {
            return true;
        }

        foreach (FieldTarget target in Enum.GetValues(typeof(FieldTarget)))
        {
            if((field.Target & target) == target)
            {
                var permissionEntityType = GetPermissionEntityTypeFromField(target);
                if(!principal.HasPermission(permissionEntityType, field.RequiredPermission))
                {
                    return false;
                }
            }
        }
        return true;
    }

    public static PermissionEntityType GetPermissionEntityTypeFromField(FieldTarget target)
    {
        if (target == FieldTarget.TestSuite)
        {
            return PermissionEntityType.TestSuite;
        }
        if (target == FieldTarget.TestSuiteFolder)
        {
            return PermissionEntityType.TestSuite;
        }
        if (target == FieldTarget.TestCase)
        {
            return PermissionEntityType.TestCase;
        }

        if (target == FieldTarget.TestRun)
        {
            return PermissionEntityType.TestRun;
        }
        if (target == FieldTarget.TestCaseRun)
        {
            return PermissionEntityType.TestCaseRun;
        }

        if (target == FieldTarget.Requirement)
        {
            return PermissionEntityType.Requirement;
        }
        if (target == FieldTarget.RequirementSpecification)
        {
            return PermissionEntityType.RequirementSpecification;
        }
        if (target == FieldTarget.RequirementSpecificationFolder)
        {
            return PermissionEntityType.RequirementSpecification;
        }
        return PermissionEntityType.TestCase;
    }
}
