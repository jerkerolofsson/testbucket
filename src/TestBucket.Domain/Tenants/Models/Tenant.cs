using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Domain.Projects.Models;

namespace TestBucket.Domain.Tenants.Models;
public class Tenant
{
    public required string Id { get; set; }
    public required string Name { get; set; }
    public string? IconUrl { get; set; }


    public bool CanRegisterNewUsers { get; set; }
    public bool RequireConfirmedAccount { get; set; }


    // Navigation

    public IEnumerable<TestProject>? TestProjects { get; set; }
}
