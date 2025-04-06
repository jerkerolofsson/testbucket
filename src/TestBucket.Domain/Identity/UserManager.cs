using System.Security.Claims;

using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

using TestBucket.Contracts.Identity;
using TestBucket.Domain.Identity.Models;
using TestBucket.Domain.Tenants.Models;

namespace TestBucket.Domain.Identity;
internal class UserManager : IUserManager
{
    private readonly ISuperAdminUserService _superAdminUserService;
    private readonly IUserService _userService;
    private readonly ISettingsProvider _settingsProvider;

    public UserManager(ISuperAdminUserService superAdminUserService, IUserService userService, ISettingsProvider settingsProvider)
    {
        _superAdminUserService = superAdminUserService;
        _userService = userService;
        _settingsProvider = settingsProvider;
    }

    public async Task<IdentityResult> AddUserAsync(ClaimsPrincipal principal, string email, string password)
    {
        // Access guard for protected API
        principal.ThrowIfNotAdmin();

        var tenantId = principal.GetTenantIdOrThrow();
        return await _superAdminUserService.RegisterAndConfirmUserAsync(tenantId, email, password);
    }

    public async Task<PagedResult<ApplicationUser>> BrowseAsync(ClaimsPrincipal principal, int offset, int count)
    {
        // Access guard for protected API
        var tenantId = principal.GetTenantIdOrThrow();
        return await _superAdminUserService.BrowseAsync(tenantId, offset, count);
    }

    public async Task AddApiKeyAsync(ClaimsPrincipal principal, ApplicationUserApiKey apiKey)
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
}
