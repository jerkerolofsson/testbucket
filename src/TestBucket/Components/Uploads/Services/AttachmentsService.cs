using TestBucket.Domain.Files;
using TestBucket.Domain.Files.Models;

namespace TestBucket.Components.Uploads.Services;

internal class AttachmentsService : TenantBaseService
{
    private readonly IFileRepository _fileRepository;

    public AttachmentsService(IFileRepository fileRepository, AuthenticationStateProvider authenticationStateProvider) : base(authenticationStateProvider)
    {
        _fileRepository = fileRepository;
    }

    /// <summary>
    /// Deletes the resource
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task DeleteResourceByIdAsync(long id)
    {
        var tenantId = await GetTenantIdAsync();
        await _fileRepository.DeleteResourceByIdAsync(tenantId, id);
    }
    /// <summary>
    /// Gets the resource with data
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task<FileResource?> GetResourceByIdAsync(long id)
    {
        var tenantId = await GetTenantIdAsync();
        return await _fileRepository.GetResourceByIdAsync(tenantId, id);
    }

    /// <summary>
    /// Returns attachments to a test run
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task<IReadOnlyList<FileResource>> GetTestRunAttachmentsAsync(long id)
    {
        var tenantId = await GetTenantIdAsync();
        return await _fileRepository.GetTestRunAttachmentsAsync(tenantId, id);
    }

    /// <summary>
    /// Returns attachments to a requirement
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task<IReadOnlyList<FileResource>> GetRequirementAttachmentsAsync(long id)
    {
        var tenantId = await GetTenantIdAsync();
        return await _fileRepository.GetRequirementAttachmentsAsync(tenantId, id);
    }

    /// <summary>
    /// Returns attachments to a requirement
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task<IReadOnlyList<FileResource>> GetRequirementSpecificationAttachmentsAsync(long id)
    {
        var tenantId = await GetTenantIdAsync();
        return await _fileRepository.GetRequirementSpecificationAttachmentsAsync(tenantId, id);
    }

    /// <summary>
    /// Returns attachments to a test case
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task<IReadOnlyList<FileResource>> GetTestCaseRunAttachmentsAsync(long testCaseRunId)
    {
        var tenantId = await GetTenantIdAsync();
        return await _fileRepository.GetTestCaseRunAttachmentsAsync(tenantId, testCaseRunId);
    }

    /// <summary>
    /// Returns attachments to a test case
    /// </summary>
    /// <param name="testCaseId"></param>
    /// <returns></returns>
    public async Task<IReadOnlyList<FileResource>> GetTestCaseAttachmentsAsync(long testCaseId)
    {
        var tenantId = await GetTenantIdAsync();
        return await _fileRepository.GetTestCaseAttachmentsAsync(tenantId, testCaseId);
    }

    /// <summary>
    /// Returns attachments for a test suite
    /// </summary>
    /// <param name="testSuiteId"></param>
    /// <returns></returns>
    public async Task<IReadOnlyList<FileResource>> GetTestSuiteAttachmentsAsync(long testSuiteId)
    {
        var tenantId = await GetTenantIdAsync();
        return await _fileRepository.GetTestSuiteAttachmentsAsync(tenantId, testSuiteId);
    }

    /// <summary>
    /// Returns attachments for a project
    /// </summary>
    /// <param name="testProjectId"></param>
    /// <returns></returns>
    public async Task<IReadOnlyList<FileResource>> GetTestProjectAttachmentsAsync(long testProjectId)
    {
        var tenantId = await GetTenantIdAsync();
        return await _fileRepository.GetTestProjectAttachmentsAsync(tenantId, testProjectId);
    }

    /// <summary>
    /// Returns attachments to a test suite folder (like a feature)
    /// </summary>
    /// <param name="testSuiteFolderId"></param>
    /// <returns></returns>
    public async Task<IReadOnlyList<FileResource>> GetTestSuiteFolderAttachmentsAsync(long testSuiteFolderId)
    {
        var tenantId = await GetTenantIdAsync();
        return await _fileRepository.GetTestSuiteFolderAttachmentsAsync(tenantId, testSuiteFolderId);
    }
}
