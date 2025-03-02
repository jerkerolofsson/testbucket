using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestBucket.Domain.Identity.Models;
public class UserPreferences
{
    public long Id { get; set; }

    /// <summary>
    /// Tenant
    /// </summary>
    public required string TenantId { get; set; }

    /// <summary>
    /// Authenticated Identity.Name
    /// </summary>
    public required string UserName { get; set; }

    /// <summary>
    /// Active team
    /// </summary>
    public long? ActiveTeamId { get; set; }

    /// <summary>
    /// Active project
    /// </summary>
    public long? ActiveProjectId { get; set; }

    /// <summary>
    /// Use dark mode
    /// </summary>
    public bool DarkMode { get; set; } = true;
}
