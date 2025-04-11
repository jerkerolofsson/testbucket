using MudBlazor;
using OllamaSharp.Models.Chat;

using TestBucket.Components.Account.Pages.Manage;
using TestBucket.Components.Projects.Dialogs;
using TestBucket.Components.Users.Dialogs;
using TestBucket.Domain.Identity;
using TestBucket.Domain.Shared;

namespace TestBucket.Components.Users.Services;

internal class UserController : TenantBaseService
{
    private readonly IUserManager _userManager;
    private readonly IUserService _userService;
    private readonly IDialogService _dialogService;

    public UserController
        (AuthenticationStateProvider authenticationStateProvider, 
        IUserManager userManager,
        IUserService userService,
        IDialogService dialogService) : 
        base(authenticationStateProvider)
    {
        _userManager = userManager;
        _userService = userService;
        _dialogService = dialogService;
    }

    public async Task<string[]> SearchUserNamesAsync(string query, int offset = 0, int count = 25)
    {
        ClaimsPrincipal principal = await GetUserClaimsPrincipalAsync();
        var tenantId = principal.GetTenantIdOrThrow();
        var result = await _userService.SearchUserNamesAsync(tenantId, new SearchQuery { Text = query, Offset = offset, Count = count }, default);
        return result.Items;
    }

    public async Task AddUserAsync()
    {
        var dialog = await _dialogService.ShowAsync<AddUserDialog>(null);
        var result = await dialog.Result;
    }

    public async Task<IdentityResult> CreateUserAsync(string email, string password)
    {
        ClaimsPrincipal principal = await GetUserClaimsPrincipalAsync();
        return await _userManager.AddUserAsync(principal, email, password);
    }

    public async Task<ApplicationUser?> GetUserByNormalizedUserNameAsync(string normalizedUserName)
    {
        ClaimsPrincipal principal = await GetUserClaimsPrincipalAsync();
        return await _userManager.GetUserByNormalizedUserNameAsync(principal, normalizedUserName);
    }
    public async Task<PagedResult<ApplicationUser>> BrowseAsync(int offset, int count)
    {
        ClaimsPrincipal principal = await GetUserClaimsPrincipalAsync();
        return await _userManager.BrowseAsync(principal, offset, count);
    }
}
