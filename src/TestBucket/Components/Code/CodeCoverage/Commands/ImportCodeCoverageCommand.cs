
using Quartz;

using TestBucket.CodeCoverage;
using TestBucket.Components.Shared.Alerts;
using TestBucket.Contracts.Localization;
using TestBucket.Domain.Code.CodeCoverage.Import;
using TestBucket.Domain.Commands;
using TestBucket.Domain.Files;
using TestBucket.Domain.Jobs;
using TestBucket.Domain.Keyboard;

namespace TestBucket.Components.Code.CodeCoverage.Commands;

internal class ImportCodeCoverageCommand : ICommand
{
    private readonly IAppLocalization _loc;
    private readonly ISchedulerFactory _scheduler;
    private readonly AppNavigationManager _appNav;
    private readonly AlertController _alertController;

    public ImportCodeCoverageCommand(IAppLocalization loc, ISchedulerFactory scheduler, AppNavigationManager appNav, AlertController alertController)
    {
        _loc = loc;
        _scheduler = scheduler;
        _appNav = appNav;
        _alertController = alertController;
    }

    public string Id => "import-code-coverage";

    public string Name => _loc.Code["import-code-coverage"];

    public string Description => _loc.Code["import-code-coverage-description"];

    public bool Enabled => CodeCoverageMediaTypes.IsCodeCoverageFile(_appNav.State.SelectedFileResource?.ContentType);

    public KeyboardBinding? DefaultKeyboardBinding => null;

    public string? Icon => null;

    public string[] ContextMenuTypes => ["file"];

    public int SortOrder => 1;

    public string? Folder => null;

    public PermissionEntityType? PermissionEntityType => Domain.Identity.Permissions.PermissionEntityType.Project;

    public PermissionLevel? RequiredLevel => PermissionLevel.Read;

    public async ValueTask ExecuteAsync(ClaimsPrincipal principal)
    {
        var file = _appNav.State.SelectedFileResource;
        if (file is null)
        {
            return;
        }

        // Make sure the user has read-access to the resource
        if (!principal.HasPermission(file, PermissionLevel.Read))
        {
            await _alertController.ShowNoPermissionAlertAsync();
            return;
        }

        var scheduler = await _scheduler.GetScheduler();
        var jobData = new JobDataMap
        {
            { "ResourceId", file.Id.ToString() }
        };
        jobData.AddUser(principal);

        await scheduler.TriggerJob(new JobKey(nameof(ImportCodeCoverageResourceJob)), jobData);
    }
}
