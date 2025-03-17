using TestBucket.Data.Fields;
using TestBucket.Data.Files;
using TestBucket.Data.Identity;
using TestBucket.Data.Requirements;
using TestBucket.Data.Settings;
using TestBucket.Data.Teams;
using TestBucket.Data.Tenants;
using TestBucket.Data.Testing;
using TestBucket.Domain.Fields;
using TestBucket.Domain.Files;
using TestBucket.Domain.Requirements;
using TestBucket.Domain.Settings;
using TestBucket.Domain.Teams;

namespace Microsoft.Extensions.DependencyInjection;
public static class DataServiceExtensions
{
    public static IServiceCollection AddDataServices(this IServiceCollection services)
    {
        services.AddScoped<IRequirementRepository, RequirementRepository>();
        services.AddScoped<ITenantRepository, TenantRepository>();
        services.AddScoped<IProjectRepository, ProjectRepository>();
        services.AddScoped<ITeamRepository, TeamRepository>();
        services.AddScoped<IFieldRepository, FieldRepository>();
        services.AddScoped<ISuperAdminUserService, SuperAdminUserService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<ISettingsProvider, SettingsRepository>();
        services.AddScoped<ITestCaseRepository, TestCaseRepository>();
        services.AddScoped<IUserPreferenceRepository, UserPreferenceRepository>();
        services.AddScoped<IFileRepository, FileRepository>();

        return services;
    }
}
