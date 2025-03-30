using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

using TestBucket.Domain.Environments.Models;
using TestBucket.Domain.Fields.Models;
using TestBucket.Domain.Files.Models;
using TestBucket.Domain.Requirements.Models;
using TestBucket.Domain.Settings.Models;
using TestBucket.Domain.Teams.Models;

namespace TestBucket.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<ApplicationUser>(options)
{
    internal DbSet<FieldDefinition> FieldDefinitions { get; set; }
    internal DbSet<TestCaseField> TestCaseFields { get; set; }
    internal DbSet<TestRunField> TestRunFields { get; set; }
    internal DbSet<TestCaseRunField> TestCaseRunFields { get; set; }
    internal DbSet<ApplicationUserApiKey> ApiKeys { get; set; }
    internal DbSet<Tenant> Tenants { get; set; }
    internal DbSet<FileResource> Files { get; set; }
    internal DbSet<UserPreferences> UserPreferences { get; set; }
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
    /// 
    /// </summary>
    internal DbSet<TestEnvironment> TestEnvironments { get; set; }

    /// <summary>
    /// Settings common for all tenants
    /// </summary>
    internal DbSet<GlobalSettings> GlobalSettings { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
    }


}
