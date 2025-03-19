using TestBucket.Domain.AI;
using TestBucket.Domain.Fields;
using TestBucket.Domain.Identity;
using TestBucket.Domain.Requirements;
using TestBucket.Domain.Requirements.Import;
using TestBucket.Domain.Settings;
using TestBucket.Domain.Settings.Appearance;
using TestBucket.Domain.Settings.Server;
using TestBucket.Domain.States;
using TestBucket.Domain.Testing;

namespace Microsoft.Extensions.DependencyInjection;
public static class DomainServiceExtensions
{
    public static IServiceCollection AddDomainServices(this IServiceCollection services)
    {
        services.AddScoped<IStateService, StateService>();
        services.AddScoped<ITextTestResultsImporter, TextImporter>();
        services.AddScoped<ITestCaseGenerator, TestCaseGenerator>();

        services.AddScoped<IRequirementImporter, RequirementImporter>();
        services.AddScoped<IRequirementManager, RequirementManager>();

        services.AddScoped<IFieldDefinitionManager, FieldDefinitionManager>();

        // Settings
        services.AddScoped<IUserPreferencesManager, UserPreferencesManager>();
        services.AddScoped<ISettingsManager, SettingsManager>();
        services.AddScoped<ISetting, DefaultTenantSetting>();
        services.AddScoped<ISetting, DarkModeSetting>();
        services.AddScoped<ISetting, ThemeSetting>();
        services.AddScoped<ISetting, IncreasedContrastSetting>();

        return services;
    }
}
