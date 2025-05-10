using TestBucket.Data.Automation;
using TestBucket.Data.Code;
using TestBucket.Data.Comments;
using TestBucket.Data.Fields;
using TestBucket.Data.Files;
using TestBucket.Data.Identity;
using TestBucket.Data.Issues;
using TestBucket.Data.Milestones;
using TestBucket.Data.Requirements;
using TestBucket.Data.Runners;
using TestBucket.Data.Settings;
using TestBucket.Data.Teams;
using TestBucket.Data.Tenants;
using TestBucket.Data.TestAccounts;
using TestBucket.Data.TestEnvironments;
using TestBucket.Data.Testing;
using TestBucket.Data.TestResources;
using TestBucket.Domain.Automation.Pipelines;
using TestBucket.Domain.Automation.Runners;
using TestBucket.Domain.Code;
using TestBucket.Domain.Code.Services;
using TestBucket.Domain.Comments;
using TestBucket.Domain.Environments;
using TestBucket.Domain.Fields;
using TestBucket.Domain.Files;
using TestBucket.Domain.Identity.Permissions;
using TestBucket.Domain.Issues;
using TestBucket.Domain.Milestones;
using TestBucket.Domain.Requirements;
using TestBucket.Domain.Settings;
using TestBucket.Domain.Teams;
using TestBucket.Domain.TestAccounts;
using TestBucket.Domain.TestResources;

namespace Microsoft.Extensions.DependencyInjection;
public static class DataServiceExtensions
{
    public static IServiceCollection AddDataServices(this IServiceCollection services)
    {
        services.AddScoped<ICommitRepository, CommitRepository>();
        services.AddScoped<IArchitectureRepository, ArchitectureRepository>();
        services.AddScoped<IMilestoneRepository, MilestoneRepository>();

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
        

        return services;
    }
}
