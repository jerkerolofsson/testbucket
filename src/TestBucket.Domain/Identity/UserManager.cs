using System.Data;
using System.Security.Claims;

using Microsoft.AspNetCore.Identity;

using TestBucket.Contracts.Identity;
using TestBucket.Domain.Identity.Models;

namespace TestBucket.Domain.Identity;
internal class UserManager : IUserManager
{
    private readonly ISuperAdminUserService _superAdminUserService;
    private readonly IUserService _userService;
    private readonly ISettingsProvider _settingsProvider;

    private ApplicationUser? _self;

    public UserManager(ISuperAdminUserService superAdminUserService, IUserService userService, ISettingsProvider settingsProvider)
    {
        _superAdminUserService = superAdminUserService;
        _userService = userService;
        _settingsProvider = settingsProvider;
    }

    public async Task<ApplicationUser> GetSelfAsync(ClaimsPrincipal principal)
    {
        if (_self is null)
        {
            _self = await FindAsync(principal);
        }
        return _self ?? throw new ArgumentException("Cannot find self - is principal a valid user?");
    }

    public async Task<IdentityResult> AddUserAsync(ClaimsPrincipal principal, string email, string password)
    {
        // Access guard for protected API
        principal.ThrowIfNotAdmin();

        var tenantId = principal.GetTenantIdOrThrow();
        return await _superAdminUserService.RegisterAndConfirmUserAsync(tenantId, email, password);
    }

    public async Task UpdateUserAsync(ClaimsPrincipal principal, ApplicationUser user)
    {
        var tenantId = principal.GetTenantIdOrThrow();

        // Reset cached self in case we are updating the profile image or similar..
        _self = null;

        // Access guard for protected API
        if (user.TenantId != tenantId)
        {
            throw new UnauthorizedAccessException("Users cannot update user of another tenant");
        }

        if (principal.Identity?.Name != user.UserName)
        {
            // Users can update themselves, but if they are not admin they cannot update other users
            principal.ThrowIfNotAdmin();
        }

        await _superAdminUserService.UpdateUserAsync(tenantId, user);
    }

    public async Task<PagedResult<ApplicationUser>> BrowseAsync(ClaimsPrincipal principal, int offset, int count)
    {
        // Access guard for protected API
        var tenantId = principal.GetTenantIdOrThrow();
        return await _superAdminUserService.BrowseAsync(tenantId, offset, count);
    }

    public async Task AddApiKeyAsync(string scope, ClaimsPrincipal principal, ApplicationUserApiKey apiKey)
    {
        var settings = await _settingsProvider.LoadGlobalSettingsAsync();
        if (string.IsNullOrEmpty(settings.SymmetricJwtKey))
        {
            throw new InvalidDataException("No symmetric key has been configured");
        }
        if (string.IsNullOrEmpty(settings.JwtIssuer))
        {
            throw new InvalidDataException("No issuer has been configured");
        }
        if (string.IsNullOrEmpty(settings.JwtAudience))
        {
            throw new InvalidDataException("No audience has been configured");
        }

        var user = await FindAsync(principal) ?? throw new InvalidOperationException("User not found"); ;
        var tenantId = principal.GetTenantIdOrThrow();

        // Generate the API key
        var generator = new ApiKeyGenerator(settings.SymmetricJwtKey, settings.JwtIssuer, settings.JwtAudience);
        apiKey.Key = generator.GenerateAccessToken(scope, principal, apiKey.Expiry.DateTime);

        apiKey.TenantId = tenantId;
        apiKey.CreatedBy = principal.Identity?.Name ?? throw new Exception("Missing identity");
        apiKey.Created = DateTimeOffset.UtcNow;
        apiKey.ApplicationUserId = user.Id;
        await _userService.AddApiKeyAsync(apiKey);
    }
    public async Task AddPersonalApiKeyAsync(ClaimsPrincipal principal, ApplicationUserApiKey apiKey)
    {
        var settings = await _settingsProvider.LoadGlobalSettingsAsync();
        if (string.IsNullOrEmpty(settings.SymmetricJwtKey))
        {
            throw new InvalidDataException("No symmetric key has been configured");
        }
        if (string.IsNullOrEmpty(settings.JwtIssuer))
        {
            throw new InvalidDataException("No issuer has been configured");
        }
        if (string.IsNullOrEmpty(settings.JwtAudience))
        {
            throw new InvalidDataException("No audience has been configured");
        }

        var user = await FindAsync(principal) ?? throw new InvalidOperationException("User not found"); ;
        var tenantId = principal.GetTenantIdOrThrow();

        // Generate the API key
        var generator = new ApiKeyGenerator(settings.SymmetricJwtKey, settings.JwtIssuer, settings.JwtAudience);
        apiKey.Key = generator.GenerateAccessToken(principal, apiKey.Expiry.DateTime);

        apiKey.TenantId = tenantId;
        apiKey.CreatedBy = principal.Identity?.Name ?? throw new Exception("Missing identity");
        apiKey.Created = DateTimeOffset.UtcNow;
        apiKey.ApplicationUserId = user.Id;
        await _userService.AddApiKeyAsync(apiKey);
    }

    public async Task DeleteApiKeyAsync(ClaimsPrincipal principal, ApplicationUserApiKey apiKey)
    {
        var user = await FindAsync(principal) ?? throw new InvalidOperationException("User not found"); ;

        var tenantId = principal.GetTenantIdOrThrow();
        await _userService.DeleteApiKeyAsync(user.Id, tenantId, apiKey.Id);
    }

    public async Task<ApplicationUser?> GetUserByNormalizedUserNameAsync(ClaimsPrincipal principal, string normalizedUserName)
    {
        var tenantId = principal.GetTenantIdOrThrow();
        return await _userService.FindByNormalizedEmailAsync(principal, normalizedUserName);
    }
    public async Task<ApplicationUser?> FindAsync(ClaimsPrincipal principal)
    {
        var tenantId = principal.GetTenantIdOrThrow();
        return await _userService.FindAsync(principal, tenantId);
    }

    public async Task<ApplicationUserApiKey[]> GetApiKeysAsync(ClaimsPrincipal principal)
    {
        var tenantId = principal.GetTenantIdOrThrow();
        var user = await FindAsync(principal) ?? throw new InvalidOperationException("User not found"); ;
        return await _userService.GetApiKeysAsync(user.Id, tenantId);
    }

    public async Task AssignRoleAsync(ClaimsPrincipal principal, string normalizedEmail, string role)
    {
        principal.ThrowIfNotAdmin();
        await _superAdminUserService.AssignRoleAsync(principal.GetTenantIdOrThrow(), normalizedEmail, role);
    }
    public async Task UnassignRoleAsync(ClaimsPrincipal principal, string normalizedEmail, string role)
    {
        principal.ThrowIfNotAdmin();
        await _superAdminUserService.UnassignRoleAsync(principal.GetTenantIdOrThrow(), normalizedEmail, role);
    }

    public async Task AddRoleAsync(ClaimsPrincipal principal, string role)
    {
        principal.ThrowIfNotAdmin();
        await _superAdminUserService.AddRoleAsync(principal.GetTenantIdOrThrow(), role);
    }
    public async Task RemoveRoleAsync(ClaimsPrincipal principal, string role)
    {
        principal.ThrowIfNotAdmin();
        await _superAdminUserService.RemoveRoleAsync(principal.GetTenantIdOrThrow(), role);
    }

    public async Task<IReadOnlyList<string>> GetRoleNamesAsync(ClaimsPrincipal principal)
    {
        principal.ThrowIfNotAdmin();
        return await _superAdminUserService.GetRoleNamesAsync(principal.GetTenantIdOrThrow());
    }

    public async Task<IReadOnlyList<string>> GetUserRoleNamesAsync(ClaimsPrincipal principal,string normalizedUserName)
    {
        principal.ThrowIfNotAdmin();
        var user = await GetUserByNormalizedUserNameAsync(principal, normalizedUserName);
        if(user is null)
        {
            return [];
        }
        return await _superAdminUserService.GetUserRoleNamesAsync(user);
    }

}
