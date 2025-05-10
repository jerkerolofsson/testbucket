using TestBucket.Domain.Identity.Permissions;
using TestBucket.Domain.Shared;

namespace TestBucket.Components.Settings.Roles;

internal class RolesController : TenantBaseService
{
    private readonly IUserPermissionsManager _userPermissionsManager;

    public RolesController(IUserPermissionsManager userPermissionsManager, AuthenticationStateProvider authenticationStateProvider) 
        :base(authenticationStateProvider)
    {
        _userPermissionsManager = userPermissionsManager;
    }

    /// <summary>
    /// Adds a permission for a role
    /// </summary>
    /// <param name="permission"></param>
    /// <returns></returns>
    public async Task AddAsync(RolePermission permission)
    {
        ClaimsPrincipal principal = await GetUserClaimsPrincipalAsync();

        if (permission.Role == TestBucket.Domain.Identity.Roles.SUPERADMIN ||
            permission.Role == TestBucket.Domain.Identity.Roles.ADMIN)
        {
            principal.ThrowIfNotSuperAdmin();
        }
        principal.ThrowIfNotAdmin();

        await _userPermissionsManager.AddTenantRolePermissionAsync(principal, permission);
    }

    /// <summary>
    /// Updates a permission for a role
    /// </summary>
    /// <param name="permission"></param>
    /// <returns></returns>
    public async Task UpdateAsync(RolePermission permission)
    {
        ClaimsPrincipal principal = await GetUserClaimsPrincipalAsync();

        if(permission.Role == TestBucket.Domain.Identity.Roles.SUPERADMIN || 
            permission.Role == TestBucket.Domain.Identity.Roles.ADMIN)
        {
            principal.ThrowIfNotSuperAdmin();
        }
        principal.ThrowIfNotAdmin();

        await _userPermissionsManager.UpdateTenantRolePermissionAsync(principal, permission);
    }

    /// <summary>
    /// Returns all permissions
    /// </summary>
    /// <returns></returns>
    public async Task<RolePermission[]> GetTenantRolePermissionsAsync()
    {
        ClaimsPrincipal principal = await GetUserClaimsPrincipalAsync();
        return await _userPermissionsManager.GetTenantRolePermissionsAsync(principal);
    }
}
