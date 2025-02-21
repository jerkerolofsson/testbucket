using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Data.Identity.Models;

namespace TestBucket.Data.Projects.Models;

[Table("projects")]
public class TestProject
{
    public long Id { get; set; }
    public required string Slug { get; set; }
    public required string Name { get; set; }

    public required string ShortName { get; set; }

    public string? IconUrl { get; set; }

    /// <summary>
    /// Automatically grant access for all tenant users
    /// </summary>
    public bool GrantAccessToAllTenantUsers { get; set; }

    // Navigation

    public string? TenantId { get; set; }
    public Tenant? Tenant { get; set; }

    public IEnumerable<ApplicationUser>? ProjectMembers { get; set; }

}
