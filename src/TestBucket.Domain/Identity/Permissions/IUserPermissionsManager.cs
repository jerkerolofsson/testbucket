using System.Security.Claims;

namespace TestBucket.Domain.Identity.Permissions;

/// <summary>
/// Manages permission levels
/// 
/// Role permissions: Bound to a tenant. Applies to all users as a foundation.
/// ProjectUserPermissions: Bound to a project/user combination. Overrides role based permissions.
/// 
/// </summary>
public interface IUserPermissionsManager
{
    Task AddProjectUserPermissionAsync(ClaimsPrincipal principal, ProjectUserPermission userPermission);
    Task AddTenantRolePermissionAsync(ClaimsPrincipal principal, RolePermission rolePermission);
    Task<ProjectUserPermission[]> GetProjectUserPermissionsAsync(ClaimsPrincipal principal, long projectId);
    Task<RolePermission[]> GetTenantRolePermissionsAsync(ClaimsPrincipal principal);
    Task<RolePermission[]> GetTenantRolePermissionsForCurrentUserAsync(ClaimsPrincipal principal);
    Task UpdateProjectUserPermissionAsync(ClaimsPrincipal principal, ProjectUserPermission userPermission);
    Task UpdateTenantRolePermissionAsync(ClaimsPrincipal principal, RolePermission rolePermission);
}