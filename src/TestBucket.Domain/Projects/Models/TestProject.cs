using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Contracts.Issues.States;
using TestBucket.Contracts.Requirements.States;
using TestBucket.Contracts.Testing.States;
using TestBucket.Domain.Identity.Models;
using TestBucket.Domain.Teams.Models;
using TestBucket.Domain.Tenants.Models;

namespace TestBucket.Domain.Projects.Models;

[Table("projects")]
[Index(nameof(TenantId), nameof(Slug))]
public record class TestProject
{
    /// <summary>
    /// DB ID
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// Slug
    /// </summary>
    public required string Slug { get; set; }

    /// <summary>
    /// Name
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// Abbreviation
    /// </summary>
    public required string ShortName { get; set; }

    /// <summary>
    /// Project description, this is fed into the chat context as a reference
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Image icon URL
    /// </summary>
    public string? IconUrl { get; set; }

    /// <summary>
    /// Automatically grant access for all tenant users
    /// </summary>
    public bool GrantAccessToAllTenantUsers { get; set; }

    /// <summary>
    /// Automatically grant access for all tenant users
    /// </summary>
    public bool GrantAccessToAllTeamUsers { get; set; }

    /// <summary>
    /// Number of test suites
    /// </summary>
    public int NumberOfTestSuites { get; set; }

    /// <summary>
    /// Total number of test cases
    /// </summary>
    public int NumberOfTestCases { get; set; }

    /// <summary>
    /// Total number of test cases
    /// </summary>
    public int NumberOfIssues { get; set; }

    /// <summary>
    /// Total number of open issues
    /// </summary>
    public int NumberOfOpenIssues { get; set; }

    /// <summary>
    /// Number of test runs
    /// </summary>
    public int NumberOfRuns { get; set; }

    // Navigation

    public string? TenantId { get; set; }
    public Tenant? Tenant { get; set; }

    public long? TeamId { get; set; }
    public Team? Team { get; set; }

    public IEnumerable<ExternalSystem>? ExternalSystems { get; set; }

    public IEnumerable<ApplicationUser>? ProjectMembers { get; set; }

}
