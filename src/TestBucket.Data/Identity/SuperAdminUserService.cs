using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using TestBucket.Data.Identity.Models;

namespace TestBucket.Data.Identity;

/// <summary>
/// Contains sensitive functions not for normal users
/// 
/// All calls to this service should be validated with SUPERUSER permissions
/// </summary>
internal class SuperAdminUserService : ISuperAdminUserService
{
    private readonly ILogger<SuperAdminUserService> _logger;
    private readonly IServiceProvider _serviceProvider;

    public SuperAdminUserService(ILogger<SuperAdminUserService> logger, IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    public async Task AssignRoleAsync(string tenantId, string email, string roleName)
    {
        var userManager = _serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var userStore = _serviceProvider.GetRequiredService<IUserStore<ApplicationUser>>();
        var storeContext = userStore.GetType().GetProperty("Context")!.GetValue(userStore) as DbContext;
        var tenantUserStore = new ApplicationUserStore(storeContext!, tenantId);

        var normalizedEmail = userManager.NormalizeEmail(email);
        var user = await tenantUserStore.FindByEmailAsync(normalizedEmail, CancellationToken.None);

        if (user is null)
        {
            _logger.LogError("Failed to assign role '{roleName}' to '{email}' as user was not found for tenant '{tenantId}'", roleName, email, tenantId);
        }
        else
        {
            var roles = await userManager.GetRolesAsync(user);
            if (!roles.Contains(roleName))
            {
                _logger.LogInformation("Adding role '{roleName}' to '{email}' for tenant '{tenantId}'", roleName, email, tenantId);
                await userManager.AddToRoleAsync(user, roleName);
            }
        }
    }
    private IUserEmailStore<ApplicationUser> GetEmailStore(UserManager<ApplicationUser> userManager, IUserStore<ApplicationUser> userStore)
    {
        if (!userManager.SupportsUserEmail)
        {
            throw new NotSupportedException("The default UI requires a user store with email support.");
        }
        return (IUserEmailStore<ApplicationUser>)userStore;
    }
    public async Task<bool> RegisterAndConfirmUserAsync(string tenantId, string email, string password)
    {
        var user = new ApplicationUser() { TenantId = tenantId };

        var username = email;

        using var scope = _serviceProvider.CreateScope();
        var dbContextFactory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<ApplicationDbContext>>();
        using var dbContext = await dbContextFactory.CreateDbContextAsync();

        //var userStore = ActivatorUtilities.CreateInstance<ApplicationUserStore>(scope.ServiceProvider, new object[] { dbContext, tenantId });
        var userStore = new ApplicationUserStore(dbContext, tenantId);
        //await userStore.CreateAsync(user);

        var userManager = ActivatorUtilities.CreateInstance<UserManager<ApplicationUser>>(scope.ServiceProvider, [userStore]);

        //var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        //var userStore = scope.ServiceProvider.GetRequiredService<IUserStore<ApplicationUser>>();
        var confirmation = scope.ServiceProvider.GetRequiredService<IUserConfirmation<ApplicationUser>>();
        await userManager.SetUserNameAsync(user, username);

        var emailStore = GetEmailStore(userManager, userStore);
        await emailStore.SetEmailAsync(user, (string)email, CancellationToken.None);

        var result = await userManager.CreateAsync(user, password);
        if (result.Succeeded)
        {
            var code = await userManager.GenerateEmailConfirmationTokenAsync(user);
            await userManager.ConfirmEmailAsync(user, code);
        }
        else
        {
            return false;
        }

        await userManager.AddClaimAsync(user, new Claim("tenant", tenantId));

        if (!(await userManager.IsEmailConfirmedAsync(user)))
        {
            _logger.LogWarning("Default user's email is not confirmed");
            return false;
        }
        if (!(await confirmation.IsConfirmedAsync(userManager, user)))
        {
            _logger.LogWarning("Default user's account is not confirmed");
            return false;
        }
        return true;
    }
}
