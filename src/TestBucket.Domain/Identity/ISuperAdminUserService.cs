using Microsoft.AspNetCore.Identity;

using TestBucket.Domain.Identity.Models;

namespace TestBucket.Domain.Identity;

public interface ISuperAdminUserService
{
    Task AssignRoleAsync(string tenantId, string email, string roleName);
    Task<PagedResult<ApplicationUser>> BrowseAsync(string tenantId, int offset, int count);
    Task<IdentityResult> RegisterAndConfirmUserAsync(string tenantId, string email, string password);
}