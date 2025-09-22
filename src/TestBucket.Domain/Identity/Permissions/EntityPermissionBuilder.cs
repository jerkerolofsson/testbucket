using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Domain.Identity.Permissions.Models;

namespace TestBucket.Domain.Identity.Permissions
{
    /// <summary>
    /// Helper that will merge permission levels from multiple roles
    /// </summary>
    public class EntityPermissionBuilder
    {
        private readonly UserPermissions _userPermissions = new();

        public string UserName { get; set; } = "system";
        public string Email { get; set; } = "admin@admin.com";

        /// <summary>
        /// Tenant
        /// </summary>
        public string? TenantId { get; set; }

        /// <summary>
        /// project
        /// </summary>
        public long? ProjectId { get; set; }

        public UserPermissions Build() => _userPermissions;


        public EntityPermissionBuilder AddAllPermissions()
        {
            Add(PermissionEntityType.TestSuite, PermissionLevel.All);
            Add(PermissionEntityType.TestCase, PermissionLevel.All);
            Add(PermissionEntityType.TestCaseRun, PermissionLevel.All);
            Add(PermissionEntityType.TestRun, PermissionLevel.All);
            Add(PermissionEntityType.TestAccount, PermissionLevel.All);
            Add(PermissionEntityType.TestResource, PermissionLevel.All);
            Add(PermissionEntityType.Project, PermissionLevel.All);
            Add(PermissionEntityType.User, PermissionLevel.All);
            Add(PermissionEntityType.Runner, PermissionLevel.All);
            Add(PermissionEntityType.Team, PermissionLevel.All);
            Add(PermissionEntityType.Tenant, PermissionLevel.All);
            Add(PermissionEntityType.Requirement, PermissionLevel.All);
            Add(PermissionEntityType.RequirementSpecification, PermissionLevel.All);
            Add(PermissionEntityType.Architecture, PermissionLevel.All);
            Add(PermissionEntityType.Issue, PermissionLevel.All);
            Add(PermissionEntityType.Heuristic, PermissionLevel.All);
            Add(PermissionEntityType.Dashboard, PermissionLevel.All);
            Add(PermissionEntityType.McpServer, PermissionLevel.All);
            return this;
        }
        public EntityPermissionBuilder Add(PermissionEntityType entityType, PermissionLevel level)
        {
            var permission = _userPermissions.Permisssions.Where(x => x.EntityType == entityType).FirstOrDefault();
            if (permission is null)
            {
                permission = new EntityPermission(entityType, PermissionLevel.None);
            }
            else
            {
                _userPermissions.Permisssions.Remove(permission);
            }
            _userPermissions.Permisssions.Add(new EntityPermission(entityType, permission.Level | level));

            return this;
        }

        public EntityPermissionBuilder Add(RolePermission rolePermission)
        {
            var permission = _userPermissions.Permisssions.Where(x => x.EntityType == rolePermission.Entity).FirstOrDefault();
            if(permission is null)
            {
                permission = new EntityPermission(rolePermission.Entity, PermissionLevel.None);
            }
            else
            {
                _userPermissions.Permisssions.Remove(permission);
            }
            _userPermissions.Permisssions.Add(new EntityPermission(rolePermission.Entity, permission.Level | rolePermission.Level));

            return this;
        }
    }
}
