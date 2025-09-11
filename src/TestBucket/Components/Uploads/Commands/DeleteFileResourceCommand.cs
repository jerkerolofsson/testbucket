using Quartz;

using TestBucket.CodeCoverage;
using TestBucket.Components.Uploads.Services;
using TestBucket.Contracts.Localization;
using TestBucket.Domain;
using TestBucket.Domain.Code.CodeCoverage.Import;
using TestBucket.Domain.Commands;
using TestBucket.Domain.Files;
using TestBucket.Domain.Identity.Permissions;
using TestBucket.Domain.Keyboard;

namespace TestBucket.Components.Uploads.Commands;

internal class DeleteFileResourceCommand : ICommand
{
    private readonly IAppLocalization _loc;
    private readonly AppNavigationManager _appNav;
    private readonly AttachmentsService _attachments;

    public DeleteFileResourceCommand(IAppLocalization loc, AppNavigationManager appNav, AttachmentsService attachments)
    {
        _loc = loc;
        _appNav = appNav;
        _attachments = attachments;
    }

    public string Id => "delete-file-resource";

    public string Name => _loc.Shared["delete"];

    public string Description => "";

    public bool Enabled => _appNav.State.SelectedFileResource is not null;

    public KeyboardBinding? DefaultKeyboardBinding => null;

    public string? Icon => Icons.Material.Filled.Delete;

    public string[] ContextMenuTypes => ["file"];

    public int SortOrder => 1;

    public string? Folder => null;

    public PermissionEntityType? PermissionEntityType => null;

    public PermissionLevel? RequiredLevel => null;

    public async ValueTask ExecuteAsync(ClaimsPrincipal principal)
    {
        var file = _appNav.State.SelectedFileResource;
        if (file is null)
        {
            return;
        }

        await _attachments.DeleteResourceByIdAsync(file.Id);

    }
}
