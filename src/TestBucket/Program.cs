using Blazored.LocalStorage;

using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;

using MudBlazor.Services;

using MudExtensions.Services;

using TestBucket.Components;
using TestBucket.Components.Account;
using TestBucket.Components.Environments.Services;
using TestBucket.Components.Layout.Controls;
using TestBucket.Components.Projects;
using TestBucket.Components.Requirements.Services;
using TestBucket.Components.Settings.ApiKeys;
using TestBucket.Components.Shared.Fields;
using TestBucket.Components.Shared.Themeing;
using TestBucket.Components.Teams;
using TestBucket.Components.Tests;
using TestBucket.Components.Tests.Services;
using TestBucket.Components.Uploads.Services;
using TestBucket.Components.Users;
using TestBucket.Components.Users.Services;
using TestBucket.Contracts.Integrations;
using TestBucket.Data.Migrations;
using TestBucket.Domain.ApiKeys;
using TestBucket.Domain.Commands;
using TestBucket.Gitlab;
using TestBucket.Identity;

using Toolbelt.Blazor.Extensions.DependencyInjection;

namespace TestBucket;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
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


        builder.Services.AddLocalization(options =>
        {
        });

        builder.Services.AddCascadingAuthenticationState();
        builder.Services.AddScoped<IdentityUserAccessor>();
        builder.Services.AddScoped<IdentityRedirectManager>();
        builder.Services.AddScoped<AuthenticationStateProvider, IdentityRevalidatingAuthenticationStateProvider>();

        builder.Services.AddAuthentication("ApiKey").AddScheme<ApiKeyAuthenticationOptions, ApiKeyAuthenticationHandler>("ApiKey", null);

        //await AddOllamaAsync(builder);

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
        //NpgsqlConnection.GlobalTypeMapper.EnableDynamicJson();
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
        // dbContextOptionsBuilder =>  dbContextOptionsBuilder.UseNpgsql()

        //var connectionString = builder.Configuration.GetConnectionString("ConnectionStrings__testbucket") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
        //builder.Services.AddDbContext<ApplicationDbContext>(options =>
        //    options.UseNpgsql(connectionString));
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
        builder.Services.AddScoped<TenantService>();
        builder.Services.AddScoped<TeamService>();
        builder.Services.AddScoped<ProjectController>();
        builder.Services.AddScoped<TestSuiteService>();
        builder.Services.AddScoped<TestRunCreationController>();
        builder.Services.AddScoped<AttachmentsService>();
        builder.Services.AddScoped<AppNavigationManager>();
        builder.Services.AddScoped<TestEnvironmentController>();

        builder.Services.AddScoped<RequirementBrowser>();
        builder.Services.AddScoped<RequirementEditorController>();
        builder.Services.AddScoped<TestExecutionController>();

        builder.Services.AddScoped<UserPreferencesService>();
        builder.Services.AddScoped<TestBrowser>();
        builder.Services.AddScoped<TestCaseEditorController>();
        builder.Services.AddScoped<UploadService>();
        builder.Services.AddScoped<FieldController>();
        builder.Services.AddScoped<UserController>();

        builder.Services.AddScoped<HotKeysService>();
        builder.Services.AddScoped<ICommand, NewTestCommand>();
        builder.Services.AddScoped<ICommand, NewFolderCommand>();

        builder.Services.AddScoped(typeof(DragAndDropService<>));
        builder.Services.AddDataServices();
        builder.Services.AddDomainServices();

        // Integrations
        builder.Services.AddSingleton<IProjectDataSource, GitlabProjectDataSource>();
        builder.Services.AddDotHttpApiTestExtension();

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
        app.UseHttpsRedirection();
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

        app.Run();
    }
}
