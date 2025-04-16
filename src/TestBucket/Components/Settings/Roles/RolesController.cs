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

    public async Task<RolePermission[]> GetTenantRolePermissionsAsync()
    {
        ClaimsPrincipal principal = await GetUserClaimsPrincipalAsync();
        return await _userPermissionsManager.GetTenantRolePermissionsAsync(principal);
    }
}
