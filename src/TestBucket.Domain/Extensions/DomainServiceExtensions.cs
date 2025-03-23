using TestBucket.Domain.AI;
using TestBucket.Domain.AI.Settings;
using TestBucket.Domain.Fields;
using TestBucket.Domain.Identity;
using TestBucket.Domain.Progress;
using TestBucket.Domain.Projects;
using TestBucket.Domain.Requirements;
using TestBucket.Domain.Requirements.Import;
using TestBucket.Domain.Settings;
using TestBucket.Domain.Settings.Appearance;
using TestBucket.Domain.Settings.Server;
using TestBucket.Domain.States;
using TestBucket.Domain.Testing;
using TestBucket.Domain.Testing.Settings;

namespace Microsoft.Extensions.DependencyInjection;
public static class DomainServiceExtensions
{
    public static IServiceCollection AddDomainServices(this IServiceCollection services)
    {
        services.AddScoped<IStateService, StateService>();
        services.AddScoped<ITextTestResultsImporter, TextImporter>();
        services.AddScoped<ITestCaseGenerator, TestCaseGenerator>();
        services.AddScoped<ITestCaseManager, TestCaseManager>();
        services.AddScoped<ITestRunManager, TestRunManager>();

        services.AddScoped<IRequirementImporter, RequirementImporter>();
        services.AddScoped<IRequirementManager, RequirementManager>();
        services.AddScoped<IUserManager, UserManager>();
        services.AddScoped<IFieldDefinitionManager, FieldDefinitionManager>();
        services.AddScoped<IFieldManager, FieldManager>();
        services.AddScoped<IProjectManager, ProjectManager>();

        services.AddScoped<IProgressManager, ProgressManager>();
        services.AddScoped<IChatClientFactory, ChatClientFactory>();

        // Settings
        services.AddScoped<IUserPreferencesManager, UserPreferencesManager>();
        services.AddScoped<ISettingsManager, SettingsManager>();
        services.AddScoped<ISetting, DefaultTenantSetting>();
        services.AddScoped<ISetting, DarkModeSetting>();
        services.AddScoped<ISetting, ThemeSetting>();
        services.AddScoped<ISetting, IncreasedContrastSetting>();
        services.AddScoped<ISetting, AiProviderSetting>();
        services.AddScoped<ISetting, AiProviderUrlSetting>();
        services.AddScoped<ISetting, AiModelSetting>();
        services.AddScoped<ISetting, GithubModelsDeveloperKeySetting>();
        services.AddScoped<ISetting, AzureAiProductionKeySetting>();

        // Test settings
        services.AddScoped<ISetting, ShowFailureMessageDialogWhenFailingTestCaseRunSetting>();
        services.AddScoped<ISetting, AdvanceToNextNotCompletedTestWhenSettingResultSetting>();

        return services;
    }
}
