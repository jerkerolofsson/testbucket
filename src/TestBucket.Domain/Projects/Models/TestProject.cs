using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Domain.Identity.Models;
using TestBucket.Domain.Teams.Models;
using TestBucket.Domain.Tenants.Models;

namespace TestBucket.Domain.Projects.Models;

[Table("projects")]
public class TestProject
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

    // Navigation

    public string? TenantId { get; set; }
    public Tenant? Tenant { get; set; }

    public long? TeamId { get; set; }
    public Team? Team { get; set; }

    public IEnumerable<ApplicationUser>? ProjectMembers { get; set; }

}
