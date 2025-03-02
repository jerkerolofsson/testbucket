using Microsoft.AspNetCore.Identity;

using TestBucket.Components.Tenants;
using TestBucket.Domain.Identity.Models;

namespace TestBucket.Components.Account;

internal sealed class IdentityUserAccessor(UserManager<ApplicationUser> userManager, IdentityRedirectManager redirectManager)
{
    public async Task<string> GetRequiredTenantId(HttpContext context)
    {
        var user = await GetRequiredUserAsync(context);
        return user.TenantId;
    }

    public async Task<ApplicationUser> GetRequiredUserAsync(HttpContext context)
    {
        var user = await userManager.GetUserAsync(context.User);

        string? tenantId = TenantResolver.ResolveTenantIdFromPath(context.Request.Path);
        if (user is null)
        {
            redirectManager.RedirectToWithStatus($"/{tenantId}/Account/InvalidUser", $"Error: Unable to load user with ID '{userManager.GetUserId(context.User)}'.", context);
        }
        else if(user.TenantId != tenantId)
        {
            redirectManager.RedirectToWithStatus($"/{tenantId}/Account/InvalidUser", $"Error: Invalid tenant for user '{userManager.GetUserId(context.User)}'.", context);
        }

        return user;
    }
}
