

using Microsoft.Extensions.Localization;

using NGitLab.Models;

using TestBucket.Domain.Testing.TestRepository;
using TestBucket.Domain.Testing.TestSuites;
using TestBucket.Localization;

namespace TestBucket.Components.Tests.TestRepository.Services;

internal class TestRepositoryController : TenantBaseService
{
    private readonly ITestRepositoryManager _manager;
    private readonly IStringLocalizer<SharedStrings> _loc;
    private readonly IDialogService _dialogService;
    public TestRepositoryController(AuthenticationStateProvider authenticationStateProvider, ITestRepositoryManager manager, IDialogService dialogService, IStringLocalizer<SharedStrings> loc) : base(authenticationStateProvider)
    {
        _manager = manager;
        _dialogService = dialogService;
        _loc = loc;
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

    internal async Task AddTestRepositoryFolderAsync(long projectId, long? parentFolderId, string name)
    {
        var principal = await GetUserClaimsPrincipalAsync();
        var folder = new TestRepositoryFolder { Name = name, ParentId = parentFolderId, TestProjectId = projectId };
        await _manager.AddFolderAsync(principal, folder);
    }

    /// <summary>
    /// Returns all root folders
    /// </summary>
    /// <param name="projectId"></param>
    /// <returns></returns>
    internal async Task<IReadOnlyList<TestRepositoryFolder>> GetRootFoldersAsync(long projectId)
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
    internal async Task<IReadOnlyList<TestRepositoryFolder>> GetFoldersAsync(long projectId, long parentId)
    {
        var principal = await GetUserClaimsPrincipalAsync();
        return await _manager.GetFoldersAsync(principal, projectId, parentId);
    }

    internal async Task UpdateTestRepositoryFolderAsync(TestRepositoryFolder testRepositoryFolder)
    {
        var principal = await GetUserClaimsPrincipalAsync();
        await _manager.UpdateFolderAsync(principal, testRepositoryFolder);
    }
}
