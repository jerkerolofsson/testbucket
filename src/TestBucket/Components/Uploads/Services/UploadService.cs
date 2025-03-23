
using Microsoft.AspNetCore.Components.Forms;

using TestBucket.Components.Tenants;
using TestBucket.Domain.Files;
using TestBucket.Domain.Files.Models;
using TestBucket.Domain.Testing.Models;
using TestBucket.Formats.Dtos;

namespace TestBucket.Components.Uploads.Services;

internal class UploadService : TenantBaseService
{
    private readonly IFileRepository _repo;

    public UploadService(AuthenticationStateProvider authenticationStateProvider, IFileRepository repo) : base(authenticationStateProvider)
    {
        _repo = repo;
    }

    public async Task<FileResource> UploadAsync(ResourceCategory category, IBrowserFile file, long maxFileSize = 512_000, CancellationToken cancellationToken = default)
    {
        return await UploadAsync(category, file, null, null, null, null, null, null, null, null, maxFileSize, cancellationToken);
    }

    public async Task<FileResource> UploadAsync(ResourceCategory category, IBrowserFile file, 
        long? testCaseId, long? testRunId, long? testCaseRunId, long? testSuiteId, long? testSuiteFolderId, long? testProjectId,
        long? requirementId, long? requirementSpecificationId,
        long maxFileSize = 512_000, CancellationToken cancellationToken = default)
    {
        var tenantId = await GetTenantIdAsync();

        using var stream = file.OpenReadStream(maxFileSize, cancellationToken);
        var data = new byte[stream.Length];
        await stream.ReadExactlyAsync(data, 0, data.Length, cancellationToken);

        var resource = new FileResource
        {
            Name = file.Name,
            ContentType = file.ContentType,
            Data = data,
            TenantId = tenantId,
            Length = data.Length,
            Created = DateTime.UtcNow,

            RequirementId = requirementId,
            RequirementSpecificationId = requirementSpecificationId,

            TestCaseId = testCaseId,
            TestRunId = testRunId,
            TestCaseRunId = testCaseRunId,
            TestProjectId = testProjectId,
            TestSuiteId = testSuiteId,
            TestSuiteFolderId = testSuiteFolderId
        };

        await _repo.AddResourceAsync(resource);

        return resource;
    }

    public async Task<FileResource> UploadAsync(ResourceCategory category, AttachmentDto attachment,
      long? testCaseId, long? testRunId, long? testCaseRunId, long? testSuiteId, long? testSuiteFolderId, long? testProjectId,
      long? requirementId, long? requirementSpecificationId,
      CancellationToken cancellationToken = default)
    {
        var tenantId = await GetTenantIdAsync();

        var data = attachment.Data ?? [];

        var resource = new FileResource
        {
            Name = attachment.Name,
            ContentType = attachment.ContentType ?? "application/octet-stream",
            Data = data,
            TenantId = tenantId,
            Length = data.Length,
            Created = DateTime.UtcNow,

            RequirementId = requirementId,
            RequirementSpecificationId = requirementSpecificationId,

            TestCaseId = testCaseId,
            TestRunId = testRunId,
            TestCaseRunId = testCaseRunId,
            TestProjectId = testProjectId,
            TestSuiteId = testSuiteId,
            TestSuiteFolderId = testSuiteFolderId
        };

        await _repo.AddResourceAsync(resource);

        return resource;
    }
}
