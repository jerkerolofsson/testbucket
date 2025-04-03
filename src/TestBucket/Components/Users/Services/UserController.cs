using MudBlazor;
using OllamaSharp.Models.Chat;

using TestBucket.Components.Account.Pages.Manage;
using TestBucket.Components.Projects.Dialogs;
using TestBucket.Components.Users.Dialogs;
using TestBucket.Domain.Identity;

namespace TestBucket.Components.Users.Services;

internal class UserController : TenantBaseService
{
    private readonly IUserManager _userManager;
    private readonly IDialogService _dialogService;

    public UserController
        (AuthenticationStateProvider authenticationStateProvider, 
        IUserManager userManager, 
        IDialogService dialogService) : 
        base(authenticationStateProvider)
    {
        _userManager = userManager;
        _dialogService = dialogService;
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
