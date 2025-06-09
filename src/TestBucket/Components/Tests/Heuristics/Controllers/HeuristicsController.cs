using Microsoft.Extensions.Localization;

using OllamaSharp.Models.Chat;

using TestBucket.Components.Tests.Heuristics.Dialog;
using TestBucket.Domain.Shared;
using TestBucket.Domain.Shared.Specifications;
using TestBucket.Domain.Testing.Heuristics;
using TestBucket.Domain.Testing.Heuristics.Models;
using TestBucket.Localization;

namespace TestBucket.Components.Tests.Heuristics.Controllers;

internal class HeuristicsController : TenantBaseService
{
    private readonly IHeuristicsManager _manager;
    private readonly IDialogService _dialogService;
    private readonly IStringLocalizer<SharedStrings> _loc;

    public HeuristicsController(AuthenticationStateProvider authenticationStateProvider, IHeuristicsManager manager, IStringLocalizer<SharedStrings> loc, IDialogService dialogService) : base(authenticationStateProvider)
    {
        _manager = manager;
        _loc = loc;
        _dialogService = dialogService;
    }

    public async Task AddAsync(TestProject project)
    {
        var principal = await GetUserClaimsPrincipalAsync();

        var hasPermission = await ShowErrorIfNoPermissionAsync(_loc, _dialogService, PermissionEntityType.Heuristic, PermissionLevel.Write);
        if (!hasPermission)
            return;

        var heuristic = new Heuristic() { Description = "", Name = "", TestProjectId = project.Id };

        var parameters = new DialogParameters<EditHeuristicDialog>
        {
            { x => x.Heuristic, heuristic }
        };

        var dialog = await _dialogService.ShowAsync<EditHeuristicDialog>(_loc["add-heuristic"], parameters, DefaultBehaviors.DialogOptions);
        var result = await dialog.Result;
        if (result?.Data is Heuristic editedHeuristic)
        {
            await _manager.AddAsync(principal, editedHeuristic);
        }
    }

    public async Task<IReadOnlyList<Heuristic>> GetHeuristicsAsync(long projectId)
    {
        var principal = await GetUserClaimsPrincipalAsync();
        var hasPermission = await ShowErrorIfNoPermissionAsync(_loc, _dialogService, PermissionEntityType.Heuristic, PermissionLevel.Read);
        if (!hasPermission)
            return [];

        // Filter heuristics by project
        var filters = new FilterSpecification<Heuristic>[]
        {
            new FilterByProject<Heuristic>(projectId),
            new FilterByTenant<Heuristic>(principal.GetTenantIdOrThrow())
        };

        var result = await _manager.SearchAsync(principal, filters, 0, 1000);
        return result.Items;
    }

    public async Task EditAsync(Heuristic heuristic)
    {
        var principal = await GetUserClaimsPrincipalAsync();
        var hasPermission = await ShowErrorIfNoPermissionAsync(_loc, _dialogService, PermissionEntityType.Heuristic, PermissionLevel.Write);
        if (!hasPermission)
            return;

        var parameters = new DialogParameters<EditHeuristicDialog>
        {
            { x => x.Heuristic, heuristic }
        };
        var dialog = await _dialogService.ShowAsync<EditHeuristicDialog>(_loc["add-heuristic"], parameters, DefaultBehaviors.DialogOptions);
        var result = await dialog.Result;
        if (result?.Data is Heuristic editedHeuristic)
        {
            await _manager.UpdateAsync(principal, editedHeuristic);
        }

    }
    public async Task DeleteAsync(Heuristic heuristic)
    {
        var principal = await GetUserClaimsPrincipalAsync();

        var hasPermission = await ShowErrorIfNoPermissionAsync(_loc, _dialogService, PermissionEntityType.Heuristic, PermissionLevel.Delete);
        if (!hasPermission)
            return;

        // Show confirmation dialog before deleting
        var confirmResult = await _dialogService.ShowMessageBox(new MessageBoxOptions
        {
            YesText = _loc["yes"],
            NoText = _loc["no"],
            Title = _loc["confirm-delete-title"],
            MarkupMessage = new MarkupString(_loc["confirm-delete-message"])
        });
        if (confirmResult is false)
            return;

        await _manager.DeleteAsync(principal, heuristic);
    }
}
