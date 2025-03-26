using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Identity;

using TestBucket.Domain.Identity.Models;

namespace TestBucket.Domain.Identity;
internal class UserManager : IUserManager
{
    private readonly ISuperAdminUserService _superAdminUserService;

    public UserManager(ISuperAdminUserService superAdminUserService)
    {
        _superAdminUserService = superAdminUserService;
    }

    public async Task<IdentityResult> AddUserAsync(ClaimsPrincipal principal, string email, string password)
    {
        principal.ThrowIfNotAdmin();
        var tenantId = principal.GetTenantIdOrThrow();
        return await _superAdminUserService.RegisterAndConfirmUserAsync(tenantId, email, password);
    }

    public async Task<PagedResult<ApplicationUser>> BrowseAsync(ClaimsPrincipal principal, int offset, int count)
    {
        // Access guard for protected API
        principal.ThrowIfNotAdmin();
        var tenantId = principal.GetTenantIdOrThrow();

        return await _superAdminUserService.BrowseAsync(tenantId, offset, count);
    }
}
