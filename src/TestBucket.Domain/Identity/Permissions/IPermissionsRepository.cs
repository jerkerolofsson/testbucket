using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace TestBucket.Domain.Identity.Permissions
{
    public interface IPermissionsRepository
    {
        Task AddProjectUserPermissionAsync(ProjectUserPermission userPermission);
        Task AddTenantRolePermissionAsync(RolePermission rolePermission);

        /// <summary>
        /// Returns permissions specific to a user and a proejct
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="userId"></param>
        /// <param name="projectId"></param>
        /// <returns></returns>
        Task<ProjectUserPermission[]> GetProjectUserPermissionsAsync(string tenantId, long userId, long projectId);
        Task<RolePermission[]> GetTenantRolePermissionsAsync(string tenantId, string role);
        Task<RolePermission[]> GetTenantRolePermissionsAsync(string tenantId);
        Task UpdateProjectUserPermissionAsync(ProjectUserPermission userPermission);
        Task UpdateTenantRolePermissionAsync(RolePermission rolePermission);
    }
}
