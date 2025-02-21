
namespace TestBucket.Data.Identity;

public interface ISuperAdminUserService
{
    Task AssignRoleAsync(string tenantId, string email, string roleName);
    Task<bool> RegisterAndConfirmUserAsync(string tenantId, string email, string password);
}