using Microsoft.Extensions.Localization;

using TestBucket.Contracts.Fields;
using TestBucket.Contracts.Issues.States;
using TestBucket.Domain.Fields;
using TestBucket.Domain.Insights.Model;
using TestBucket.Domain.Issues;
using TestBucket.Domain.Issues.Models;
using TestBucket.Domain.Issues.Search;
using TestBucket.Localization;

namespace TestBucket.Components.Issues.Controllers;

internal class IssueController : TenantBaseService
{
    private readonly AppNavigationManager _appNavigationManager;
    private readonly IStringLocalizer<SharedStrings> _loc;
    private readonly IDialogService _dialogService;
    private readonly IIssueManager _manager;

    public IssueController(
        AppNavigationManager appNavigationManager,
        IStringLocalizer<SharedStrings> loc,
        IDialogService dialogService,
        IIssueManager manager, 
        AuthenticationStateProvider provider)
        : base(provider)
    {
        _appNavigationManager = appNavigationManager;
        _loc = loc;
        _dialogService = dialogService;
        _manager = manager;
    }

    public async Task<InsightsData<MappedIssueState, int>> GetIssueCountPerStateAsync(SearchIssueQuery request)
    {
        var principal = await GetUserClaimsPrincipalAsync();
        return await _manager.GetIssueCountPerStateAsync(principal, request);

    }

    public async Task<PagedResult<LocalIssue>> SearchAsync(long projectId, string text, int offset, int count)
    {
        var principal = await GetUserClaimsPrincipalAsync();
        return await _manager.SearchLocalIssuesAsync(principal, projectId, text, offset,count);
    }
    public async Task DeleteLocalIssueAsync(LocalIssue issue)
    {
        var result = await _dialogService.ShowMessageBox(new MessageBoxOptions
        {
            YesText = _loc["yes"],
            NoText = _loc["no"],
            Title = _loc["confirm-delete-title"],
            MarkupMessage = new MarkupString(_loc["confirm-delete-message"])
        });
        if (result == true)
        {
            var principal = await GetUserClaimsPrincipalAsync();
            await _manager.DeleteLocalIssueAsync(principal, issue);
            _appNavigationManager.NavigateTo(_appNavigationManager.GetIssuesUrl(), false);
        }
    }
    public async Task<LocalIssue?> GetIssueByIdAsync(long id)
    {
        var principal = await GetUserClaimsPrincipalAsync();
        return await _manager.GetIssueByIdAsync(principal, id);
    }
    public async Task UpdateIssueAsync(LocalIssue issue)
    {
        var principal = await GetUserClaimsPrincipalAsync();
        await _manager.UpdateLocalIssueAsync(principal, issue);
    }
    public async Task AddIssueAsync(LocalIssue issue)
    {
        var principal = await GetUserClaimsPrincipalAsync();
        await _manager.AddLocalIssueAsync(principal, issue);
    }
    public async Task AddIssueAsync(LinkedIssue issue)
    {
        var principal = await GetUserClaimsPrincipalAsync();
        await _manager.AddLinkedIssueAsync(principal, issue);
    }
}
