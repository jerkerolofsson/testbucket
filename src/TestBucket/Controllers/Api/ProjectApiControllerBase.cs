using System.Net.Mime;

using Microsoft.AspNetCore.Mvc;

namespace TestBucket.Controllers.Api;

public class ProjectApiControllerBase : ControllerBase
{
    protected async Task<string> ReadRequestBodyAsync()
    {
        var encoding = Encoding.UTF8;
        if (Request.ContentType is not null)
        {
            var contentType = new ContentType(Request.ContentType);
            if(contentType.CharSet is not null)
            {
                encoding = Encoding.GetEncoding(contentType.CharSet);
            }
        }

        if (Request.Body.CanSeek)
        {
            Request.Body.Seek(0, SeekOrigin.Begin);
        }
        var bodyStream = new StreamReader(Request.Body, encoding);
        return await bodyStream.ReadToEndAsync();
    }

    protected async Task<byte[]> ReadRequestBodyAsByteArrayAsync()
    {
        if (Request.Body.CanSeek)
        {
            Request.Body.Seek(0, SeekOrigin.Begin);
        }
        using var memoryStream = new MemoryStream();
        await Request.Body.CopyToAsync(memoryStream);
        return memoryStream.ToArray();
    }
}
