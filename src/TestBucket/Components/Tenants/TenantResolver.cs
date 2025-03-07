using TestBucket.Domain.Settings;

namespace TestBucket.Components.Tenants;

public class TenantResolver
{
    private readonly ISettingsProvider _settingsProvider;

    public TenantResolver(ISettingsProvider settingsProvider)
    {
        _settingsProvider = settingsProvider;
    }

    public async Task<string> ResolveTenantIdFromUrlAsync(string url)
    {
        var tenantId = ResolveTenantIdFromUrl(url);
        if(tenantId is not null)
        {
            return tenantId;
        }

        var settings = await _settingsProvider.LoadGlobalSettingsAsync();
        return settings.DefaultTenant;
    }

    public static string? ResolveTenantId(IHttpContextAccessor contextAccessor)
    {
        string? path = contextAccessor.HttpContext?.Request?.Path ?? "admin";
        return ResolveTenantIdFromPath(path);
    }

    public static string? ResolveTenantIdFromPath(string? path)
    {
        if (path is null)
        {
            return null;
        }
        path = path.Split('?')[0].Split('#')[0];

        var pathItems = path.Trim('/').Split('/');
        var tenantId = pathItems[0];
        if(tenantId == "api" && pathItems.Length > 1)
        {
            tenantId = pathItems[1];
        }
        return tenantId;
    }

    public static string? ResolveTenantIdFromUrl(string url)
    {
        if (Uri.TryCreate(url, UriKind.Absolute, out var uri))
        {
            return ResolveTenantIdFromPath(uri.PathAndQuery);
        }
        return null;
    }
}