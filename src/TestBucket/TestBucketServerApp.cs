using Blazored.LocalStorage;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using MudBlazor.Services;
using MudExtensions.Services;
using TestBucket.Components.Account;
using TestBucket.Components.Automation;
using TestBucket.Components.Environments.Services;
using TestBucket.Components.Layout.Controls;
using TestBucket.Components.Projects;
using TestBucket.Components.Requirements.Commands;
using TestBucket.Components.Requirements.Services;
using TestBucket.Components.Settings.ApiKeys;
using TestBucket.Components.Settings.Roles;
using TestBucket.Components.Shared.Fields;
using TestBucket.Components.Shared.Themeing;
using TestBucket.Components.Teams;
using TestBucket.Components.TestAccounts.Services;
using TestBucket.Components.TestResources.Services;
using TestBucket.Components.Tests.Services;
using TestBucket.Components.Tests.TestCases.Commands;
using TestBucket.Components.Tests.TestCases.Services;
using TestBucket.Components.Tests.TestRuns.LinkIssue;
using TestBucket.Components.Tests.TestSuites.Commands;
using TestBucket.Components.Tests.TestSuites.Services;
using TestBucket.Components.Uploads.Services;
using TestBucket.Components.Users.Services;
using TestBucket.Components.Users;
using TestBucket.Data.Migrations;
using TestBucket.Domain.ApiKeys;
using TestBucket.Domain.Settings.Models;
using TestBucket.Identity;
using Toolbelt.Blazor.Extensions.DependencyInjection;
using TestBucket.Components;
using TestBucket.Domain.Commands;
using TestBucket.Components.Tests.Requiremnts.Commands;
using TestBucket.Components.Tests.Requirements.Commands;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace TestBucket;

public static class TestBucketServerApp
{
    public static WebApplication CreateApp(this WebApplicationBuilder builder)
    {

        builder.AddServiceDefaults();

        // Add services to the container.
        builder.Services.AddRazorComponents()
            .AddInteractiveServerComponents()
            .AddHubOptions(hubOptions =>
            {
                // We set this to quite large to handle capture screenshots etc
                hubOptions.MaximumReceiveMessageSize = 2_000_000;
                //hubOptions.ClientTimeoutInterval = TimeSpan.FromSeconds(10);
            });

        builder.Services.AddBlazoredLocalStorage();
        builder.Services.AddMemoryCache();

        builder.Services.AddSingleton<RoleLocalizer>();
        builder.Services.AddLocalization(options =>
        {
        });

        builder.Services.AddCascadingAuthenticationState();
        builder.Services.AddScoped<IdentityUserAccessor>();
        builder.Services.AddScoped<IdentityRedirectManager>();
        builder.Services.AddScoped<AuthenticationStateProvider, IdentityRevalidatingAuthenticationStateProvider>();

        builder.Services.AddAuthentication("ApiKey").AddScheme<ApiKeyAuthenticationOptions, ApiKeyAuthenticationHandler>("ApiKey", null);

        builder.Services.AddAuthorization(options =>
        {
            options.AddPolicy("ApiKeyOrBearer", policy =>
            {
                policy.AddAuthenticationSchemes(IdentityConstants.ApplicationScheme, "ApiKey");
                policy.RequireAuthenticatedUser();
            });
        });
        builder.Services.AddAuthentication(options =>
        {
            options.DefaultScheme = IdentityConstants.ApplicationScheme;
            options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
        })
            .AddIdentityCookies();
        string? connectionString = null;
        builder.AddNpgsqlDbContext<ApplicationDbContext>("testbucketdb", configureSettings =>
        {
            connectionString = configureSettings.ConnectionString;
        },
        dbContextBuilder =>
        {
            dbContextBuilder.UseNpgsql(builder =>
            {
                builder.ConfigureDataSource(dataSource =>
                {
                    dataSource.EnableDynamicJson();
                });
            });

            if (builder.Environment.IsDevelopment())
            {
                dbContextBuilder.EnableSensitiveDataLogging();
                dbContextBuilder.EnableDetailedErrors();
            }
        });
        builder.Services.AddDbContextFactory<ApplicationDbContext>();

        builder.Services.AddHealthChecks().AddCheck("self", () =>
        {
            if(MigrationService.ReadyFlag)
            {
                return HealthCheckResult.Healthy();
            }
            return HealthCheckResult.Unhealthy();
            
        }, ["live", "ready"]);

        var localhostOrigin = "localhost";
        builder.Services.AddCors(options =>
        {
            options.AddPolicy(name: localhostOrigin,
                              policy =>
                              {
                                  policy.WithOrigins("https://localhost", "https://127.0.0.1", "https://::1");
                                  policy.AllowAnyHeader();
                                  policy.AllowAnyMethod();
                                  policy.AllowCredentials();
                              });
        });

        var seedConfiguration = new SeedConfiguration
        {
            Tenant = Environment.GetEnvironmentVariable(TestBucketEnvironmentVariables.TB_DEFAULT_TENANT),
            Email = Environment.GetEnvironmentVariable(TestBucketEnvironmentVariables.TB_ADMIN_USER),
            SymmetricKey = Environment.GetEnvironmentVariable(TestBucketEnvironmentVariables.TB_JWT_SYMMETRIC_KEY),
            Issuer = Environment.GetEnvironmentVariable(TestBucketEnvironmentVariables.TB_JWT_ISS),
            Audience = Environment.GetEnvironmentVariable(TestBucketEnvironmentVariables.TB_JWT_AUD),
            AccessToken = Environment.GetEnvironmentVariable(TestBucketEnvironmentVariables.TB_ADMIN_ACCESS_TOKEN),
            PublicEndpointUrl = Environment.GetEnvironmentVariable(TestBucketEnvironmentVariables.TB_PUBLIC_ENDPOINT),
            Password = Environment.GetEnvironmentVariable(TestBucketEnvironmentVariables.TB_ADMIN_PASSWORD),
        };
        builder.Services.AddSingleton(seedConfiguration);

        builder.Services.AddDatabaseDeveloperPageExceptionFilter();
        builder.Services.AddHostedService<MigrationService>();

        builder.Services.AddIdentityCore<ApplicationUser>(options =>
        {
            options.SignIn.RequireConfirmedAccount = true;
        })
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddSignInManager()
            .AddDefaultTokenProviders();

        builder.Services.ConfigureApplicationCookie(o =>
        {
            o.Events = new CookieAuthenticationEvents()
            {
                OnRedirectToLogin = (ctx) =>
                {
                    if (ctx.Request.Path.StartsWithSegments("/api") && ctx.Response.StatusCode == 200)
                    {
                        ctx.Response.StatusCode = 401;
                    }

                    return Task.CompletedTask;
                },
                OnRedirectToAccessDenied = (ctx) =>
                {
                    if (ctx.Request.Path.StartsWithSegments("/api") && ctx.Response.StatusCode == 200)
                    {
                        ctx.Response.StatusCode = 403;
                    }

                    return Task.CompletedTask;
                }
            };
        });

        builder.Services.AddScoped<SignInManager<ApplicationUser>, ApplicationSignInManager>();
        builder.Services.AddSingleton<IEmailSender<ApplicationUser>, IdentityNoOpEmailSender>();

        builder.Services.AddScoped<UserApiKeysController>();
        builder.Services.AddScoped<UserRegistrationController>();
        builder.Services.AddScoped<ThemingService>();
        builder.Services.AddScoped<TenantResolver>();
        builder.Services.AddScoped<TenantController>();
        builder.Services.AddScoped<RolesController>();
        builder.Services.AddScoped<TeamController>();
        builder.Services.AddScoped<ProjectController>();
        builder.Services.AddScoped<TestSuiteService>();
        builder.Services.AddScoped<TestRunCreationController>();
        builder.Services.AddScoped<AttachmentsService>();
        builder.Services.AddScoped<AppNavigationManager>();
        builder.Services.AddScoped<TestEnvironmentController>();
        builder.Services.AddScoped<TestResourceController>();
        builder.Services.AddScoped<TestAccountController>();
        builder.Services.AddScoped<PipelineController>();
        builder.Services.AddSingleton<ResourceViewFactory>();
        builder.Services.AddScoped<RequirementBrowser>();
        builder.Services.AddScoped<RequirementEditorController>();
        builder.Services.AddScoped<TestExecutionController>();
        builder.Services.AddScoped<RunnersController>();

        builder.Services.AddScoped<UserPreferencesService>();
        builder.Services.AddScoped<TestBrowser>();
        builder.Services.AddScoped<TestCaseEditorController>();
        builder.Services.AddScoped<UploadService>();
        builder.Services.AddScoped<FieldController>();
        builder.Services.AddScoped<UserController>();

        builder.Services.AddScoped<HotKeysService>();

        // Test suite

        builder.Services.AddScoped<ICommand, RunTestSuiteCommand>();
        builder.Services.AddScoped<ICommand, AddTestSuiteToRunCommand>();


        // Test tree view
        builder.Services.AddScoped<ICommand, BatchTagCommand>();
        builder.Services.AddScoped<ICommand, NewFolderCommand>();
        builder.Services.AddScoped<ICommand, SyncWithActiveDocumentCommand>();

        // Test case
        builder.Services.AddScoped<ICommand, DuplicateTestCommand>();
        builder.Services.AddScoped<ICommand, NewTestCommand>();

        builder.Services.AddScoped<ICommand, LinkIssueCommand>();
        builder.Services.AddScoped<ICommand, DeleteLinkedIssueCommand>();
        builder.Services.AddScoped<ICommand, RefreshLinkedIssueCommand>();

        // requirement
        builder.Services.AddScoped<ICommand, SyncWithActiveRequirementCommand>();
        builder.Services.AddScoped<ICommand, CreateTestCaseFromRequirementCommand>();
        builder.Services.AddScoped<ICommand, DeleteRequirementCommand>();
        builder.Services.AddScoped<ICommand, DeleteRequirementFolderCommand>();
        builder.Services.AddScoped<ICommand, CreateRequirementCommand>();
        builder.Services.AddScoped<ICommand, NewRequirementFolderCommand>();
        builder.Services.AddScoped<ICommand, CreateChildRequirementCommand>();


        builder.Services.AddScoped(typeof(DragAndDropService<>));
        builder.Services.AddDataServices();
        builder.Services.AddDomainServices();

        // Integrations
        builder.Services.AddGitHubExtension();
        builder.Services.AddTrelloExtension();
        builder.Services.AddGitlabExtension();
        builder.Services.AddAzureExtension();

        builder.Services.AddHotKeys2();
        builder.Services.AddMudServices(config =>
        {
        });
        builder.Services.AddMudExtensions();

        builder.Services.AddControllers(options =>
        {
            var policy = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .Build();
            options.Filters.Add(new AuthorizeFilter(policy));
        });

        var app = builder.Build();

        app.MapDefaultEndpoints();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseMigrationsEndPoint();
        }
        else
        {
            app.UseExceptionHandler("/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }
        app.UseAntiforgery();

        if (Environment.GetEnvironmentVariable("TB_HTTPS_REDIRECT") != "disabled")
        {
            app.UseHttpsRedirection();
        }
        app.UseCors(localhostOrigin);


        var requestLocalizationOptions = new RequestLocalizationOptions()
             .AddSupportedCultures(["en-US", "sv-SE"])
             .AddSupportedUICultures(["en-US", "sv-SE"]);
        requestLocalizationOptions.DefaultRequestCulture = new Microsoft.AspNetCore.Localization.RequestCulture("en-US");
        app.UseRequestLocalization(requestLocalizationOptions);

        app.MapStaticAssets();
        app.MapControllers();
        app.MapRazorComponents<App>()
            .AddInteractiveServerRenderMode();

        // Add additional endpoints required by the Identity /Account Razor components.
        app.MapAdditionalIdentityEndpoints();

        return app;
    }
}
