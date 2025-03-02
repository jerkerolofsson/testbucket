using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Data.Files;
using TestBucket.Data.Identity;
using TestBucket.Data.Settings;
using TestBucket.Data.Teams;
using TestBucket.Data.Tenants;
using TestBucket.Data.Testing;
using TestBucket.Domain.Files;
using TestBucket.Domain.Projects;
using TestBucket.Domain.Settings;
using TestBucket.Domain.Teams;
using TestBucket.Domain.Tenants;
using TestBucket.Domain.Testing;

namespace Microsoft.Extensions.DependencyInjection;
public static class DataServiceExtensions
{
    public static IServiceCollection AddDataServices(this IServiceCollection services)
    {
        services.AddScoped<ITenantRepository, TenantRepository>();
        services.AddScoped<IProjectRepository, ProjectRepository>();
        services.AddScoped<ITeamRepository, TeamRepository>();
        services.AddScoped<ISuperAdminUserService, SuperAdminUserService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<ISettingsProvider, SettingsRepository>();
        services.AddScoped<ITestCaseRepository, TestCaseRepository>();
        services.AddScoped<IUserPreferenceRepository, UserPreferenceRepository>();
        services.AddScoped<IFileRepository, FileRepository>();

        return services;
    }
}
