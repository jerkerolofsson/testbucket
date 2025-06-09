using MudBlazor;

using TestBucket.Components.Settings.ApiKeys.Dialogs;
using TestBucket.Contracts.Identity;
using TestBucket.Domain.Identity;
using TestBucket.Domain.Shared;

namespace TestBucket.Components.Settings.ApiKeys;

internal class UserApiKeysController : TenantBaseService
{
    private readonly IDialogService _dialogService;
    private readonly IUserManager _userManager;
    private readonly ILogger<UserApiKeysController> _logger;

    public UserApiKeysController(
        AuthenticationStateProvider authenticationStateProvider,
        IUserManager userManager,
        ILogger<UserApiKeysController> logger,
        IDialogService dialogService) : base(authenticationStateProvider)
    {
        _userManager = userManager;
        _logger = logger;
        _dialogService = dialogService;
    }

    /// <summary>
    /// Shows a dialog that lets the user add an API key
    /// </summary>
    /// <returns></returns>
    public async Task<ApplicationUserApiKey?> AddApiKeyAsync(long? testProjectId, string? name = null)
    {
        name ??= "New access token " + DateTimeOffset.UtcNow.Date.ToShortDateString();
        var parameters = new DialogParameters<AddApiKeyDialog>
        {
            { x => x.TestProjectId, testProjectId },
            { x => x.Name, name }
        };
        var dialog = await _dialogService.ShowAsync<AddApiKeyDialog>(null, parameters, DefaultBehaviors.DialogOptions);
        var result = await dialog.Result;
        return result?.Data as ApplicationUserApiKey;
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
    /// <returns></returns>
    public async Task<ApplicationUserApiKey> AddApiKeyAsync(string name, string tokenType, long? testProjectId, DateTimeOffset expiry)
    {
        var principal = await GetUserClaimsPrincipalAsync();

        ApplicationUser? user = await _userManager.FindAsync(principal);

        var apiKey = new ApplicationUserApiKey()
        {
            Created = DateTimeOffset.UtcNow,
            TenantId = principal.GetTenantIdOrThrow(),
            Expiry = expiry.ToUniversalTime(),
            Name = name,
            Key = "",
        };

        switch(tokenType)
        {
            case "project":
                if (testProjectId is null)
                {
                    throw new InvalidOperationException("ProjectId must not be null");
                }
                await _userManager.AddProjectApiKeyAsync(principal, testProjectId.Value, apiKey);
                break;
            case "personal":
               
                await _userManager.AddPersonalApiKeyAsync(principal, apiKey);
                break;
            default:
                await _userManager.AddApiKeyAsync(tokenType, principal, apiKey);
                break;
        }
        return apiKey;
    }

}
