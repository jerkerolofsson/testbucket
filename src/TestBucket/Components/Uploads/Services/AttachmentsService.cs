using System.Security.Principal;

using TestBucket.Contracts.Localization;
using TestBucket.Domain.Files;
using TestBucket.Domain.Files.Models;

namespace TestBucket.Components.Uploads.Services;

internal class AttachmentsService : TenantBaseService
{
    private readonly IFileResourceManager _fileResourceManager;
    private readonly IDialogService _dialogService;
    private readonly IAppLocalization _loc;

    public AttachmentsService(IFileResourceManager fileResourceManager, AuthenticationStateProvider authenticationStateProvider, IDialogService dialogService, IAppLocalization loc) : base(authenticationStateProvider)
    {
        _fileResourceManager = fileResourceManager;
        _dialogService = dialogService;
        _loc = loc;
    }

    /// <summary>
    /// Deletes the resource
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task<bool> DeleteResourceByIdAsync(long id)
    {
        var result = await _dialogService.ShowMessageBox(new MessageBoxOptions
        {
            YesText = _loc.Shared["yes"],
            NoText = _loc.Shared["no"],
            Title = _loc.Shared["confirm-delete-title"],
            MarkupMessage = new MarkupString(_loc.Shared["confirm-delete-message"])
        });
        if (result == true)
        {
            var principal = await GetUserClaimsPrincipalAsync();
            return await _fileResourceManager.DeleteResourceByIdAsync(principal, id);
        }
        return false;
    }

    /// <summary>
    /// Gets the resource with data
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task<FileResource?> GetResourceByIdAsync(long id)
    {
        var principal = await GetUserClaimsPrincipalAsync();
        return await _fileResourceManager.GetResourceByIdAsync(principal, id);
    }

    /// <summary>
    /// Returns attachments to a test run
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task<IReadOnlyList<FileResource>> GetTestRunAttachmentsAsync(long id)
    {
        var principal = await GetUserClaimsPrincipalAsync();
        return await _fileResourceManager.GetTestRunAttachmentsAsync(principal, id);
    }

    /// <summary>
    /// Returns attachments to a requirement
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task<IReadOnlyList<FileResource>> GetRequirementAttachmentsAsync(long id)
    {
        var principal = await GetUserClaimsPrincipalAsync();
        return await _fileResourceManager.GetRequirementAttachmentsAsync(principal, id);
    }

    /// <summary>
    /// Returns attachments to a requirement
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task<IReadOnlyList<FileResource>> GetRequirementSpecificationAttachmentsAsync(long id)
    {
        var principal = await GetUserClaimsPrincipalAsync();
        return await _fileResourceManager.GetRequirementSpecificationAttachmentsAsync(principal, id);
    }

    /// <summary>
    /// Returns attachments to a test case
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task<IReadOnlyList<FileResource>> GetTestCaseRunAttachmentsAsync(long testCaseRunId)
    {
        var principal = await GetUserClaimsPrincipalAsync();
        return await _fileResourceManager.GetTestCaseRunAttachmentsAsync(principal, testCaseRunId);
    }

    /// <summary>
    /// Returns attachments to a test case
    /// </summary>
    /// <param name="testCaseId"></param>
    /// <returns></returns>
    public async Task<IReadOnlyList<FileResource>> GetTestCaseAttachmentsAsync(long testCaseId)
    {
        var principal = await GetUserClaimsPrincipalAsync();
        return await _fileResourceManager.GetTestCaseAttachmentsAsync(principal, testCaseId);
    }

    /// <summary>
    /// Returns attachments for a test suite
    /// </summary>
    /// <param name="testSuiteId"></param>
    /// <returns></returns>
    public async Task<IReadOnlyList<FileResource>> GetTestSuiteAttachmentsAsync(long testSuiteId)
    {
        var principal = await GetUserClaimsPrincipalAsync();
        return await _fileResourceManager.GetTestSuiteAttachmentsAsync(principal, testSuiteId);
    }

    /// <summary>
    /// Returns attachments for a project
    /// </summary>
    /// <param name="testProjectId"></param>
    /// <returns></returns>
    public async Task<IReadOnlyList<FileResource>> GetTestProjectAttachmentsAsync(long testProjectId)
    {
        var principal = await GetUserClaimsPrincipalAsync();
        return await _fileResourceManager.GetTestProjectAttachmentsAsync(principal, testProjectId);
    }

    /// <summary>
    /// Returns attachments to a test suite folder (like a feature)
    /// </summary>
    /// <param name="testSuiteFolderId"></param>
    /// <returns></returns>
    public async Task<IReadOnlyList<FileResource>> GetTestSuiteFolderAttachmentsAsync(long testSuiteFolderId)
    {
        var principal = await GetUserClaimsPrincipalAsync();
        return await _fileResourceManager.GetTestSuiteFolderAttachmentsAsync(principal, testSuiteFolderId);
    }
}
