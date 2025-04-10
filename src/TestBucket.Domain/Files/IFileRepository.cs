using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Domain.Files.Models;

namespace TestBucket.Domain.Files;

/// <summary>
/// Contains files/attachments
/// </summary>
public interface IFileRepository
{
    /// <summary>
    /// Gets a resource
    /// </summary>
    /// <param name="tenantId"></param>
    /// <param name="id"></param>
    /// <returns></returns>
    Task<FileResource?> GetResourceByIdAsync(string tenantId, long id);

    /// <summary>
    /// Adds a resource
    /// </summary>
    /// <param name="resource"></param>
    /// <returns></returns>
    Task AddResourceAsync(FileResource resource);

    /// <summary>
    /// Deletes a resource
    /// </summary>
    /// <param name="tenantId"></param>
    /// <param name="id"></param>
    /// <returns></returns>
    Task DeleteResourceByIdAsync(string tenantId, long id);

    /// <summary>
    /// Returns attachments to a test case
    /// </summary>
    /// <param name="tenantId"></param>
    /// <param name="testCaseId"></param>
    /// <returns></returns>
    Task<IReadOnlyList<FileResource>> GetTestCaseAttachmentsAsync(string tenantId, long testCaseId);

    /// <summary>
    /// Returns attachments to a test case run
    /// </summary>
    /// <param name="tenantId"></param>
    /// <param name="testCaseRunId"></param>
    /// <returns></returns>
    Task<IReadOnlyList<FileResource>> GetTestCaseRunAttachmentsAsync(string tenantId, long testCaseRunId);

    /// <summary>
    /// Returns attachments for a test suite
    /// </summary>
    /// <param name="tenantId"></param>
    /// <param name="testSuiteId"></param>
    /// <returns></returns>
    Task<IReadOnlyList<FileResource>> GetTestSuiteAttachmentsAsync(string tenantId, long testSuiteId);

    /// <summary>
    /// Returns attachments for a project
    /// </summary>
    /// <param name="tenantId"></param>
    /// <param name="testProjectId"></param>
    /// <returns></returns>
    Task<IReadOnlyList<FileResource>> GetTestProjectAttachmentsAsync(string tenantId, long testProjectId);

    /// <summary>
    /// Returns attachments to a test suite folder (like a feature)
    /// </summary>
    /// <param name="tenantId"></param>
    /// <param name="testSuiteFolderId"></param>
    /// <returns></returns>
    Task<IReadOnlyList<FileResource>> GetTestSuiteFolderAttachmentsAsync(string tenantId, long testSuiteFolderId);

    /// <summary>
    /// Returns attachments to test run
    /// </summary>
    /// <param name="tenantId"></param>
    /// <param name="testRunId"></param>
    /// <returns></returns>
    Task<IReadOnlyList<FileResource>> GetTestRunAttachmentsAsync(string tenantId, long testRunId);

    /// <summary>
    /// Returns attachments to requirement
    /// </summary>
    /// <param name="tenantId"></param>
    /// <param name="requirementId"></param>
    /// <returns></returns>
    Task<IReadOnlyList<FileResource>> GetRequirementAttachmentsAsync(string tenantId, long requirementId);

    /// <summary>
    /// Returns attachments to requirement spec.
    /// </summary>
    /// <param name="tenantId"></param>
    /// <param name="requirementSpecificationId"></param>
    /// <returns></returns>
    Task<IReadOnlyList<FileResource>> GetRequirementSpecificationAttachmentsAsync(string tenantId, long requirementSpecificationId);


}
