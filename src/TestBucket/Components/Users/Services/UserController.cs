using Microsoft.Extensions.Localization;

using TestBucket.Components.Users.Dialogs;
using TestBucket.Domain.Identity;
using TestBucket.Domain.Shared;
using TestBucket.Localization;

namespace TestBucket.Components.Users.Services;

internal class UserController : TenantBaseService
{
    private readonly IStringLocalizer<SharedStrings> _loc;
    private readonly IUserManager _userManager;
    private readonly IUserService _userService;
    private readonly IDialogService _dialogService;

    public UserController
        (AuthenticationStateProvider authenticationStateProvider, 
        IStringLocalizer<SharedStrings> loc,
        IUserManager userManager,
        IUserService userService,
        IDialogService dialogService) : 
        base(authenticationStateProvider)
    {
        _loc = loc;
        _userManager = userManager;
        _userService = userService;
        _dialogService = dialogService;
    }

    public async Task<string?> PickUserAsync()
    {
        var dialog = await _dialogService.ShowAsync<SelectUserDialog>(null, DefaultBehaviors.DialogOptions);
        var result = await dialog.Result;
        return result?.Data as string;
    }

    public async Task<bool> DeleteUserAsync(ApplicationUser user)
    {
        ClaimsPrincipal principal = await GetUserClaimsPrincipalAsync();
        if(!await ShowErrorIfNoPermissionAsync(_loc, _dialogService, PermissionEntityType.User, PermissionLevel.Delete))
        {
            return false;
        }

        var result = await _dialogService.ShowMessageBox(new MessageBoxOptions
        {
            YesText = _loc["yes"],
            NoText = _loc["no"],
            Title = _loc["confirm-delete-title"],
            MarkupMessage = new MarkupString(_loc["confirm-delete-message"])
        });
        if (result == true)
        {
            await _userManager.DeleteUserAsync(principal, user);
            return true;
        }
        return false;
    }

    public async Task<string[]> SearchUserNamesAsync(string query, int offset = 0, int count = 25)
    {
        ClaimsPrincipal principal = await GetUserClaimsPrincipalAsync();
        var tenantId = principal.GetTenantIdOrThrow();
        var result = await _userService.SearchUserNamesAsync(tenantId, new SearchQuery { Text = query, Offset = offset, Count = count }, default);
        return result.Items;
    }

    public async Task<PagedResult<string>> BrowseUserNamesAsync(string query, int offset = 0, int count = 25)
    {
        ClaimsPrincipal principal = await GetUserClaimsPrincipalAsync();
        var tenantId = principal.GetTenantIdOrThrow();
        var result = await _userService.SearchUserNamesAsync(tenantId, new SearchQuery { Text = query, Offset = offset, Count = count }, default);
        return result;
    }

    public async Task AddUserAsync()
    {
        if (!await ShowErrorIfNoPermissionAsync(_loc, _dialogService, PermissionEntityType.User, PermissionLevel.Write))
        {
            return;
        }
        var dialog = await _dialogService.ShowAsync<AddUserDialog>(null, DefaultBehaviors.DialogOptions);
        var result = await dialog.Result;
    }

    public async Task<IdentityResult> CreateUserAsync(string email, string password)
    {
        ClaimsPrincipal principal = await GetUserClaimsPrincipalAsync();
        if (!await ShowErrorIfNoPermissionAsync(_loc, _dialogService, PermissionEntityType.User, PermissionLevel.Write))
        {
            return IdentityResult.Failed(new IdentityError { Code = "no-permission", Description = "No permission" });
        }

        return await _userManager.AddUserAsync(principal, email, password);
    }
    public async Task AssignRoleAsync(ApplicationUser user, string role)
    {

        if (user.NormalizedEmail is null)
        {
            throw new ArgumentNullException(nameof(user.NormalizedEmail));
        }

        ClaimsPrincipal principal = await GetUserClaimsPrincipalAsync();
        if (!await ShowErrorIfNoPermissionAsync(_loc, _dialogService, PermissionEntityType.User, PermissionLevel.Write))
        {
            return;
        }

        await _userManager.AssignRoleAsync(principal, user.NormalizedEmail, role);
    }
    public async Task UnassignRoleAsync(ApplicationUser user, string role)
    {
        if (user.NormalizedEmail is null)
        {
            throw new ArgumentNullException(nameof(user.NormalizedEmail));
        }

        ClaimsPrincipal principal = await GetUserClaimsPrincipalAsync();
        if (!await ShowErrorIfNoPermissionAsync(_loc, _dialogService, PermissionEntityType.User, PermissionLevel.Write))
        {
            return;
        }

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
        if (!await ShowErrorIfNoPermissionAsync(_loc, _dialogService, PermissionEntityType.User, PermissionLevel.Write))
        {
            return;
        }

        await _userManager.AddRoleAsync(principal, role);

    }
    public async Task<IReadOnlyList<string>> GetRoleNamesAsync()
    {
        ClaimsPrincipal principal = await GetUserClaimsPrincipalAsync();
        principal.ThrowIfNoPermission(PermissionEntityType.User, PermissionLevel.Read);
        return await _userManager.GetRoleNamesAsync(principal);
    }
    public async Task<IReadOnlyList<string>> GetUserRoleNamesAsync(string normalizedUserName)
    {
        ClaimsPrincipal principal = await GetUserClaimsPrincipalAsync();
        principal.ThrowIfNoPermission(PermissionEntityType.User, PermissionLevel.Read);
        return await _userManager.GetUserRoleNamesAsync(principal, normalizedUserName);
    }


    public async Task<ApplicationUser?> GetUserByNormalizedUserNameAsync(string normalizedUserName)
    {
        ClaimsPrincipal principal = await GetUserClaimsPrincipalAsync();
        principal.ThrowIfNoPermission(PermissionEntityType.User, PermissionLevel.Read);
        return await _userManager.GetUserByNormalizedUserNameAsync(principal, normalizedUserName);
    }
    public async Task<PagedResult<ApplicationUser>> BrowseAsync(int offset, int count)
    {
        ClaimsPrincipal principal = await GetUserClaimsPrincipalAsync();
        principal.ThrowIfNoPermission(PermissionEntityType.User, PermissionLevel.Read);
        return await _userManager.BrowseAsync(principal, offset, count);
    }
}
