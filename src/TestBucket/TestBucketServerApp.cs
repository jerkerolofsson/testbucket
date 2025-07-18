using System.Globalization;

using Blazored.LocalStorage;

using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;

using MudBlazor.Services;

using MudExtensions.Services;

using TestBucket.Components;
using TestBucket.Components.Account;
using TestBucket.Components.AI.Controllers;
using TestBucket.Components.AI.Runner.Commands;
using TestBucket.Components.Automation;
using TestBucket.Components.Code.Controllers;
using TestBucket.Components.Comments;
using TestBucket.Components.Environments.Services;
using TestBucket.Components.Issues.Commands;
using TestBucket.Components.Issues.Controllers;
using TestBucket.Components.Labels.Controllers;
using TestBucket.Components.Layout.Controls;
using TestBucket.Components.Metrics.Controls.Controllers;
using TestBucket.Components.Milestones.Controllers;
using TestBucket.Components.Projects;
using TestBucket.Components.Reporting.Controllers;
using TestBucket.Components.Requirements.Commands.Collections;
using TestBucket.Components.Requirements.Commands.Epics;
using TestBucket.Components.Requirements.Commands.Folders;
using TestBucket.Components.Requirements.Commands.General;
using TestBucket.Components.Requirements.Commands.Initatives;
using TestBucket.Components.Requirements.Commands.Shared;
using TestBucket.Components.Requirements.Commands.Tasks;
using TestBucket.Components.Requirements.Services;
using TestBucket.Components.Search.Controllers;
using TestBucket.Components.Settings.ApiKeys;
using TestBucket.Components.Settings.Commands;
using TestBucket.Components.Settings.Controllers;
using TestBucket.Components.Settings.Links;
using TestBucket.Components.Settings.Roles;
using TestBucket.Components.Shared.Commands;
using TestBucket.Components.Shared.Fields;
using TestBucket.Components.Shared.Themeing;
using TestBucket.Components.Teams;
using TestBucket.Components.TestAccounts.Services;
using TestBucket.Components.TestResources.Services;
using TestBucket.Components.Tests.Heuristics.Controllers;
using TestBucket.Components.Tests.Services;
using TestBucket.Components.Tests.TestCases.Commands;
using TestBucket.Components.Tests.TestCases.Services;
using TestBucket.Components.Tests.TestLab.Commands;
using TestBucket.Components.Tests.TestLab.Services;
using TestBucket.Components.Tests.TestRepository.Commands;
using TestBucket.Components.Tests.TestRepository.Services;
using TestBucket.Components.Tests.TestRuns.Commands;
using TestBucket.Components.Tests.TestRuns.Controllers;
using TestBucket.Components.Tests.TestRuns.LinkIssue;
using TestBucket.Components.Tests.TestSuites.Commands;
using TestBucket.Components.Tests.TestSuites.Services;
using TestBucket.Components.Uploads.Services;
using TestBucket.Components.Users;
using TestBucket.Components.Users.Services;
using TestBucket.Contracts.Localization;
using TestBucket.Data.Migrations;
using TestBucket.Domain.AI.Mcp.Extensions;
using TestBucket.Domain.ApiKeys;
using TestBucket.Domain.Commands;
using TestBucket.Domain.Settings.Models;
using TestBucket.Identity;
using TestBucket.Localization;

using Toolbelt.Blazor.Extensions.DependencyInjection;

namespace TestBucket;

public static class TestBucketServerApp
{
    public static WebApplication CreateApp(this WebApplicationBuilder builder)
    {

        builder.AddServiceDefaults();

        // Add services to the container.
        builder.Services.AddRazorComponents()
            .AddInteractiveServerComponents(configure =>
            {
                
            })
            .AddHubOptions(hubOptions =>
            {
                // We set this to quite large to handle capture screenshots etc
                hubOptions.MaximumReceiveMessageSize = 2_000_000;
                //hubOptions.ClientTimeoutInterval = TimeSpan.FromSeconds(10);
            });

        builder.Services.Configure<RequestLocalizationOptions>(options =>
        {
            List<CultureInfo> supportedUICultures = [new CultureInfo("en"), new CultureInfo("de-DE"), new CultureInfo("ja-JP"), new CultureInfo("sv-SE"), new CultureInfo("zh-Hans")];

            options.DefaultRequestCulture = new RequestCulture("en", "en");
            options.SupportedUICultures = supportedUICultures;
            options.RequestCultureProviders.Insert(0, new CustomRequestCultureProvider((HttpContext context) =>
            {
                // Check for Locale cookie first
                if (context.Request.Cookies.TryGetValue("Locale", out string? value))
                {
                    return Task.FromResult< ProviderCultureResult?>(new ProviderCultureResult(value));
                }

                return Task.FromResult<ProviderCultureResult?>(null);
            }));
            options.RequestCultureProviders.Insert(1, new AcceptLanguageHeaderRequestCultureProvider());
        });

        builder.Services.AddBlazoredLocalStorage();
        builder.Services.AddMemoryCache();

        // MCP
        builder.Services.AddHttpContextAccessor();
        builder.Services.AddMcpTools();

        builder.Services.AddSettingLinks();

        builder.Services.AddSingleton<RoleLocalizer>();
        builder.Services.AddLocalization(options =>
        {
        });

        builder.Services.AddSingleton<IAppLocalization, AppLocalization>();

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
                builder.UseVector();
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
            if(MigrationReadyWaiter.IsReady)
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
            options.Tokens.AuthenticatorIssuer = "TestBucket";
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

        builder.Services.AddScoped<ArchitectureController>();
        builder.Services.AddScoped<CommitController>();

        builder.Services.AddScoped<MetricsController>();
        builder.Services.AddScoped<CommentsController>();
        builder.Services.AddScoped<UserApiKeysController>();
        builder.Services.AddScoped<UserRegistrationController>();
        builder.Services.AddScoped<ThemingService>();
        builder.Services.AddScoped<TenantResolver>();
        builder.Services.AddScoped<TenantController>();
        builder.Services.AddScoped<RolesController>();
        builder.Services.AddScoped<TeamController>();
        builder.Services.AddScoped<ProjectController>();
        builder.Services.AddScoped<TestSuiteController>();
        builder.Services.AddScoped<TestLabController>();
        builder.Services.AddScoped<TestRepositoryController>();
        builder.Services.AddScoped<TestRunCreationController>();
        builder.Services.AddScoped<AttachmentsService>();
        builder.Services.AddScoped<AppNavigationManager>();
        builder.Services.AddScoped<TestEnvironmentController>();
        builder.Services.AddScoped<HeuristicsController>();
        builder.Services.AddScoped<TestResourceController>();
        builder.Services.AddScoped<TestAccountController>();
        builder.Services.AddScoped<DashboardController>();
        builder.Services.AddScoped<PipelineController>();
        builder.Services.AddSingleton<ResourceViewFactory>();
        builder.Services.AddScoped<RequirementBrowser>();
        builder.Services.AddScoped<RequirementEditorController>();
        builder.Services.AddScoped<TestExecutionController>();
        builder.Services.AddScoped<RunnersController>();
        builder.Services.AddScoped<IssueController>();
        builder.Services.AddScoped<InsightsController>();
        builder.Services.AddScoped<TestRunController>();
        builder.Services.AddScoped<UnifiedSearchController>();
        builder.Services.AddScoped<UserPreferencesController>();
        builder.Services.AddScoped<CommandController>();
        builder.Services.AddScoped<McpController>();


        builder.Services.AddScoped<UserPreferencesService>();
        builder.Services.AddScoped<TestBrowser>();
        builder.Services.AddScoped<TestCaseEditorController>();
        builder.Services.AddScoped<UploadService>();
        builder.Services.AddScoped<FieldController>();
        builder.Services.AddScoped<UserController>();
        builder.Services.AddScoped<CommandController>();

        builder.Services.AddScoped<HotKeysService>();

        // Test suite

        builder.Services.AddScoped<ICommand, RunTestSuiteCommand>();
        builder.Services.AddScoped<ICommand, NewTestSuiteCommand>();
        builder.Services.AddScoped<ICommand, DeleteTestSuiteCommand>();

        builder.Services.AddScoped<ICommand, AddTestSuiteToRunCommand>();
        builder.Services.AddScoped<ICommand, DeleteTestSuiteFolderCommand>();
        builder.Services.AddScoped<ICommand, DeleteTestRepositoryFolderCommand>();
        builder.Services.AddScoped<ICommand, DeleteTestLabFolderCommand>();
        builder.Services.AddScoped<ICommand, EditTestSuiteFolderCommand>();
        builder.Services.AddScoped<ICommand, RunTestSuiteFolderCommand>();

        builder.Services.AddScoped<MilestonesController>();
        builder.Services.AddScoped<LabelController>();

        // Test tree view
        builder.Services.AddScoped<ICommand, BatchTagCommand>();
        builder.Services.AddScoped<ICommand, NewFolderCommand>();
        builder.Services.AddScoped<ICommand, SyncWithActiveDocumentCommand>();

        // Test case
        builder.Services.AddScoped<ICommand, DuplicateTestCommand>();
        builder.Services.AddScoped<ICommand, NewTestCommand>();
        builder.Services.AddScoped<ICommand, NewExploratoryTestCommand>();
        builder.Services.AddScoped<ICommand, NewTemplateCommand>();
        builder.Services.AddScoped<ICommand, NewSharedStepsCommand>();
        builder.Services.AddScoped<ICommand, DeleteTestCommand>();
        builder.Services.AddScoped<ICommand, RunTestCaseCommand>();

        // Test Case Run

        builder.Services.AddScoped<ICommand, ImportTestCasesCommand>();
        builder.Services.AddScoped<ICommand, ImportTestResultsCommand>();

        builder.Services.AddScoped<ICommand, LinkIssueCommand>();
        builder.Services.AddScoped<ICommand, DeleteLinkedIssueCommand>();
        builder.Services.AddScoped<ICommand, RefreshLinkedIssueCommand>();

        // requirement
        builder.Services.AddScoped<ICommand, SyncWithActiveRequirementCommand>();
        builder.Services.AddScoped<ICommand, CreateTestCaseFromRequirementCommand>();
        builder.Services.AddScoped<ICommand, DeleteRequirementCommand>();
        builder.Services.AddScoped<ICommand, DeleteRequirementFolderCommand>();
        builder.Services.AddScoped<ICommand, DeleteRequirementSpecificationCommand>();
        builder.Services.AddScoped<ICommand, NewRequirementSpecificationSearchFolderCommand>();
        builder.Services.AddScoped<ICommand, NewRequirementFolderCommand>();
        builder.Services.AddScoped<ICommand, BatchTagRequirementsCommand>();

        builder.Services.AddScoped<ICommand, SplitSpecificationCommand>();
        builder.Services.AddScoped<ICommand, ApproveRequirementCommand>();
        builder.Services.AddScoped<ICommand, EditRequirementCommand>();

        builder.Services.AddScoped<ICommand, LinkRequirementToTestCommand>();
        builder.Services.AddScoped<ICommand, CreateRequirementCommand>();
        builder.Services.AddScoped<ICommand, CreateInitiativeCommand>();
        builder.Services.AddScoped<ICommand, CreateEpicCommand>();
        builder.Services.AddScoped<ICommand, CreateTaskCommand>();
        builder.Services.AddScoped<ICommand, CreateSpecificationCommand>();
        builder.Services.AddScoped<ICommand, ImportRequirementsCommand>();
        builder.Services.AddScoped<ICommand, ExportCollectionCommand>();

        // Issues
        builder.Services.AddScoped<ICommand, CreateIssueCommand>();
        builder.Services.AddScoped<ICommand, CloseIssueCommand>();

        // Run
        builder.Services.AddScoped<ICommand, AssignAllUnassignedToAiRunner>();
        builder.Services.AddScoped<ICommand, AssignAllUnassignedToMe>();
        builder.Services.AddScoped<ICommand, DeleteRunCommand>();
        builder.Services.AddScoped<ICommand, DuplicateRunCommand>();

        // Test Case Run
        builder.Services.AddScoped<ICommand, GoToTestRunCommand>();
        builder.Services.AddScoped<ICommand, GoToTestCaseCommand>();

        // Settings
        builder.Services.AddScoped<ICommand, OpenAiSettingsCommand>();

        builder.Services.AddScoped(typeof(DragAndDropService<>));
        builder.Services.AddDataServices();
        builder.Services.AddDomainServices();

        // Integrations
        builder.Services.AddGitHubExtension();
        builder.Services.AddTrelloExtension();
        builder.Services.AddGitlabExtension();
        builder.Services.AddJiraExtension();
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
        app.UseRequestLocalization(app.Services.GetRequiredService<IOptions<RequestLocalizationOptions>>().Value);

        if (Environment.GetEnvironmentVariable("TB_HTTPS_REDIRECT") != "disabled")
        {
            app.UseHttpsRedirection();
        }
        app.UseCors(localhostOrigin);

        app.MapMcp("/mcp");

        app.MapStaticAssets();
        app.MapControllers();
        app.MapRazorComponents<App>()
            .AddInteractiveServerRenderMode();

        // Add additional endpoints required by the Identity /Account Razor components.
        app.MapAdditionalIdentityEndpoints();

        return app;
    }
}
