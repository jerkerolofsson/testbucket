using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestBucket.Contracts;

/// <summary>
/// Defines environment variable names used for initial seed.
/// Can be used by docker compose files, or by tests to setup the initial environment
/// </summary>
public static class TestBucketEnvironmentVariables
{
    /// <summary>
    /// Default Tenant
    /// </summary>
    public const string TB_DEFAULT_TENANT = nameof(TB_DEFAULT_TENANT);

    /// <summary>
    /// Default admin email
    /// </summary>
    public const string TB_ADMIN_USER = nameof(TB_ADMIN_USER);

    /// <summary>
    /// Default admin password
    /// </summary>
    public const string TB_ADMIN_PASSWORD = nameof(TB_ADMIN_PASSWORD);

    /// <summary>
    /// JWT symmetric key
    /// </summary>
    public const string TB_JWT_SYMMETRIC_KEY = nameof(TB_JWT_SYMMETRIC_KEY);

    /// <summary>
    /// JWT issuer
    /// </summary>
    public const string TB_JWT_ISS = nameof(TB_JWT_ISS);

    /// <summary>
    /// JWT audience
    /// </summary>
    public const string TB_JWT_AUD = nameof(TB_JWT_AUD);

    /// <summary>
    /// Remove?
    /// </summary>
    public const string TB_ADMIN_ACCESS_TOKEN = nameof(TB_ADMIN_ACCESS_TOKEN);

    /// <summary>
    /// Publicly accessible URL (in case the service is running behind a reverse-proxy)
    /// </summary>
    public const string TB_PUBLIC_ENDPOINT = nameof(TB_PUBLIC_ENDPOINT);

}
