using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace TestBucket.Identity;

public static class TenantResolver
{
    public static string? ResolveTenantId(IHttpContextAccessor contextAccessor)
    {
        string? path = contextAccessor.HttpContext?.Request?.Path ?? "admin";

        return ResolveTenantIdFromPath(path);
    }

    public static string ResolveTenantIdFromPath(string? path)
    {
        path ??= "admin";
        var pathItems = path.Trim('/').Split('/');
        var tenantId = pathItems[0].Split('?')[0].Split('#')[0];
        return tenantId;
    }

    public static string? ResolveTenantIdFromUrl(string url)
    {
        if(Uri.TryCreate(url, UriKind.Absolute, out var uri))
        {
            return ResolveTenantIdFromPath(uri.PathAndQuery);
        }
        return "admin";
    }
}