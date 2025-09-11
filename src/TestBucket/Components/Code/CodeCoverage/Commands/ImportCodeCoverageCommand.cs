
using Quartz;

using TestBucket.CodeCoverage;
using TestBucket.Components.Code.CodeCoverage.Jobs;
using TestBucket.Contracts.Localization;
using TestBucket.Domain.Commands;
using TestBucket.Domain.Keyboard;

namespace TestBucket.Components.Code.CodeCoverage.Commands;

public class ImportCodeCoverageCommand : ICommand
{
    private readonly IAppLocalization _loc;
    private readonly ISchedulerFactory _scheduler;
    private readonly AppNavigationManager _appNav;

    public ImportCodeCoverageCommand(IAppLocalization loc, ISchedulerFactory scheduler, AppNavigationManager appNav)
    {
        _loc = loc;
        _scheduler = scheduler;
        _appNav = appNav;
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

        var scheduler = await _scheduler.GetScheduler();
        var jobData = new JobDataMap
        {
            { "ResourceId", file.Id.ToString() },
            { "TenantId", file.TenantId }
        };
        await scheduler.TriggerJob(new JobKey(nameof(ImportCodeCoverageResourceJob)), jobData);
    }
}
