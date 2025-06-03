using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

using TestBucket.Localization;

namespace TestBucket.Components.Tenants;
internal abstract class TenantBaseService
{
    private readonly AuthenticationStateProvider _authenticationStateProvider;
    private string? _tenantId;

    public TenantBaseService(AuthenticationStateProvider authenticationStateProvider)
    {
        _authenticationStateProvider = authenticationStateProvider;
    }

    /// <summary>
    /// Shows an error message box if the user has no permission to do the task
    /// </summary>
    /// <param name="dialogService"></param>
    /// <param name="entityType"></param>
    /// <param name="level"></param>
    /// <returns>True if the user has permission</returns>
    protected async Task<bool> ShowErrorIfNoPermissionAsync(
        IStringLocalizer<SharedStrings> loc,
        IDialogService dialogService, PermissionEntityType entityType, PermissionLevel level)
    {
        var principal = await GetUserClaimsPrincipalAsync();
        if (!principal.HasPermission(PermissionEntityType.User, PermissionLevel.Delete))
        {
            await dialogService.ShowMessageBox(new MessageBoxOptions
            {
                CancelText = loc["cancel"],
                Title = loc["no-permission-title"],
                MarkupMessage = new MarkupString(loc["no-permission-message"])
            });
            return false;
        }
        return true;
    }

    protected async Task<ClaimsPrincipal> GetUserClaimsPrincipalAsync()
    {
        var authState = await _authenticationStateProvider.GetAuthenticationStateAsync();
        return authState.User;
    }

    /// <summary>
    /// Returns the tenant from the authenticated users claims
    /// </summary>
    /// <returns></returns>
    protected async Task<string> GetTenantIdAsync()
    {
        if (_tenantId is not null)
        {
            return _tenantId;
        }
        var authState = await _authenticationStateProvider.GetAuthenticationStateAsync();
        var claims = authState.User.Claims;
        _tenantId = authState.User.Claims.Where(x => x.Type == "tenant").Select(x => x.Value).FirstOrDefault();
        return _tenantId ?? throw new InvalidDataException("User is not authenticated or is missing the tenant claim");
    }

    protected async Task<string?> GetUserNameAsync()
    {
        var authState = await _authenticationStateProvider.GetAuthenticationStateAsync();
        return authState.User.Identity?.Name;
    }
}
