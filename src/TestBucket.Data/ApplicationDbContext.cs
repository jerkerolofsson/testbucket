using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

using TestBucket.Data.Identity.Models;
using TestBucket.Data.Projects.Models;

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


}
