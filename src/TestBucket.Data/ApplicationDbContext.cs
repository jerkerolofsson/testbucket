using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

using TestBucket.Domain.Settings.Models;

namespace TestBucket.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<ApplicationUser>(options)
{
    internal DbSet<Tenant> Tenants { get; set; }
    internal DbSet<TestProject> Projects { get; set; }
    internal DbSet<TestSuite> TestSuites { get; set; }
    internal DbSet<TestSuiteFolder> TestSuiteFolders { get; set; }
    internal DbSet<TestCase> TestCases { get; set; }
    internal DbSet<TestRun> TestRuns { get; set; }
    internal DbSet<TestCaseRun> TestCaseRuns { get; set; }

    /// <summary>
    /// Settings common for all tenants
    /// </summary>
    internal DbSet<GlobalSettings> GlobalSettings { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
    }


}
