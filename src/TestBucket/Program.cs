using MudExtensions.Services;
using TestBucket.Components;
using TestBucket.Components.Account;
using TestBucket.Data;
using Npgsql.EntityFrameworkCore.PostgreSQL.Extensions;
using TestBucket.Data.Migrations;
using TestBucket.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using TestBucket.Components.Tenants;
using TestBucket.Components.Projects;
using MudBlazor.Services;
using TestBucket.Components.Users;
using TestBucket.Components.Tests.Services;
using TestBucket.Domain.Identity.Models;
using TestBucket.Components.Shared;
using Blazored.LocalStorage;
using TestBucket.Components.Uploads.Services;
using TestBucket.Components.Teams;
using TestBucket.Components.Shared.Fields;
using Npgsql;
using TestBucket.Components.Shared.Themeing;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using TestBucket.Domain.ApiKeys;
using OllamaSharp.Models;
using OllamaSharp;
using System;

namespace TestBucket;

public class Program
{
    public static async Task Main(string[] args)
    {


        var builder = WebApplication.CreateBuilder(args);
        builder.AddServiceDefaults();

        // Add services to the container.
        builder.Services.AddRazorComponents()
            .AddInteractiveServerComponents();

        builder.Services.AddBlazoredLocalStorage();

        builder.Services.AddCascadingAuthenticationState();
        builder.Services.AddScoped<IdentityUserAccessor>();
        builder.Services.AddScoped<IdentityRedirectManager>();
        builder.Services.AddScoped<AuthenticationStateProvider, IdentityRevalidatingAuthenticationStateProvider>();

        builder.Services.AddAuthentication("ApiKey").AddScheme<ApiKeyAuthenticationOptions, ApiKeyAuthenticationHandler>("ApiKey", null);

        await AddOllamaAsync(builder);

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

        builder.Services.AddScoped<UserRegistrationService>();
        builder.Services.AddScoped<ThemingService>();
        builder.Services.AddScoped<TenantResolver>();
        builder.Services.AddScoped<TenantService>();
        builder.Services.AddScoped<TeamService>();
        builder.Services.AddScoped<ProjectService>();
        builder.Services.AddScoped<TestSuiteService>();
        builder.Services.AddScoped<TestRunCreationService>();
        builder.Services.AddScoped<AttachmentsService>();
        builder.Services.AddScoped<AppNavigationManager>();

        builder.Services.AddScoped<TestService>();
        builder.Services.AddScoped<UserPreferencesService>();
        builder.Services.AddScoped<TestBrowser>();
        builder.Services.AddScoped<TestCaseEditorService>();
        builder.Services.AddScoped<UploadService>();
        builder.Services.AddScoped<FieldService>();
        builder.Services.AddScoped(typeof(DragAndDropService<>));
        builder.Services.AddDataServices();
        builder.Services.AddDomainServices();

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

        app.MapStaticAssets();
        app.MapControllers();
        app.MapRazorComponents<App>()
            .AddInteractiveServerRenderMode();

        // Add additional endpoints required by the Identity /Account Razor components.
        app.MapAdditionalIdentityEndpoints();

        app.Run();
    }

    private static async Task AddOllamaAsync(WebApplicationBuilder builder)
    {
        var ollamaBaseUrl = builder.Configuration["OLLAMA_BASE_URL"];
        if (ollamaBaseUrl is not null)
        {
            //string model = "deepseek-r1:7b";
            string model = "deepseek-r1:30b";
            // deepseek-r1:7b
            //var ollama = new OllamaApiClient(ollamaBaseUrl, "deepseek-r1:7b");
            var ollama = new OllamaApiClient(ollamaBaseUrl, model);
            await foreach(var response in ollama.PullModelAsync(model))
            {
                if (response is not null)
                {
                    Console.WriteLine($"{response.Status}: {response.Completed}/{response.Total} ({response.Percent})");
                }
            }
            builder.Services.AddSingleton<Microsoft.Extensions.AI.IChatClient>(ollama);
        }
    }
}
