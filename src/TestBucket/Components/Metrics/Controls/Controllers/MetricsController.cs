
using TestBucket.Components.Metrics.Controls.Dialogs;
using TestBucket.Domain.Metrics;
using TestBucket.Domain.Metrics.Models;

namespace TestBucket.Components.Metrics.Controls.Controllers;

internal class MetricsController : TenantBaseService
{
    private readonly IMetricsManager _manager;
    private readonly IDialogService _dialogService;

    public MetricsController(AuthenticationStateProvider authenticationStateProvider, IMetricsManager manager, IDialogService dialogService) : base(authenticationStateProvider)
    {
        _manager = manager;
        _dialogService = dialogService;
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
