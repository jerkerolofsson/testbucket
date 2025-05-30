using Microsoft.AspNetCore.Components.Forms;

using TestBucket.Components.Uploads.Services;
using TestBucket.Domain.Files.Models;

namespace TestBucket.Components.Shared.Editor;
internal class TestCaseImageUploadHandler(UploadService uploadService, long? testCaseId) : ImageUploadHandler
{
    public override async Task<string?> UploadImageAsync(IBrowserFile file, long maxFileSize)
    {
        if(testCaseId is null)
        {
            return null;
        }
        var fileResource = await uploadService.UploadTestCaseAttachmentAsync(file, testCaseId.Value, maxFileSize);
        return $"/api/resources/{fileResource.Id}";
    }
}
