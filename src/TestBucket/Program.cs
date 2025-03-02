using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
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

namespace TestBucket;

public class Program
{
    public static void Main(string[] args)
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

        builder.Services.AddAuthentication(options =>
            {
                options.DefaultScheme = IdentityConstants.ApplicationScheme;
                options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
            })
            .AddIdentityCookies();

        builder.AddNpgsqlDbContext<ApplicationDbContext>("testbucketdb", null, dbContextBuilder =>
        {
            if (builder.Environment.IsDevelopment())
            {
                dbContextBuilder.EnableSensitiveDataLogging();
                dbContextBuilder.EnableDetailedErrors();
            }
        });
        builder.Services.AddDbContextFactory<ApplicationDbContext>(dbContextOptionsBuilder => dbContextOptionsBuilder.UseNpgsql());

        //var connectionString = builder.Configuration.GetConnectionString("ConnectionStrings__testbucket") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
        //builder.Services.AddDbContext<ApplicationDbContext>(options =>
        //    options.UseNpgsql(connectionString));
        builder.Services.AddDatabaseDeveloperPageExceptionFilter();
        builder.Services.AddHostedService<MigrationService>();

        builder.Services.AddIdentityCore<ApplicationUser>(options => {
            options.SignIn.RequireConfirmedAccount = true;
        })
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddSignInManager()
            .AddDefaultTokenProviders();

        builder.Services.AddScoped<SignInManager<ApplicationUser>, ApplicationSignInManager>();
        builder.Services.AddSingleton<IEmailSender<ApplicationUser>, IdentityNoOpEmailSender>();

        builder.Services.AddScoped<UserRegistrationService>();
        builder.Services.AddScoped<TenantResolver>();
        builder.Services.AddScoped<TenantService>();
        builder.Services.AddScoped<TeamService>();
        builder.Services.AddScoped<ProjectService>();
        builder.Services.AddScoped<TestSuiteService>();
        builder.Services.AddScoped<TestService>();
        builder.Services.AddScoped<UserPreferencesService>();
        builder.Services.AddScoped<TestBrowser>();
        builder.Services.AddScoped<TestCaseEditorService>();
        builder.Services.AddScoped<UploadService>();
        builder.Services.AddScoped(typeof(DragAndDropService<>));
        builder.Services.AddDataServices();
        builder.Services.AddDomainServices();

        builder.Services.AddMudServices(config =>
        {
        });

        builder.Services.AddControllers();

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

        app.UseHttpsRedirection();

        app.UseAntiforgery();

        app.MapStaticAssets();
        app.MapControllers();
        app.MapRazorComponents<App>()
            .AddInteractiveServerRenderMode();

        // Add additional endpoints required by the Identity /Account Razor components.
        app.MapAdditionalIdentityEndpoints();

        app.Run();
    }
}
