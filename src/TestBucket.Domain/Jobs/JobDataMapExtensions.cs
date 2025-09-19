using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

using Quartz;

namespace TestBucket.Domain.Jobs;

/// <summary>
/// Extensions when creating jobs
/// </summary>
public static class JobDataMapExtensions
{
    /// <summary>
    /// Adds user information to the job data map
    /// </summary>
    /// <param name="map"></param>
    /// <param name="principal"></param>
    /// <returns></returns>
    public static JobDataMap AddUser(this JobDataMap map, ClaimsPrincipal principal)
    {
        var tenantId = principal.GetTenantIdOrThrow();
        map.Add("UserName", principal.Identity?.Name ?? "system");
        map.Add("TenantId", tenantId);
        return map;
    }

    public static JobDataMap AddAsJson<T>(this JobDataMap map, string key, T data)
    {
        var json = JsonSerializer.Serialize(data);
        map.Add(key, json);
        return map;
    }

    public static T GetFromJson<T>(this JobDataMap map, string key)
    {
        if (!map.ContainsKey(key))
        {
            throw new KeyNotFoundException($"Key '{key}' not found in job data map");
        }
        var json = map.GetString(key);
        if (json is null)
        {
            throw new InvalidOperationException($"Key '{key}' is null in job data map");
        }
        return JsonSerializer.Deserialize<T>(json) ?? throw new InvalidOperationException($"Key '{key}' could not be deserialized from job data map");
    }
}
