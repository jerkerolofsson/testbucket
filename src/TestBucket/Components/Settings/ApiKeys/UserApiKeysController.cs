using TestBucket.Components.Users;
using TestBucket.Domain.Identity;
using TestBucket.Domain.Shared;

namespace TestBucket.Components.Settings.ApiKeys;

internal class UserApiKeysController : TenantBaseService
{
    private readonly IUserManager _userManager;
    private readonly ILogger<UserApiKeysController> _logger;

    public UserApiKeysController(
        AuthenticationStateProvider authenticationStateProvider,
        IUserManager userManager,
        ILogger<UserApiKeysController> logger) : base(authenticationStateProvider)
    {
        _userManager = userManager;
        _logger = logger;
    }

    public Task AddApiKeyAsync()
    {
        return Task.CompletedTask;
    }

    public async Task<ApplicationUserApiKey[]> GetApiKeysAsync()
    {
        var principal = await GetUserClaimsPrincipalAsync();
        return await _userManager.GetApiKeysAsync(principal);
    }

    /// <summary>
    /// Deletes an API key
    /// </summary>
    /// <param name="apiKey"></param>
    /// <returns></returns>
    public async Task DeleteApiKeyAsync(ApplicationUserApiKey apiKey)
    {
        var principal = await GetUserClaimsPrincipalAsync();
        principal.ThrowIfEntityTenantIsDifferent(apiKey);
        await _userManager.DeleteApiKeyAsync(principal, apiKey);
    }

    /// <summary>
    /// Adds an API key
    /// </summary>
    /// <param name="apiKey"></param>
    /// <returns></returns>
    public async Task<ApplicationUserApiKey> AddApiKeyAsync(string name, DateTimeOffset expiry)
    {
        var principal = await GetUserClaimsPrincipalAsync();

        ApplicationUser? user = await _userManager.FindAsync(principal);

        var apiKey = new ApplicationUserApiKey()
        {
            Created = DateTimeOffset.UtcNow,
            TenantId = principal.GetTenantIdOrThrow(),
            Expiry = expiry.ToUniversalTime(),
            Name = name,
            Key = "123456"
        };

        await _userManager.AddApiKeyAsync(principal, apiKey);
        return apiKey;
    }

}
