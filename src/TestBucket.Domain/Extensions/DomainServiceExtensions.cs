using Microsoft.AspNetCore.Authentication;

using TestBucket.Contracts.Integrations;
using TestBucket.Domain.AI;
using TestBucket.Domain.AI.Settings;
using TestBucket.Domain.ApiKeys;
using TestBucket.Domain.Automation.Hybrid;
using TestBucket.Domain.Automation.Pipelines;
using TestBucket.Domain.Automation.Runners;
using TestBucket.Domain.Automation.Runners.Jobs;
using TestBucket.Domain.Code.Services;
using TestBucket.Domain.Commands;
using TestBucket.Domain.Environments;
using TestBucket.Domain.Export;
using TestBucket.Domain.Export.Services;
using TestBucket.Domain.ExtensionManagement;
using TestBucket.Domain.Fields;
using TestBucket.Domain.Files;
using TestBucket.Domain.Identity;
using TestBucket.Domain.Identity.Permissions;
using TestBucket.Domain.Issues;
using TestBucket.Domain.Progress;
using TestBucket.Domain.Projects;
using TestBucket.Domain.Requirements;
using TestBucket.Domain.Requirements.Import;
using TestBucket.Domain.Requirements.RequirementExtensions;
using TestBucket.Domain.Search;
using TestBucket.Domain.Settings;
using TestBucket.Domain.Settings.Appearance;
using TestBucket.Domain.Settings.Server;
using TestBucket.Domain.States;
using TestBucket.Domain.States.Caching;
using TestBucket.Domain.Teams;
using TestBucket.Domain.Tenants;
using TestBucket.Domain.TestAccounts;
using TestBucket.Domain.TestAccounts.Allocation;
using TestBucket.Domain.Testing;
using TestBucket.Domain.Testing.Compiler;
using TestBucket.Domain.Testing.Markdown;
using TestBucket.Domain.Testing.Services.Classification;
using TestBucket.Domain.Testing.Services.Import;
using TestBucket.Domain.Testing.Settings;
using TestBucket.Domain.Testing.TestCases;
using TestBucket.Domain.Testing.TestRuns;
using TestBucket.Domain.Testing.TestSuites;
using TestBucket.Domain.TestResources;
using TestBucket.Domain.TestResources.Allocation;

namespace Microsoft.Extensions.DependencyInjection;
public static class DomainServiceExtensions
{
    public static IServiceCollection AddDomainServices(this IServiceCollection services)
    {
        //services.AddMediatR(o =>
        //{
        //    o.RegisterServicesFromAssembly(typeof(DomainServiceExtensions).Assembly);
        //});

        services.AddMediator(options =>
        {
            options.ServiceLifetime = ServiceLifetime.Scoped;
        });

        services.AddSingleton<IApiKeyAuthenticator,ApiKeyAuthenticator>();
        services.AddScoped<IUserPermissionsManager, UserPermissionsManager>();
        services.AddScoped<IClaimsTransformation, PermissionClaimsTransformation>();
        services.AddScoped<ITenantManager, TenantManager>();
        services.AddScoped<IExtensionManager, ExtensionManager>();

        // Runner/Hybrid
        services.AddScoped<IMarkdownTestRunner, HybridRunner>();
        services.AddScoped<IMarkdownDetector, HybridDetector>();
        services.AddSingleton<GetJobLock>();
        services.AddSingleton<JobAddedEventSignal>();
        services.AddScoped<IRunnerManager, RunnerManager>();
        services.AddScoped<IJobManager, JobManager>();
        services.AddScoped<TestResourceDependencyAllocator>();
        services.AddScoped<TestAccountDependencyAllocator>();

        services.AddScoped<IFileResourceManager, FileResourceManager>();

        services.AddScoped<IStateService, StateService>();
        services.AddSingleton<ProjectStateCache>();

        services.AddScoped<IMarkdownDetector,TemplateDetector>();
        services.AddScoped<ITestCompiler, TestCompiler>();

        services.AddScoped<ITextTestResultsImporter, TextImporter>();
        services.AddScoped<ITestCaseManager, TestCaseManager>();
        services.AddScoped<ITestSuiteManager, TestSuiteManager>();
        services.AddScoped<ITestRunManager, TestRunManager>();

        // Backup / Export
        services.AddScoped<IBackupManager, BackupManager>();
        services.AddScoped<Exporter>();
        services.AddHostedService<PeriodicBackupService>();

        // Requirements

        services.AddScoped<IRequirementImporter, RequirementImporter>();
        services.AddScoped<IRequirementManager, RequirementManager>();
        services.AddScoped<IRequirementExtensionManager, RequirementExtensionManager>();
        services.AddHostedService<BackgroundExternalRequirementSynchronizer>();

        services.AddScoped<IFieldDefinitionManager, FieldDefinitionManager>();
        services.AddScoped<IFieldManager, FieldManager>();

        services.AddScoped<IIssueManager, IssueManager>();

        services.AddScoped<IUserManager, UserManager>();
        services.AddScoped<IProfilePictureManager, ProfilePictureManager>();

        services.AddScoped<ICommitManager, CommitManager>();
        services.AddHostedService<CodeRepoCommmitBackgroundIndexer>();

        services.AddScoped<ITeamManager, TeamManager>();
        services.AddScoped<IProjectManager, ProjectManager>();
        services.AddScoped<IPipelineProjectManager, PipelineProjectManager>();
        services.AddScoped<IProjectTokenGenerator, ProjectTokenGenerator>();
        services.AddScoped<IPipelineManager, PipelineManager>();

        services.AddScoped<ITestResourceManager, TestResourceManager>();
        services.AddScoped<ITestAccountManager, TestAccountManager>();

        services.AddScoped<ICommandManager, CommandManager>();
        services.AddScoped<IUnifiedSearchManager, UnifiedSearchManager>();
        services.AddScoped<IProgressManager, ProgressManager>();
        services.AddScoped<ITestEnvironmentManager, TestEnvironmentManager>();

        // automation
        services.AddScoped<IMarkdownAutomationRunner, MarkdownAutomationRunner>();

        // AI
        services.AddScoped<IChatClientFactory, ChatClientFactory>();
        services.AddScoped<ITestCaseGenerator, TestCaseGenerator>();
        services.AddScoped<IClassifier, GenericClassifier>();
        services.AddHostedService<BackgroundClassificationService>();

        // Settings
        services.AddScoped<IUserPreferencesManager, UserPreferencesManager>();
        services.AddScoped<ISettingsManager, SettingsManager>();
        services.AddScoped<ISetting, DefaultTenantSetting>();
        services.AddScoped<ISetting, DarkModeSetting>();
        services.AddScoped<ISetting, ProfileImageSetting>();
        services.AddScoped<ISetting, ThemeSetting>();
        services.AddScoped<ISetting, IncreasedContrastSetting>();
        services.AddScoped<ISetting, AiProviderSetting>();
        services.AddScoped<ISetting, AiProviderUrlSetting>();
        services.AddScoped<ISetting, AiModelSetting>();
        services.AddScoped<ISetting, AiLlmClassificationModelSetting>();
        services.AddScoped<ISetting, AiLlmTestGenerationModelSetting>();
        services.AddScoped<ISetting, GithubModelsDeveloperKeySetting>();
        services.AddScoped<ISetting, AzureAiProductionKeySetting>();

        // Test settings
        services.AddScoped<ISetting, ShowFailureMessageDialogWhenFailingTestCaseRunSetting>();
        services.AddScoped<ISetting, AdvanceToNextNotCompletedTestWhenSettingResultSetting>();

        return services;
    }
}
