using System.Security.Claims;

using Microsoft.Extensions.Caching.Memory;

namespace TestBucket.Domain.Identity.Permissions
{
    public class UserPermissionsManager : IUserPermissionsManager
    {
        private readonly IPermissionsRepository _repository;
        private readonly IMemoryCache _cache;
        private readonly IUserService _userService;

        public UserPermissionsManager(IPermissionsRepository repository, IMemoryCache cache, IUserService userService)
        {
            _repository = repository;
            _cache = cache;
            _userService = userService;
        }

        #region Permissions

        /// <summary>
        /// Returns global permissions based on current user's role
        /// </summary>
        /// <param name="principal"></param>
        /// <returns></returns>
        public async Task<RolePermission[]> GetTenantRolePermissionsForCurrentUserAsync(ClaimsPrincipal principal)
        {
            var tenantId = principal.GetTenantIdOrThrow();

            if (principal.IsInRole(Roles.SUPERADMIN))
            {
                var role = Roles.SUPERADMIN;
                return await _repository.GetTenantRolePermissionsAsync(tenantId, role);
            }
            else if (principal.IsInRole(Roles.ADMIN))
            {
                var role = Roles.ADMIN;
                return await _repository.GetTenantRolePermissionsAsync(tenantId, role);
            }
            else if (principal.IsInRole(Roles.REGULAR_USER))
            {
                var role = Roles.REGULAR_USER;
                return await _repository.GetTenantRolePermissionsAsync(tenantId, role);
            }
            else if (principal.IsInRole(Roles.READ_ONLY))
            {
                var role = Roles.READ_ONLY;
                return await _repository.GetTenantRolePermissionsAsync(tenantId, role);
            }

            return [];
        }

        public async Task<RolePermission[]> GetTenantRolePermissionsAsync(ClaimsPrincipal principal)
        {
            if(principal.Identity?.IsAuthenticated == false)
            {
                return [];
            }

            var tenantId = principal.GetTenantIdOrThrow();
            string cacheKey = $"{tenantId}{principal.Identity?.Name}";

            var result = await _cache.GetOrCreateAsync(cacheKey, async (e) =>
            {
                // We cache for a short time here so that changed permissions apply in a reasonable time, and this 
                // method is called when loading the page. Once the session is established it will not be called again
                // until the user reloads the page
                e.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(30);
                return await _repository.GetTenantRolePermissionsAsync(tenantId);
            });
            return result ?? [];
        }
        public async Task AddTenantRolePermissionAsync(ClaimsPrincipal principal, RolePermission rolePermission)
        {
            var tenantId = principal.GetTenantIdOrThrow();

            rolePermission.Created = DateTimeOffset.UtcNow;
            rolePermission.Modified = DateTimeOffset.UtcNow;
            rolePermission.CreatedBy = principal.Identity?.Name ?? throw new Exception("No identity in current principal");
            rolePermission.ModifiedBy = principal.Identity?.Name ?? throw new Exception("No identity in current principal");
            rolePermission.TenantId = principal.GetTenantIdOrThrow();

            await _repository.AddTenantRolePermissionAsync(rolePermission);
        }

        public async Task UpdateTenantRolePermissionAsync(ClaimsPrincipal principal, RolePermission rolePermission)
        {
            principal.ThrowIfEntityTenantIsDifferent(rolePermission);
            rolePermission.Modified = DateTimeOffset.UtcNow;
            rolePermission.ModifiedBy = principal.Identity?.Name ?? throw new Exception("No identity in current principal");

            await _repository.UpdateTenantRolePermissionAsync(rolePermission);
        }
        /// <summary>
        /// Returns permissions specific to a project and user
        /// </summary>
        /// <param name="principal"></param>
        /// <returns></returns>
        public async Task<ProjectUserPermission[]> GetProjectUserPermissionsAsync(ClaimsPrincipal principal, long projectId)
        {
            var tenantId = principal.GetTenantIdOrThrow();
            
            var user = await _userService.FindAsync(principal, tenantId);
            if(user is null)
            {
                return [];
            }
            return await _repository.GetProjectUserPermissionsAsync(tenantId, user.Id, projectId);
        }

        public async Task UpdateProjectUserPermissionAsync(ClaimsPrincipal principal, ProjectUserPermission userPermission)
        {
            principal.ThrowIfEntityTenantIsDifferent(userPermission);
            userPermission.Modified = DateTimeOffset.UtcNow;
            userPermission.ModifiedBy = principal.Identity?.Name ?? throw new Exception("No identity in current principal");

            await _repository.UpdateProjectUserPermissionAsync(userPermission);
        }

        public async Task AddProjectUserPermissionAsync(ClaimsPrincipal principal, ProjectUserPermission userPermission)
        {
            var tenantId = principal.GetTenantIdOrThrow();

            userPermission.Created = DateTimeOffset.UtcNow;
            userPermission.CreatedBy = principal.Identity?.Name ?? throw new Exception("No identity in current principal");
            userPermission.Modified = DateTimeOffset.UtcNow;
            userPermission.ModifiedBy = principal.Identity?.Name ?? throw new Exception("No identity in current principal");
            userPermission.TenantId = principal.GetTenantIdOrThrow();

            await _repository.AddProjectUserPermissionAsync(userPermission);
        }
        #endregion Permissions
    }
}
