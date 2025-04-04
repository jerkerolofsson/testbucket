﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Storage;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

using OpenTelemetry.Trace;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using Microsoft.Extensions.Configuration;
using System.Threading;
using TestBucket.Domain.Identity.Models;
using TestBucket.Domain.Identity;
using TestBucket.Domain.Tenants;
using TestBucket.Domain.Settings.Models;
using TestBucket.Domain.Settings;
using TestBucket.Domain.Tenants.Models;
using TestBucket.Domain.Identity.Permissions;


namespace TestBucket.Data.Migrations;
public class MigrationService(IServiceProvider serviceProvider, ILogger<MigrationService> logger, SeedConfiguration seedConfiguration) : BackgroundService
{
    public const string ActivitySourceName = "Migrations";
    private static readonly ActivitySource s_activitySource = new(ActivitySourceName);
    private static readonly SemaphoreSlim s_lock = new(1);


    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        using var activity = s_activitySource.StartActivity("Migrating database", ActivityKind.Client);

        try
        {

            await s_lock.WaitAsync();
            try
            {
                using var scope = serviceProvider.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                await EnsureDatabaseAsync(dbContext, cancellationToken);
                await RunMigrationAsync(dbContext, cancellationToken);
            }
            finally
            {
                s_lock.Release();
            }
            try
            {
                await SeedDataAsync(cancellationToken);
            }
            finally
            {
                s_lock.Release();
            }
        }
        catch (Exception ex)
        {
            activity?.AddException(ex);
            throw;
        }
    }

    private static async Task EnsureDatabaseAsync(ApplicationDbContext dbContext, CancellationToken cancellationToken)
    {
        var dbCreator = dbContext.GetService<IRelationalDatabaseCreator>();

        var strategy = dbContext.Database.CreateExecutionStrategy();
        await strategy.ExecuteAsync(async () =>
        {
            // Create the database if it does not exist.
            // Do this first so there is then a database to start a transaction against.
            if (!await dbCreator.ExistsAsync(cancellationToken))
            {
                await dbCreator.CreateAsync(cancellationToken);
            }
        });
    }

    private static async Task RunMigrationAsync(ApplicationDbContext dbContext, CancellationToken cancellationToken)
    {
        var strategy = dbContext.Database.CreateExecutionStrategy();
        await strategy.ExecuteAsync(async () =>
        {
            // Run migration in a transaction to avoid partial migration if it fails.
            //await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);

            await dbContext.Database.MigrateAsync(cancellationToken);
            //await transaction.CommitAsync(cancellationToken);
        });
    }

    private IUserEmailStore<ApplicationUser> GetEmailStore(UserManager<ApplicationUser> userManager, IUserStore<ApplicationUser> userStore)
    {
        if (!userManager.SupportsUserEmail)
        {
            throw new NotSupportedException("The default UI requires a user store with email support.");
        }
        return (IUserEmailStore<ApplicationUser>)userStore;
    }

    private async Task SeedDataAsync(CancellationToken cancellationToken)
    {

        string superAdminUserEmail = seedConfiguration.Email ?? "admin@admin.com";
        string adminUserPassword = seedConfiguration.Password ?? "Password123!";
        string adminTenant = seedConfiguration.Tenant ?? "admin";
        string adminApiKey = seedConfiguration.AccessToken ?? Guid.NewGuid().ToString();

        using var scope = serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        // Update settings
        await SeedSettingsAsync(adminTenant, scope);

        var strategy = dbContext.Database.CreateExecutionStrategy();
        await strategy.ExecuteAsync(async () =>
        {
            // Seed the database

            try
            {
                await CreateDefaultRolesAsync(dbContext, scope);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to create default user");
            }

            try
            {
                using var scope = serviceProvider.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                await CreateTenantAsync(scope, adminTenant, adminTenant);
                await CreateDefaultTenantAdminUserAsync(dbContext, scope, adminTenant, superAdminUserEmail, adminUserPassword);
                await CreateApiKeyAsync(dbContext, scope, adminTenant, superAdminUserEmail, adminApiKey);

                try
                {
                    await CreateDefaultRolePermissionsAsync(dbContext, adminTenant, scope);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Failed to create default user");
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to create default user");
            }

            try
            {
                using var scope = serviceProvider.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                await AssignRolesAsync(dbContext, scope, adminTenant, superAdminUserEmail, Roles.SUPERADMIN);
                await AssignRolesAsync(dbContext, scope, adminTenant, superAdminUserEmail, Roles.ADMIN);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to assign roles");
            }
        });
    }

    private async Task SeedSettingsAsync(string adminTenant, IServiceScope scope)
    {
        var settingsProvider = scope.ServiceProvider.GetRequiredService<ISettingsProvider>();
        var settings = await settingsProvider.LoadGlobalSettingsAsync();
        bool changed = false;
        string? jwtSymmetricKey = seedConfiguration.SymmetricKey;
        string? jwtIssuer = seedConfiguration.Issuer;
        string? jwtAudience = seedConfiguration.Audience;

        // Default values if not set in environment variables
        if (settings.SymmetricJwtKey is null)
        {
            settings.SymmetricJwtKey = Guid.NewGuid().ToString();
            changed = true;
        }
        if (settings.JwtIssuer is null)
        {
            settings.JwtIssuer = "testbucket";
            changed = true;
        }
        if (settings.JwtAudience is null)
        {
            settings.JwtAudience = "testbucket";
            changed = true;
        }

        // Update settings if provided
        if (settings.DefaultTenant != adminTenant)
        {
            settings.DefaultTenant = adminTenant;
            changed = true;
        }
        if (settings.SymmetricJwtKey != jwtSymmetricKey && jwtSymmetricKey is not null)
        {
            settings.SymmetricJwtKey = jwtSymmetricKey;
            changed = true;
        }
        if (settings.JwtIssuer != jwtIssuer)
        {
            settings.JwtIssuer = jwtIssuer;
            changed = true;
        }
        if (settings.JwtAudience != jwtAudience)
        {
            settings.JwtAudience = jwtAudience;
            changed = true;
        }
        if (changed)
        {
            await settingsProvider.SaveGlobalSettingsAsync(settings);
        }
    }

    private async Task AssignRolesAsync(ApplicationDbContext dbContext, IServiceScope scope, string tenantId, string email, string roleName)
    {
        var superAdminUserService = scope.ServiceProvider.GetRequiredService<ISuperAdminUserService>();
        await superAdminUserService.AssignRoleAsync(tenantId, email, roleName);

        //var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        //var userStore = scope.ServiceProvider.GetRequiredService<IUserStore<ApplicationUser>>();
        //var storeContext = userStore.GetType().GetProperty("Context")!.GetValue(userStore) as DbContext;
        //var tenantUserStore = new ApplicationUserStore(storeContext!, tenantId);

        //var normalizedEmail = userManager.NormalizeEmail(email);
        //var user = await tenantUserStore.FindByEmailAsync(normalizedEmail, CancellationToken.None);

        //if (user is null)
        //{
        //    logger.LogError("Failed to assign role '{roleName}' to '{email}' as user was not found for tenant '{tenantId}'", roleName, email, tenantId);
        //}
        //else
        //{
        //    var roles = await userManager.GetRolesAsync(user);
        //    if (!roles.Contains(roleName))
        //    {
        //        logger.LogInformation("Adding role '{roleName}' to '{email}' for tenant '{tenantId}'", roleName, email, tenantId);
        //        await userManager.AddToRoleAsync(user, roleName);
        //    }
        //}
    }

    private async Task CreateDefaultRolesAsync(ApplicationDbContext dbContext, IServiceScope scope)
    {
        await CreateRoleIfNotExistsAsync(scope, Roles.SUPERADMIN); // Can create new tenants
        await CreateRoleIfNotExistsAsync(scope, Roles.ADMIN); // Tenant admin
        await CreateRoleIfNotExistsAsync(scope, Roles.REGULAR_USER);
        await CreateRoleIfNotExistsAsync(scope, Roles.READ_ONLY);
    }

    private async Task CreateDefaultRolePermissionsAsync(ApplicationDbContext dbContext, string tenantId, IServiceScope scope)
    {
        var role = Roles.SUPERADMIN;
        var has = await dbContext.RolePermissions.Where(x => x.Role == role).AnyAsync();
        if(!has)
        {
            dbContext.RolePermissions.Add(new RolePermission { Role = role, TenantId = tenantId, Level = PermissionLevel.All, Entity = PermissionEntityType.Tenant });
            dbContext.RolePermissions.Add(new RolePermission { Role = role, TenantId = tenantId, Level = PermissionLevel.All, Entity = PermissionEntityType.User });
            dbContext.RolePermissions.Add(new RolePermission { Role = role, TenantId = tenantId, Level = PermissionLevel.All, Entity = PermissionEntityType.Project });
            dbContext.RolePermissions.Add(new RolePermission { Role = role, TenantId = tenantId, Level = PermissionLevel.All, Entity = PermissionEntityType.TestSuite });
            dbContext.RolePermissions.Add(new RolePermission { Role = role, TenantId = tenantId, Level = PermissionLevel.All, Entity = PermissionEntityType.TestCase });
            dbContext.RolePermissions.Add(new RolePermission { Role = role, TenantId = tenantId, Level = PermissionLevel.All, Entity = PermissionEntityType.Requirement });
            dbContext.RolePermissions.Add(new RolePermission { Role = role, TenantId = tenantId, Level = PermissionLevel.All, Entity = PermissionEntityType.RequirementSpecification });
            dbContext.RolePermissions.Add(new RolePermission { Role = role, TenantId = tenantId, Level = PermissionLevel.All, Entity = PermissionEntityType.TestRun });
            dbContext.RolePermissions.Add(new RolePermission { Role = role, TenantId = tenantId, Level = PermissionLevel.All, Entity = PermissionEntityType.TestCaseRun });
            await dbContext.SaveChangesAsync();
        }

        role = Roles.ADMIN;
        has = await dbContext.RolePermissions.Where(x => x.Role == role).AnyAsync();
        if (!has)
        {
            dbContext.RolePermissions.Add(new RolePermission { Role = role, TenantId = tenantId, Level = PermissionLevel.None, Entity = PermissionEntityType.Tenant });
            dbContext.RolePermissions.Add(new RolePermission { Role = role, TenantId = tenantId, Level = PermissionLevel.All, Entity = PermissionEntityType.User });
            dbContext.RolePermissions.Add(new RolePermission { Role = role, TenantId = tenantId, Level = PermissionLevel.All, Entity = PermissionEntityType.Project });
            dbContext.RolePermissions.Add(new RolePermission { Role = role, TenantId = tenantId, Level = PermissionLevel.All, Entity = PermissionEntityType.TestSuite });
            dbContext.RolePermissions.Add(new RolePermission { Role = role, TenantId = tenantId, Level = PermissionLevel.All, Entity = PermissionEntityType.TestCase });
            dbContext.RolePermissions.Add(new RolePermission { Role = role, TenantId = tenantId, Level = PermissionLevel.All, Entity = PermissionEntityType.Requirement });
            dbContext.RolePermissions.Add(new RolePermission { Role = role, TenantId = tenantId, Level = PermissionLevel.All, Entity = PermissionEntityType.RequirementSpecification });
            dbContext.RolePermissions.Add(new RolePermission { Role = role, TenantId = tenantId, Level = PermissionLevel.All, Entity = PermissionEntityType.TestRun });
            dbContext.RolePermissions.Add(new RolePermission { Role = role, TenantId = tenantId, Level = PermissionLevel.All, Entity = PermissionEntityType.TestCaseRun });
            await dbContext.SaveChangesAsync();
        }

        role = Roles.REGULAR_USER;
        has = await dbContext.RolePermissions.Where(x => x.Role == role).AnyAsync();
        if (!has)
        {
            dbContext.RolePermissions.Add(new RolePermission { Role = role, TenantId = tenantId, Level = PermissionLevel.None, Entity = PermissionEntityType.Tenant });
            dbContext.RolePermissions.Add(new RolePermission { Role = role, TenantId = tenantId, Level = PermissionLevel.None, Entity = PermissionEntityType.Project });
            dbContext.RolePermissions.Add(new RolePermission { Role = role, TenantId = tenantId, Level = PermissionLevel.Read, Entity = PermissionEntityType.User });
            dbContext.RolePermissions.Add(new RolePermission { Role = role, TenantId = tenantId, Level = PermissionLevel.All, Entity = PermissionEntityType.TestSuite });
            dbContext.RolePermissions.Add(new RolePermission { Role = role, TenantId = tenantId, Level = PermissionLevel.All, Entity = PermissionEntityType.TestCase });
            dbContext.RolePermissions.Add(new RolePermission { Role = role, TenantId = tenantId, Level = PermissionLevel.All, Entity = PermissionEntityType.Requirement });
            dbContext.RolePermissions.Add(new RolePermission { Role = role, TenantId = tenantId, Level = PermissionLevel.All, Entity = PermissionEntityType.RequirementSpecification });
            dbContext.RolePermissions.Add(new RolePermission { Role = role, TenantId = tenantId, Level = PermissionLevel.All, Entity = PermissionEntityType.TestRun });
            dbContext.RolePermissions.Add(new RolePermission { Role = role, TenantId = tenantId, Level = PermissionLevel.All, Entity = PermissionEntityType.TestCaseRun });
            await dbContext.SaveChangesAsync();
        }

        role = Roles.READ_ONLY;
        has = await dbContext.RolePermissions.Where(x => x.Role == role).AnyAsync();
        if (!has)
        {
            dbContext.RolePermissions.Add(new RolePermission { Role = role, TenantId = tenantId, Level = PermissionLevel.None, Entity = PermissionEntityType.Tenant });
            dbContext.RolePermissions.Add(new RolePermission { Role = role, TenantId = tenantId, Level = PermissionLevel.None, Entity = PermissionEntityType.Project });
            dbContext.RolePermissions.Add(new RolePermission { Role = role, TenantId = tenantId, Level = PermissionLevel.Read, Entity = PermissionEntityType.User });
            dbContext.RolePermissions.Add(new RolePermission { Role = role, TenantId = tenantId, Level = PermissionLevel.Read, Entity = PermissionEntityType.TestSuite });
            dbContext.RolePermissions.Add(new RolePermission { Role = role, TenantId = tenantId, Level = PermissionLevel.Read, Entity = PermissionEntityType.TestCase });
            dbContext.RolePermissions.Add(new RolePermission { Role = role, TenantId = tenantId, Level = PermissionLevel.Read, Entity = PermissionEntityType.Requirement });
            dbContext.RolePermissions.Add(new RolePermission { Role = role, TenantId = tenantId, Level = PermissionLevel.Read, Entity = PermissionEntityType.RequirementSpecification });
            dbContext.RolePermissions.Add(new RolePermission { Role = role, TenantId = tenantId, Level = PermissionLevel.Read, Entity = PermissionEntityType.TestRun });
            dbContext.RolePermissions.Add(new RolePermission { Role = role, TenantId = tenantId, Level = PermissionLevel.Read, Entity = PermissionEntityType.TestCaseRun });
            await dbContext.SaveChangesAsync();
        }

        //await yield;
        //await CreateRoleIfNotExistsAsync(scope, Roles.SUPERADMIN); // Can create new tenants
        //await CreateRoleIfNotExistsAsync(scope, Roles.ADMIN); // Tenant admin
        //await CreateRoleIfNotExistsAsync(scope, Roles.REGULAR_USER); // Tenant admin
    }

    private async Task CreateRoleIfNotExistsAsync(IServiceScope scope, string roleName)
    {
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

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
                logger.LogError("Failed to create role: {roleName}", roleName);
            }
        }
    }

    private async Task CreateTenantAsync(IServiceScope scope, string name, string tenantId)
    {
        var repo = scope.ServiceProvider.GetRequiredService<ITenantRepository>();
        if (!await repo.ExistsAsync(tenantId))
        {
            await repo.CreateAsync(name, tenantId);
        }
    }

    private async Task CreateApiKeyAsync(ApplicationDbContext dbContext, IServiceScope scope, string tenantId, string email, string key)
    {
        var user = await dbContext.Users.Where(x => x.TenantId == tenantId && x.Email == email).Include(x => x.ApplicationUserApiKeys).FirstOrDefaultAsync();
        if (user?.ApplicationUserApiKeys is not null)
        {
            string name = "Initial DB seeding API key";
            var apiKey = await dbContext.ApiKeys.Where(x => x.ApplicationUserId == user.Id && x.Name == name && x.TenantId == tenantId).FirstOrDefaultAsync();
            if (apiKey is null)
            {
                apiKey = new ApplicationUserApiKey
                {
                    Expiry = DateTimeOffset.UtcNow.AddDays(365),
                    ApplicationUserId = user.Id,
                    Key = key,
                    Name = name,
                    TenantId = tenantId
                };
                dbContext.ApiKeys.Add(apiKey);
                await dbContext.SaveChangesAsync();
            }
            else
            {
                var changed = false;
                if (apiKey.Key != key)
                {
                    changed = true;
                    apiKey.Key = key;
                }
                if (apiKey.Expiry > DateTimeOffset.UtcNow.AddDays(30))
                {
                    changed = true;
                    apiKey.Expiry = DateTimeOffset.UtcNow.AddDays(365);
                }

                if (changed)
                {
                    dbContext.ApiKeys.Update(apiKey);
                    await dbContext.SaveChangesAsync();
                }
            }
        }
    }

    /// <summary>
    /// Creates a new tenant and an admin user
    /// </summary>
    /// <param name="dbContext"></param>
    /// <param name="scope"></param>
    /// <param name="tenantId"></param>
    /// <param name="email"></param>
    /// <returns></returns>
    private async Task CreateDefaultTenantAdminUserAsync(ApplicationDbContext dbContext, IServiceScope scope, string tenantId, string email, string password)
    {
        var hasUser = await dbContext.Users.AsNoTracking().Where(x => x.TenantId == tenantId && x.Email == email).AnyAsync();
        if (!hasUser)
        {
            logger.LogInformation("Creating default user: {defaultUser} for default tenant {defaultTenantId}", email, tenantId);

            var superAdminUserService = scope.ServiceProvider.GetRequiredService<ISuperAdminUserService>();
            await superAdminUserService.RegisterAndConfirmUserAsync(tenantId, email, password);

            //var user = new ApplicationUser() { TenantId = tenantId };

            //var username = email;

            //var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            //var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            //var userStore = scope.ServiceProvider.GetRequiredService<IUserStore<ApplicationUser>>();
            //var confirmation = scope.ServiceProvider.GetRequiredService<IUserConfirmation<ApplicationUser>>();
            //await userManager.SetUserNameAsync(user, username);

            //var emailStore = GetEmailStore(userManager, userStore);
            //await emailStore.SetEmailAsync(user, (string)email, CancellationToken.None);

            //var result = await userManager.CreateAsync(user, password);
            //if (result.Succeeded)
            //{
            //    var code = await userManager.GenerateEmailConfirmationTokenAsync(user);
            //    await userManager.ConfirmEmailAsync(user, code);
            //}

            //await userManager.AddClaimAsync(user, new Claim("tenant", tenantId));

            //if (!(await userManager.IsEmailConfirmedAsync(user)))
            //{
            //    logger.LogWarning("Default user's email is not confirmed");
            //}
            //if (!(await confirmation.IsConfirmedAsync(userManager, user)))
            //{
            //    logger.LogWarning("Default user's account is not confirmed");
            //}

            //await dbContext.SaveChangesAsync();
        }

    }
}
