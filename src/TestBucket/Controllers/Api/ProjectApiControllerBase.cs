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
    protected long? GetTestSuiteId()
    {
        var principal = this.User;
        var claims = principal.Claims.Where(x => x.Type == "testsuite" && !string.IsNullOrEmpty(x.Value)).ToList();
        if (claims.Count == 0)
        {
            return null;
        }
        var projectIdString = claims.First().Value;
        if (long.TryParse(projectIdString, out var id))
        {
            return id;
        }
        return null;
    }
    protected long? GetTestRunId()
    {
        var principal = this.User;
        var claims = principal.Claims.Where(x => x.Type == "run" && !string.IsNullOrEmpty(x.Value)).ToList();
        if (claims.Count == 0)
        {
            return null;
        }
        var projectIdString = claims.First().Value;
        if (long.TryParse(projectIdString, out var id))
        {
            return id;
        }
        return null;
    }
    protected long? GetProjectId()
    {
        var principal = this.User;
        var claims = principal.Claims.Where(x => x.Type == "project" && !string.IsNullOrEmpty(x.Value)).ToList();
        if (claims.Count == 0)
        {
            return null;
        }
        var projectIdString = claims.First().Value;
        if(long.TryParse(projectIdString, out var projectId))
        {
            return projectId;
        }
        return null;
    }
    protected string? GetTenantId()
    {
        var principal = this.User;
        var claims = principal.Claims.Where(x => x.Type == "tenant" && !string.IsNullOrEmpty(x.Value)).ToList();
        if (claims.Count == 0)
        {
            return null;
        }
        return claims.First().Value;
    }

}
