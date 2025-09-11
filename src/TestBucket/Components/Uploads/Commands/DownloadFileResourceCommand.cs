using Quartz;

using TestBucket.CodeCoverage;
using TestBucket.Contracts.Localization;
using TestBucket.Domain;
using TestBucket.Domain.Code.CodeCoverage.Import;
using TestBucket.Domain.Commands;
using TestBucket.Domain.Files;
using TestBucket.Domain.Identity.Permissions;
using TestBucket.Domain.Keyboard;

namespace TestBucket.Components.Uploads.Commands;

public class DownloadFileResourceCommand : ICommand
{
    private readonly IAppLocalization _loc;
    private readonly AppNavigationManager _appNav;

    public DownloadFileResourceCommand(IAppLocalization loc, AppNavigationManager appNav)
    {
        _loc = loc;
        _appNav = appNav;
    }

    public string Id => "download-file-resource";

    public string Name => _loc.Shared["download"];

    public string Description => "";

    public bool Enabled => _appNav.State.SelectedFileResource is not null;

    public KeyboardBinding? DefaultKeyboardBinding => null;

    public string? Icon => TbIcons.BoldDuoTone.CloudDownload;

    public string[] ContextMenuTypes => ["file"];

    public int SortOrder => 1;

    public string? Folder => null;

    public PermissionEntityType? PermissionEntityType => null;

    public PermissionLevel? RequiredLevel => null;

    public ValueTask ExecuteAsync(ClaimsPrincipal principal)
    {
        var file = _appNav.State.SelectedFileResource;
        if (file is null)
        {
            return ValueTask.CompletedTask;
        }

        principal.ThrowIfNoPermission(file);

        string url = $"/api/resources/{file.Id}";
        _appNav.NavigateTo(url);
        return ValueTask.CompletedTask;
    }
}
