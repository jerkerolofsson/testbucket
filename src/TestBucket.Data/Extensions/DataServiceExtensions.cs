using TestBucket.Data.AI;
using TestBucket.Data.AI.MCP;
using TestBucket.Data.Audit;
using TestBucket.Data.Automation;
using TestBucket.Data.Code;
using TestBucket.Data.Comments;
using TestBucket.Data.Fields;
using TestBucket.Data.Files;
using TestBucket.Data.Identity;
using TestBucket.Data.Insights;
using TestBucket.Data.Issues;
using TestBucket.Data.Labels;
using TestBucket.Data.Metrics;
using TestBucket.Data.Milestones;
using TestBucket.Data.Projects;
using TestBucket.Data.Requirements;
using TestBucket.Data.Runners;
using TestBucket.Data.Sequence;
using TestBucket.Data.Settings;
using TestBucket.Data.Teams;
using TestBucket.Data.Tenants;
using TestBucket.Data.TestAccounts;
using TestBucket.Data.TestEnvironments;
using TestBucket.Data.Testing;
using TestBucket.Data.TestResources;
using TestBucket.Domain.AI.Billing;
using TestBucket.Domain.AI.Mcp;
using TestBucket.Domain.Audit;
using TestBucket.Domain.Automation.Pipelines;
using TestBucket.Domain.Automation.Runners;
using TestBucket.Domain.Code;
using TestBucket.Domain.Code.CodeCoverage;
using TestBucket.Domain.Code.Services;
using TestBucket.Domain.Comments;
using TestBucket.Domain.Environments;
using TestBucket.Domain.Fields;
using TestBucket.Domain.Files;
using TestBucket.Domain.Identity.Permissions;
using TestBucket.Domain.Insights;
using TestBucket.Domain.Issues;
using TestBucket.Domain.Labels;
using TestBucket.Domain.Metrics;
using TestBucket.Domain.Milestones;
using TestBucket.Domain.Requirements;
using TestBucket.Domain.Settings;
using TestBucket.Domain.States;
using TestBucket.Domain.Teams;
using TestBucket.Domain.TestAccounts;
using TestBucket.Domain.Testing.Heuristics;
using TestBucket.Domain.TestResources;

namespace Microsoft.Extensions.DependencyInjection;
public static class DataServiceExtensions
{
    public static IServiceCollection AddDataServices(this IServiceCollection services)
    {
        services.AddScoped<ISequenceGenerator, SequenceGenerator>();

        services.AddScoped<ICodeCoverageRepository, CodeCoverageRepository>();
        services.AddScoped<IAuditRepository, AuditRepository>();
        services.AddScoped<IDashboardRepository, DashboardRepository>();
        services.AddScoped<IHeuristicsRepository, HeuristicsRepository>();
        services.AddScoped<IMetricsRepository, MetricsRepository>();
        services.AddScoped<ICommitRepository, CommitRepository>();
        services.AddScoped<IArchitectureRepository, ArchitectureRepository>();
        services.AddScoped<IMilestoneRepository, MilestoneRepository>();
        services.AddScoped<ILabelRepository, LabelRepository>();
        services.AddScoped<IPipelineRepository, PipelineRepository>();
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
        services.AddScoped<IPermissionsRepository, PermissionsRepository>();
        services.AddScoped<IFileRepository, FileRepository>();
        services.AddScoped<ITestEnvironmentRepository, TestEnvironmentRepository>();
        services.AddScoped<ITestResourceRepository, TestResourceRepository>();
        services.AddScoped<ITestAccountRepository, TestAccountRepository>();
        services.AddScoped<IIssueRepository, IssueRepository>();
        services.AddScoped<IRunnerRepository, RunnerRepository>();
        services.AddScoped<IJobRepository, JobRepository>();
        services.AddScoped<ICommentRepository, CommentsRepository>();
        services.AddScoped<IMcpUserInputRepository, McpUserInputRepository>();
        services.AddScoped<IMcpServerRepository, McpServerRepository>();
        services.AddScoped<IAIUsageRepository, AIUsageRepository>();
        services.AddScoped<IStateRepository, StateRepository>();


        return services;
    }
}
