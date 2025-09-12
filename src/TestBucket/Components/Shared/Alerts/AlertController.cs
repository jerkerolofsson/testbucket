
using TestBucket.Contracts.Localization;

namespace TestBucket.Components.Shared.Alerts;

internal class AlertController : TenantBaseService
{
    private readonly IAppLocalization _loc;
    private readonly IDialogService _dialogService;

    public AlertController(IDialogService dialogService, IAppLocalization loc, AuthenticationStateProvider authenticationStateProvider) : base(authenticationStateProvider)
    {
        _dialogService = dialogService;
        _loc = loc;
    }

    public async Task<bool> HasPermissionAsync(PermissionEntityType type, PermissionLevel level)
    {
        ClaimsPrincipal principal = await GetUserClaimsPrincipalAsync();
        if (!PermissionClaims.HasPermission(principal, type, level))
        {
            await ShowNoPermissionAlertAsync();
            return false;
        }
        return true;
    }

    public async Task ShowNoPermissionAlertAsync()
    {
        await _dialogService.ShowMessageBox(new MessageBoxOptions
        {
            YesText = _loc.Shared["ok"],
            Title = _loc.Shared["no-permission-title"],
            MarkupMessage = new MarkupString(_loc.Shared["no-permission-message"])
        });
    }
}
