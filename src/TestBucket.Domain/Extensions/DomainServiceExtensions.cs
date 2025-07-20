using FluentValidation;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;

using TestBucket.Contracts.Integrations;
using TestBucket.Domain.AI;
using TestBucket.Domain.AI.Agent;
using TestBucket.Domain.AI.Mcp;
using TestBucket.Domain.AI.Mcp.Services;
using TestBucket.Domain.AI.Runner;
using TestBucket.Domain.AI.Settings;
using TestBucket.Domain.AI.Settings.LLM;
using TestBucket.Domain.ApiKeys;
using TestBucket.Domain.Appearance;
using TestBucket.Domain.Automation.Hybrid;
using TestBucket.Domain.Automation.Pipelines;
using TestBucket.Domain.Automation.Runners;
using TestBucket.Domain.Automation.Runners.Jobs;
using TestBucket.Domain.Code.DataSources;
using TestBucket.Domain.Code.Services;
using TestBucket.Domain.Commands;
using TestBucket.Domain.Comments;
using TestBucket.Domain.Environments;
using TestBucket.Domain.Export;
using TestBucket.Domain.Export.Services;
using TestBucket.Domain.ExtensionManagement;
using TestBucket.Domain.Features.Classification;
using TestBucket.Domain.Fields;
using TestBucket.Domain.Files;
using TestBucket.Domain.Identity;
using TestBucket.Domain.Identity.OAuth;
using TestBucket.Domain.Identity.Permissions;
using TestBucket.Domain.Insights;
using TestBucket.Domain.Issues;
using TestBucket.Domain.Issues.Insights;
using TestBucket.Domain.Labels;
using TestBucket.Domain.Labels.DataSources;
using TestBucket.Domain.Labels.Services;
using TestBucket.Domain.Metrics;
using TestBucket.Domain.Milestones;
using TestBucket.Domain.Milestones.DataSources;
using TestBucket.Domain.Milestones.Services;
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
using TestBucket.Domain.Testing.Heuristics;
using TestBucket.Domain.Testing.Markdown;
using TestBucket.Domain.Testing.Services.Import;
using TestBucket.Domain.Testing.Settings;
using TestBucket.Domain.Testing.TestCases;
using TestBucket.Domain.Testing.TestCases.Insights;
using TestBucket.Domain.Testing.TestLab;
using TestBucket.Domain.Testing.TestRepository;
using TestBucket.Domain.Testing.TestRuns;
using TestBucket.Domain.Testing.TestRuns.Insights;
using TestBucket.Domain.Testing.TestSuites;
using TestBucket.Domain.TestResources;
using TestBucket.Domain.TestResources.Allocation;
using TestBucket.Domain.TestResources.Settings;
using TestBucket.Integrations;

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

        // OAuth
        services.AddSingleton<OAuthAuthManager>();
        services.AddScoped<IOAuth2Authenticator, OAuthAuthenticator>();
        services.AddHostedService<BackgroundTokenRefresher>();

        services.AddSingleton<IApiKeyAuthenticator,ApiKeyAuthenticator>();
        services.AddScoped<IUserPermissionsManager, UserPermissionsManager>();
        services.AddScoped<IClaimsTransformation, PermissionClaimsTransformation>();
        services.AddScoped<ITenantManager, TenantManager>();
        services.AddScoped<IExtensionManager, ExtensionManager>();

        // Data sources
        services.AddScoped<IFieldCompletionsProvider, ExtensionFieldCompletionsAggregator>();
        services.AddScoped<IFieldCompletionsProvider, ComponentDataSource>();
        services.AddScoped<IFieldCompletionsProvider, FeatureDataSource>();
        services.AddScoped<IFieldCompletionsProvider, CommitDataSource>();
        services.AddScoped<IFieldCompletionsProvider, MilestoneDataSource>();
        services.AddScoped<IFieldCompletionsProvider, LabelDataSource>();

        // Insight data sources
        services.AddScoped<IInsightsDataSource, IssuesByComponentDataSource>();
        services.AddScoped<IInsightsDataSource, IssuesByStateDataSource>();
        services.AddScoped<IInsightsDataSource, IssuesInflowOutflow>();
        services.AddScoped<IInsightsDataSource, CountByResultDataSource>();
        services.AddScoped<IInsightsDataSource, CountByLatestResultDataSource>();
        services.AddScoped<IInsightsDataSource, ExecutedTestsByAsigneeDataSource>();
        services.AddScoped<IInsightsDataSource, ResultsByComponentDataSource>();
        services.AddScoped<IInsightsDataSource, CountByCategoryDataSource>();
        services.AddScoped<IInsightsDataSource, CountByComponentDataSource>();

        services.AddScoped<IInsightsDataManager, InsightsDataManager>();
        services.AddScoped<IDashboardManager, DashboardManager>();

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
        services.AddScoped<TestExecutionContextBuilder>();

        services.AddScoped<ITestCaseImporter, TestCaseImporter>();
        services.AddScoped<ITextTestResultsImporter, TestResultTextImporter>();
        services.AddScoped<ITestCaseManager, TestCaseManager>();
        services.AddScoped<ITestSuiteManager, TestSuiteManager>();
        services.AddScoped<ITestRunManager, TestRunManager>();
        services.AddScoped<ITestLabManager, TestLabManager>();
        services.AddScoped<ITestRepositoryManager, TestRepositoryManager>();

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
        services.AddScoped<IMilestoneManager, MilestoneManager>();
        services.AddScoped<ILabelManager, LabelManager>();
        services.AddHostedService<IssueProviderBackgroundIndexer>();
        services.AddHostedService<MilestoneIndexer>();
        services.AddHostedService<LabelIndexer>();

        services.AddScoped<IUserManager, UserManager>();
        services.AddScoped<IProfilePictureManager, ProfilePictureManager>();

        // Code
        services.AddScoped<IArchitectureManager, ArchitectureManager>();
        services.AddScoped<ICommitManager, CommitManager>();
        services.AddHostedService<CodeRepoCommmitBackgroundIndexer>();

        services.AddScoped<ITeamManager, TeamManager>();
        services.AddScoped<IProjectManager, ProjectManager>();
        services.AddScoped<IPipelineProjectManager, PipelineProjectManager>();
        services.AddScoped<IProjectTokenGenerator, ProjectTokenGenerator>();
        services.AddScoped<IPipelineManager, PipelineManager>();
        services.AddHostedService<UnconnectedPipelineIndexer>();

        services.AddScoped<ICommentsManager, CommentsManager>();

        services.AddScoped<ITestResourceManager, TestResourceManager>();
        services.AddScoped<ITestAccountManager, TestAccountManager>();
        services.AddTransient<ISetting, DeleteResourceIfNotSeenFor>();

        services.AddScoped<ICommandManager, CommandManager>();
        services.AddScoped<IUnifiedSearchManager, UnifiedSearchManager>();
        services.AddScoped<IProgressManager, ProgressManager>();
        services.AddScoped<ITestEnvironmentManager, TestEnvironmentManager>();
        services.AddScoped<IHeuristicsManager, HeuristicsManager>();

        // automation
        services.AddScoped<IMarkdownAutomationRunner, MarkdownAutomationRunner>();

        // validation
        services.AddValidatorsFromAssemblyContaining<RequirementSpecificationFolderValidator>();

        // AI
        services.AddScoped<IChatClientFactory, ChatClientFactory>();
        services.AddScoped<IMcpServerManager, McpServerManager>();
        services.AddSingleton<McpServerRunnerManager>();
        services.AddHostedService<McpServerStartupService>();
        

        // Feature: Classification
        services.AddScoped<IClassifier, GenericClassifier>();
        services.AddHostedService<BackgroundIssueClassificationService>();
        services.AddHostedService<BackgroundTestClassificationService>();

        // Settings
        services.AddScoped<IUserPreferencesManager, UserPreferencesManager>();
        services.AddScoped<ISettingsManager, SettingsManager>();
        services.AddScoped<ISetting, DefaultTenantSetting>();
        services.AddScoped<ISetting, DarkModeSetting>();
        services.AddScoped<ISetting, ExplorerDockSetting>();
        services.AddScoped<ISetting, ProfileImageSetting>();
        services.AddScoped<ISetting, ThemeSetting>();

        // Accessibility
        services.AddScoped<ISetting, IncreasedContrastSetting>();
        services.AddScoped<ISetting, IncreasedFontSizeSetting>();
        services.AddScoped<ISetting, PreferTextToIconsSetting>();
        services.AddScoped<ISetting, ReducedMotionSetting>();

        // AI LLM settings
        services.AddScoped<ISetting, AiProviderSetting>();
        services.AddScoped<ISetting, AiProviderUrlSetting>();
        services.AddScoped<ISetting, EmbeddingAiProviderSetting>();
        services.AddScoped<ISetting, EmbeddingAiProviderUrlSetting>();
        services.AddScoped<ISetting, AiModelSetting>();
        services.AddScoped<ISetting, AiLlmEmbeddingModelSetting>();
        services.AddScoped<ISetting, GithubModelsDeveloperKeySetting>();
        services.AddScoped<ISetting, AzureAiProductionKeySetting>();
        services.AddScoped<ISetting, AnthropicApiKeySetting>();

        // AI Runner settings
        services.AddScoped<ISetting, EnableAiRunnerSetting>();
        services.AddScoped<ISetting, MaxTokensPerDayForAitRunnerSetting>();

        // LLM
        services.AddScoped<AgentChatContext>();

        // AI Runner
        services.AddSingleton<AiRunnerJobQueue>();
        services.AddHostedService<AiRunner>();
        services.AddScoped<AiResultInterpreter>();

        // Metrics
        services.AddScoped<IMetricsManager, MetricsManager>();

        // Test settings
        services.AddScoped<ISetting, ShowFailureMessageDialogWhenFailingTestCaseRunSetting>();
        services.AddScoped<ISetting, AdvanceToNextNotCompletedTestWhenSettingResultSetting>();

        services.AddScoped<ITestBucketThemeManager, TestBucketThemeManager>();

        return services;
    }
}
