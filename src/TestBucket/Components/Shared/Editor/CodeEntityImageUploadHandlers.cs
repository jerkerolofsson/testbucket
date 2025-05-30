using Microsoft.AspNetCore.Components.Forms;

using TestBucket.Components.Uploads.Services;
using TestBucket.Domain.Files.Models;

namespace TestBucket.Components.Shared.Editor;
internal class ComponentEntityImageUploadHandler(UploadService uploadService, long? componentId) : ImageUploadHandler
{
    public override async Task<string?> UploadImageAsync(IBrowserFile file, long maxFileSize)
    {
        if(componentId is null)
        {
            return null;
        }
        var fileResource = await uploadService.UploadAsync(ResourceCategory.Attachment, file, 
            new FileResourceRelatedEntity { ComponentId = componentId }
            , maxFileSize);
        return $"/api/resources/{fileResource.Id}";
    }
}

internal class FeatureEntityImageUploadHandler(UploadService uploadService, long? featureId) : ImageUploadHandler
{
    public override async Task<string?> UploadImageAsync(IBrowserFile file, long maxFileSize)
    {
        if (featureId is null)
        {
            return null;
        }
        var fileResource = await uploadService.UploadAsync(ResourceCategory.Attachment, file,
            new FileResourceRelatedEntity { FeatureId = featureId }
            , maxFileSize);
        return $"/api/resources/{fileResource.Id}";
    }
}
