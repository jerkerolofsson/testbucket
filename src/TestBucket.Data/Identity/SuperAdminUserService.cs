using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using TestBucket.Domain.Identity;
using TestBucket.Domain.Identity.Models;
using TestBucket.Domain.Identity.Permissions;
using TestBucket.Domain.Shared;
using TestBucket.Domain.Tenants.Models;

namespace TestBucket.Data.Identity;

/// <summary>
/// Contains sensitive functions not for normal users
/// 
/// All calls to this service should be validated for permission
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

    public async Task<PagedResult<ApplicationUser>> BrowseAsync(string tenantId, int offset, int count)
    {
        var userStore = _serviceProvider.GetRequiredService<IUserStore<ApplicationUser>>();
        var storeContext = userStore.GetType().GetProperty("Context")!.GetValue(userStore) as DbContext;
        var tenantUserStore = new ApplicationUserStore(storeContext!, tenantId);
        tenantUserStore.TenantId = tenantId;

        return await tenantUserStore.BrowseAsync(offset, count);
    }

    private IUserEmailStore<ApplicationUser> GetEmailStore(UserManager<ApplicationUser> userManager, IUserStore<ApplicationUser> userStore)
    {
        if (!userManager.SupportsUserEmail)
        {
            throw new NotSupportedException("The default UI requires a user store with email support.");
        }
        return (IUserEmailStore<ApplicationUser>)userStore;
    }


    public async Task UpdateUserAsync(string tenantId, ApplicationUser user)
    {
        using var scope = _serviceProvider.CreateScope();
        var dbContextFactory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<ApplicationDbContext>>();
        using var dbContext = await dbContextFactory.CreateDbContextAsync();

        var userStore = new ApplicationUserStore(dbContext, tenantId);
        await userStore.UpdateAsync(user);
    }

    public async Task<IdentityResult> RegisterAndConfirmUserAsync(string tenantId, string email, string password)
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
            return result;
        }

        await userManager.AddClaimAsync(user, new Claim("tenant", tenantId));

        if (!(await userManager.IsEmailConfirmedAsync(user)))
        {
            _logger.LogWarning("Default user's email is not confirmed");
            return IdentityResult.Failed([new IdentityError() { Description = "Email could not be confirmed" }]);
        }
        if (!(await confirmation.IsConfirmedAsync(userManager, user)))
        {
            _logger.LogWarning("Default user's account is not confirmed");
            return IdentityResult.Failed([new IdentityError() { Description = "Account could not be confirmed" }]);
        }
        return IdentityResult.Success;
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
    public async Task UnassignRoleAsync(string tenantId, string email, string roleName)
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
            if (roles.Contains(roleName))
            {
                _logger.LogInformation("Removing role '{roleName}' from '{email}' for tenant '{tenantId}'", roleName, email, tenantId);
                await userManager.RemoveFromRoleAsync(user, roleName);
            }
        }
    }

    public async Task AddRoleAsync(string tenantId, string roleName)
    {
        var roleManager = _serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        var exists = await roleManager.RoleExistsAsync(roleName);
        if (!exists)
        {
            var role = new IdentityRole
            {
                Name = roleName,
                NormalizedName = roleName
            };
            var result = await roleManager.CreateAsync(role);
            if (!result.Succeeded)
            {
                throw new Exception(result.Errors.FirstOrDefault()?.Description ?? "Unknown error");
            }
        }
    }

    public async Task RemoveRoleAsync(string tenantId, string roleName)
    {
        var roleManager = _serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var role = await roleManager.FindByNameAsync(roleName);
        if (role is not null)
        {
            var result = await roleManager.DeleteAsync(role);
            if (!result.Succeeded)
            {
                throw new Exception(result.Errors.FirstOrDefault()?.Description ?? "Unknown error");
            }
        }
    }

    public async Task<IReadOnlyList<string>> GetUserRoleNamesAsync(ApplicationUser user)
    {
        var userManager = _serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        return (await userManager.GetRolesAsync(user)).AsReadOnly();
    }

    public async Task<IReadOnlyList<string>> GetRoleNamesAsync(string tenantId)
    {
        var roleManager = _serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        return await roleManager.Roles.Where(x => x.Name != null).Select(x => x.Name!).ToListAsync();
    }
}
