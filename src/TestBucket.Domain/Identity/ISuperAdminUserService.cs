using System.Security.Claims;

using Microsoft.AspNetCore.Identity;

using TestBucket.Domain.Identity.Models;

namespace TestBucket.Domain.Identity;

public interface ISuperAdminUserService
{
    Task AssignRoleAsync(string tenantId, string email, string roleName);
    Task UnassignRoleAsync(string tenantId, string email, string roleName);
    Task AddRoleAsync(string tenantId, string roleName);
    Task RemoveRoleAsync(string tenantId, string roleName);
    Task<IReadOnlyList<string>> GetRoleNamesAsync(string tenantId);

    Task<PagedResult<ApplicationUser>> BrowseAsync(string tenantId, int offset, int count);
    Task<IdentityResult> RegisterAndConfirmUserAsync(string tenantId, string email, string password);
    Task UpdateUserAsync(string tenantId, ApplicationUser user);
    Task<IReadOnlyList<string>> GetUserRoleNamesAsync(ApplicationUser user);
    Task DeleteUserAsync(ApplicationUser user);
}