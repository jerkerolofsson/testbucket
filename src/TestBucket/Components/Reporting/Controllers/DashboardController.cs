using TestBucket.Contracts.Localization;
using TestBucket.Domain.Insights;
using TestBucket.Domain.Insights.Model;

namespace TestBucket.Components.Reporting.Controllers;

internal class DashboardController : TenantBaseService
{
    private readonly IDashboardManager _manager;
    private readonly IDialogService _dialogService;
    private readonly IAppLocalization _localization;

    public DashboardController(AuthenticationStateProvider authenticationStateProvider, IDashboardManager manager, IDialogService dialogService, IAppLocalization localization) : base(authenticationStateProvider)
    {
        _manager = manager;
        _dialogService = dialogService;
        _localization = localization;
    }
    public async Task<bool> DeleteDashboardAsync(Dashboard dashboard)
    {

        // Show confirmation dialog before deleting
        var confirmResult = await _dialogService.ShowMessageBox(new MessageBoxOptions
        {
            YesText = _localization.Shared["yes"],
            NoText = _localization.Shared["no"],
            Title = _localization.Shared["confirm-delete-title"],
            MarkupMessage = new MarkupString(_localization.Shared["confirm-delete-message"])
        });
        if (confirmResult is false)
            return false;

        var principal = await GetUserClaimsPrincipalAsync();
        await _manager.DeleteDashboardAsync(principal, dashboard.Id);
        return true;
    }
    public async Task UpdateDashboardAsync(Dashboard dashboard)
    {
        var principal = await GetUserClaimsPrincipalAsync();
        await _manager.UpdateDashboardAsync(principal, dashboard);
    }
    public async Task<Dashboard?> GetDashboardByNameAsync(long projectId, string name)
    {
        var principal = await GetUserClaimsPrincipalAsync();
        return await _manager.GetDashboardByNameAsync(principal, projectId, name);
    }
    public async Task<Dashboard?> GetDashboardByIdAsync(long id)
    {
        var principal = await GetUserClaimsPrincipalAsync();
        return await _manager.GetDashboardAsync(principal, id);
    }
    public async Task<List<Dashboard>> GetDashboardsAsync(long testProjectId)
    {
        var principal = await GetUserClaimsPrincipalAsync();
        return (await _manager.GetAllDashboardsAsync(principal, testProjectId)).ToList();
    }
}
