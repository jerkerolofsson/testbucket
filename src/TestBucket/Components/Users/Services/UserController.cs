using System.Data;

using MudBlazor;
using OllamaSharp.Models.Chat;

using TestBucket.Components.Account.Pages.Manage;
using TestBucket.Components.Projects.Dialogs;
using TestBucket.Components.Users.Dialogs;
using TestBucket.Domain.Identity;
using TestBucket.Domain.Shared;
using TestBucket.Domain.Tenants.Models;

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
    public async Task AssignRoleAsync(ApplicationUser user, string role)
    {
        if (user.NormalizedEmail is null)
        {
            throw new ArgumentNullException(nameof(user.NormalizedEmail));
        }

        ClaimsPrincipal principal = await GetUserClaimsPrincipalAsync();
        await _userManager.AssignRoleAsync(principal, user.NormalizedEmail, role);
    }
    public async Task UnassignRoleAsync(ApplicationUser user, string role)
    {
        if (user.NormalizedEmail is null)
        {
            throw new ArgumentNullException(nameof(user.NormalizedEmail));
        }

        ClaimsPrincipal principal = await GetUserClaimsPrincipalAsync();
        await _userManager.UnassignRoleAsync(principal, user.NormalizedEmail, role);
    }

    /// <summary>
    /// Adds a role
    /// </summary>
    /// <param name="principal"></param>
    /// <param name="role"></param>
    /// <returns></returns>
    public async Task AddRoleAsync(ClaimsPrincipal principal, string role)
    {
        principal.ThrowIfNotAdmin();
        await _userManager.AddRoleAsync(principal, role);

    }
    public async Task<IReadOnlyList<string>> GetRoleNamesAsync()
    {
        ClaimsPrincipal principal = await GetUserClaimsPrincipalAsync();
        return await _userManager.GetRoleNamesAsync(principal);
    }
    public async Task<IReadOnlyList<string>> GetUserRoleNamesAsync(string normalizedUserName)
    {
        ClaimsPrincipal principal = await GetUserClaimsPrincipalAsync();
        return await _userManager.GetUserRoleNamesAsync(principal, normalizedUserName);
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
