using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Components.Authorization;

using TestBucket.Domain.Identity;

namespace TestBucket.Components.Shared;
public class SuperAdminGuard
{
    private readonly AuthenticationStateProvider _authenticationStateProvider;

    public SuperAdminGuard(AuthenticationStateProvider authenticationStateProvider)
    {
        _authenticationStateProvider = authenticationStateProvider;
    }

    /// <summary>
    /// Throws an exception if the user is not a super admin user
    /// </summary>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public async Task GuardAsync()
    {
        var authState = await _authenticationStateProvider.GetAuthenticationStateAsync();
        var isSuperAdmin = authState.User.IsInRole(Roles.SUPERADMIN);
        if (!isSuperAdmin)
        {
            throw new InvalidOperationException("This method can only be called by users with the 'SUPERADMIN' role");
        }
    }
}
