using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

using TestBucket.Domain.Environments.Models;
using TestBucket.Domain.Fields.Models;
using TestBucket.Domain.Files.Models;
using TestBucket.Domain.Identity.Permissions;
using TestBucket.Domain.Requirements.Models;
using TestBucket.Domain.Settings.Models;
using TestBucket.Domain.Teams.Models;
using TestBucket.Domain.TestResources.Models;
using TestBucket.Domain.TestAccounts.Models;
using TestBucket.Domain.Issues.Models;
using TestBucket.Domain.Automation.Runners.Models;
using TestBucket.Domain.Automation.Pipelines.Models;
using TestBucket.Domain.Code.Models;
using TestBucket.Domain.Comments.Models;
using TestBucket.Domain.Labels.Models;
using TestBucket.Domain.Metrics.Models;

namespace TestBucket.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<ApplicationUser>(options)
{
    internal DbSet<Tenant> Tenants { get; set; }
    internal DbSet<RolePermission> RolePermissions { get; set; }
    internal DbSet<ProjectUserPermission> ProjectUserPermissions { get; set; }

    internal DbSet<UserPreferences> UserPreferences { get; set; }
    internal DbSet<ApplicationUserApiKey> ApiKeys { get; set; }
    internal DbSet<Comment> Comments { get; set; }

    internal DbSet<Metric> Metrics { get; set; }

    internal DbSet<FieldDefinition> FieldDefinitions { get; set; }
    internal DbSet<RequirementField> RequirementFields { get; set; }
    internal DbSet<IssueField> IssueFields { get; set; }
    internal DbSet<TestCaseField> TestCaseFields { get; set; }
    internal DbSet<TestRunField> TestRunFields { get; set; }
    internal DbSet<TestCaseRunField> TestCaseRunFields { get; set; }

    internal DbSet<Milestone> Milestones { get; set; }
    internal DbSet<Label> Labels { get; set; }

    internal DbSet<Repository> Repositories { get; set; }
    internal DbSet<Commit> Commits { get; set; }
    internal DbSet<CommitFile> CommitFiless { get; set; }
    internal DbSet<Feature> Features { get; set; }
    internal DbSet<Component> Components { get; set; }
    internal DbSet<ArchitecturalLayer> ArchitecturalLayers { get; set; }
    internal DbSet<ProductSystem> ProductSystems { get; set; }

    internal DbSet<FileResource> Files { get; set; }
    internal DbSet<Team> Teams { get; set; }
    internal DbSet<TestProject> Projects { get; set; }

    /// <summary>
    /// Information about other integrated systems
    /// </summary>
    internal DbSet<ExternalSystem> ExternalSystems { get; set; }
    internal DbSet<TestSuite> TestSuites { get; set; }
    internal DbSet<TestSuiteFolder> TestSuiteFolders { get; set; }
    internal DbSet<TestCase> TestCases { get; set; }
    internal DbSet<TestRun> TestRuns { get; set; }
    internal DbSet<TestCaseRun> TestCaseRuns { get; set; }
    internal DbSet<RequirementTestLink> RequirementTestLinks { get; set; }
    internal DbSet<RequirementSpecification> RequirementSpecifications { get; set; }
    internal DbSet<RequirementSpecificationFolder> RequirementSpecificationFolders { get; set; }
    internal DbSet<Requirement> Requirements { get; set; }

    /// <summary>
    /// Variable configuration for different test environments
    /// </summary>
    internal DbSet<TestEnvironment> TestEnvironments { get; set; }

    /// <summary>
    /// Settings common for all tenants
    /// </summary>
    internal DbSet<GlobalSettings> GlobalSettings { get; set; }

    internal DbSet<TestResource> TestResources { get; set; }
    internal DbSet<TestAccount> TestAccounts { get; set; }

    /// <summary>
    /// TB Runners
    /// </summary>
    internal DbSet<Runner> Runners { get; set; }

    /// <summary>
    /// TB Runner Jobs
    /// </summary>
    internal DbSet<Job> Jobs { get; set; }

    /// <summary>
    /// CI/CD pipelines
    /// </summary>
    internal DbSet<Pipeline> Pipelines { get; set; }

    /// <summary>
    /// CI/CD jobs within pipelines
    /// </summary>
    internal DbSet<PipelineJob> PipelineJobs { get; set; }

    /// <summary>
    /// Linkes between Issues and Tests
    /// </summary>
    internal DbSet<LinkedIssue> LinkedIssues { get; set; }

    /// <summary>
    /// Local issues stored in the DB.
    /// These may be copies of external issues
    /// </summary>
    internal DbSet<LocalIssue> LocalIssues { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
    }


}
