using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Components.Forms;

namespace TestBucket.MudBlazorExtensions.Markdown;

/// <summary>
/// This should be implemented by users to enable image uploads
/// </summary>
public abstract class ImageUploadHandler
{
    /// <summary>
    /// Uploads the image and returns an URL to the image
    /// </summary>
    /// <param name="file"></param>
    /// <returns></returns>
    public abstract Task<string?> UploadImageAsync(IBrowserFile file, long maxfileSize);
}
