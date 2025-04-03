using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestBucket.Domain.Identity;

/// <summary>
/// Global roles
/// </summary>
public static class Roles
{
    /// <summary>
    /// Regular user
    /// </summary>
    public const string REGULAR_USER = nameof(REGULAR_USER);

    /// <summary>
    /// Read-onmly user
    /// </summary>
    public const string READ_ONLY = nameof(READ_ONLY);

    /// <summary>
    /// Can manage tenants
    /// </summary>
    public const string SUPERADMIN = nameof(SUPERADMIN);

    /// <summary>
    /// Administrator for the site, e.g. for one specific tenant
    /// Can manage users, roles and projects
    /// </summary>
    public const string ADMIN = nameof(ADMIN);
}
