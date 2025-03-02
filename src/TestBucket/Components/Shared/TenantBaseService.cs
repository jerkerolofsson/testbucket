using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore;

namespace TestBucket.Components.Shared;
internal abstract class TenantBaseService
{
    private readonly AuthenticationStateProvider _authenticationStateProvider;
    private string? _tenantId;

    public TenantBaseService(AuthenticationStateProvider authenticationStateProvider)
    {
        _authenticationStateProvider = authenticationStateProvider;
    }

    /// <summary>
    /// Returns the tenant from the authenticated users claims
    /// </summary>
    /// <returns></returns>
    protected async Task<string> GetTenantIdAsync()
    {
        if (_tenantId is not null)
        {
            return _tenantId;
        }
        var authState = await _authenticationStateProvider.GetAuthenticationStateAsync();
        _tenantId = authState.User.Claims.Where(x => x.Type == "tenant").Select(x => x.Value).FirstOrDefault();
        return _tenantId ?? throw new InvalidDataException("User is not authenticated or is missing the tenant claim");
    }

    protected async Task<string?> GetUserNameAsync()
    {
        var authState = await _authenticationStateProvider.GetAuthenticationStateAsync();
        return authState.User.Identity?.Name;
    }
}
