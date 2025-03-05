
using Microsoft.AspNetCore.Components.Forms;

using TestBucket.Components.Tenants;
using TestBucket.Domain.Files;
using TestBucket.Domain.Files.Models;

namespace TestBucket.Components.Uploads.Services;

internal class UploadService : TenantBaseService
{
    private readonly IFileRepository _repo;

    public UploadService(AuthenticationStateProvider authenticationStateProvider, IFileRepository repo) : base(authenticationStateProvider)
    {
        _repo = repo;
    }

    public async Task<FileResource> UploadAsync(IBrowserFile file, long maxFileSize = 512_000, CancellationToken cancellationToken = default)
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
            TenantId = tenantId
        };

        await _repo.AddResourceAsync(resource);

        return resource;
    }
}
