

using Microsoft.Extensions.Localization;

using TestBucket.Domain.Testing.TestLab;
using TestBucket.Localization;

namespace TestBucket.Components.Tests.TestLab.Services;

internal class TestLabController : TenantBaseService
{
    private readonly ITestLabManager _manager;
    private readonly IStringLocalizer<SharedStrings> _loc;
    private readonly IDialogService _dialogService;
    public TestLabController(AuthenticationStateProvider authenticationStateProvider, ITestLabManager manager, IStringLocalizer<SharedStrings> loc, IDialogService dialogService) : base(authenticationStateProvider)
    {
        _manager = manager;
        _loc = loc;
        _dialogService = dialogService;
    }

    /// <summary>
    /// Deletes a folder
    /// </summary>
    /// <param name="folderId"></param>
    /// <returns></returns>
    public async Task DeleteFolderByIdAsync(long folderId)
    {
        if (!await ShowErrorIfNoPermissionAsync(_loc, _dialogService, PermissionEntityType.TestSuite, PermissionLevel.Delete))
        {
            return;
        }

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
            await _manager.DeleteFolderAsync(principal, folderId);
        }
    }
    internal async Task AddTestLabFolderAsync(long projectId, long? parentFolderId, string name)
    {
        var principal = await GetUserClaimsPrincipalAsync();
        var folder = new TestLabFolder { Name = name, ParentId = parentFolderId, TestProjectId = projectId };
        await _manager.AddFolderAsync(principal, folder);
    }
    internal async Task<IReadOnlyList<TestLabFolder>> GetRootFoldersAsync(long projectId)
    {
        var principal = await GetUserClaimsPrincipalAsync();
        return await _manager.GetRootFoldersAsync(principal, projectId);
    }

    /// <summary>
    /// Returns all child folders within another folder
    /// </summary>
    /// <param name="projectId"></param>
    /// <param name="parentId">Parent folder ID</param>
    /// <returns></returns>
    internal async Task<IReadOnlyList<TestLabFolder>> GetFoldersAsync(long projectId, long parentId)
    {
        var principal = await GetUserClaimsPrincipalAsync();
        return await _manager.GetFoldersAsync(principal, projectId, parentId);
    }

    internal async Task UpdateTestLabFolderAsync(TestLabFolder testLabFolder)
    {
        var principal = await GetUserClaimsPrincipalAsync();
        await _manager.UpdateFolderAsync(principal, testLabFolder);
    }
}