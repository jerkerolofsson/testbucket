
using Microsoft.Extensions.Localization;

using TestBucket.Components.Metrics.Controls.Dialogs;
using TestBucket.Domain.Metrics;
using TestBucket.Domain.Metrics.Models;
using TestBucket.Localization;

namespace TestBucket.Components.Metrics.Controls.Controllers;

internal class MetricsController : TenantBaseService
{
    private readonly IMetricsManager _manager;
    private readonly IDialogService _dialogService;
    private readonly IStringLocalizer<SharedStrings> _loc;

    public MetricsController(AuthenticationStateProvider authenticationStateProvider, IMetricsManager manager, IDialogService dialogService, IStringLocalizer<SharedStrings> loc) : base(authenticationStateProvider)
    {
        _manager = manager;
        _dialogService = dialogService;
        _loc = loc;
    }

    public async Task DeleteAsync(Metric metric)
    {
        if (metric is null)
        {
            return;
        }
        var principal = await GetUserClaimsPrincipalAsync();
        if (principal is null)
        {
            return;
        }
        var confirmResult = await _dialogService.ShowMessageBox(new MessageBoxOptions
        {
            YesText = _loc["yes"],
            NoText = _loc["no"],
            Title = _loc["confirm-delete-title"],
            MarkupMessage = new MarkupString(_loc["confirm-delete-message"])
        });
        if (confirmResult is false)
            return;
        
        await _manager.DeleteMetricAsync(principal, metric);
    }
    public async Task<Metric?> AddAsync(TestCaseRun testCaseRun)
    {
        var principal = await GetUserClaimsPrincipalAsync();
        var metrics = new Metric() { MeterName = principal.Identity?.Name ?? "user", Name = "", Value = 0, TestCaseRunId = testCaseRun.Id };

        // Show Metric dialog 
        var parameters = new DialogParameters<MetricsEditorDialog>
        {
            { x => x.Metric, metrics }
        };

        var dialog = await _dialogService.ShowAsync<MetricsEditorDialog>(null, parameters, DefaultBehaviors.DialogOptions);
        var result = await dialog.Result;
        if (result?.Data is Metric createdMetric)
        {
            if (createdMetric is not null)
            {
                await _manager.AddMetricAsync(principal, createdMetric);  

                testCaseRun.Metrics ??= [];
                testCaseRun.Metrics.Add(createdMetric);
            }
            return createdMetric;
        }
        return null;
    }
}
