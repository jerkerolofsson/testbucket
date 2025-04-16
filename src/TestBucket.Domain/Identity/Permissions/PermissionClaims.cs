using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Domain.Testing.Models;

namespace TestBucket.Domain.Identity.Permissions
{
    public static class PermissionClaims
    {
        public const string RequirementSpecification = "tbp-rs";
        public const string Requirement = "tbp-r";

        public const string TestSuite = "tbp-ts";
        public const string TestCase = "tbp-tc";

        public const string TestRun = "tbp-tr";
        public const string TestCaseRun = "tbp-tcr";

        public const string User = "tbp-u";
        public const string Project = "tbp-p";
        public const string Tenant = "tbp-tenant";

        public static void ThrowIfNoPermission(this ClaimsPrincipal principal, PermissionEntityType entityType, PermissionLevel requiredLevel)
        {
            if (!HasPermission(principal, entityType, requiredLevel))
            {
                throw new UnauthorizedAccessException($"The user {principal.Identity?.Name} does not have the required access ({requiredLevel}) for entities of type {entityType}");
            }
        }

        public static bool HasPermission(this ClaimsPrincipal principal, PermissionEntityType entityType, PermissionLevel requiredLevel)
        {
            // Scan all claims, it is possible that a user has multiple claims of the same type
            // if the user belongs to multiple roles.
            foreach (var level in GetPermissionLevelsFromClaims(principal.Claims, entityType))
            {
                if ((level & requiredLevel) == requiredLevel)
                {
                    return true;
                }
            }
            return false;
        }

        //public static bool HasPermission(IEnumerable<Claim> claims, PermissionEntityType entityType, PermissionLevel requiredLevel)
        //{
        //    var userLevel = GetPermissionLevelFromClaims(claims, entityType);
        //    return (userLevel & requiredLevel) == requiredLevel;
        //}

        /// <summary>
        /// Searches the claims for a permission claim type based on the specified entity and returns it
        /// </summary>
        /// <param name="claims"></param>
        /// <param name="entityType"></param>
        /// <returns></returns>
        public static PermissionLevel GetPermissionLevelFromClaims(IEnumerable<Claim> claims, PermissionEntityType entityType)
        {
            var claimType = GetClaimTypeFromEntity(entityType);
            if(claimType is not null)
            {
                var claim = claims.Where(x => x.Type == claimType).FirstOrDefault();
                if(claim?.Value is not null && int.TryParse(claim.Value, out var levelAsInteger))
                {
                    return (PermissionLevel)levelAsInteger;
                }
            }

            return PermissionLevel.None;
        }

        public static IEnumerable<PermissionLevel> GetPermissionLevelsFromClaims(IEnumerable<Claim> claims, PermissionEntityType entityType)
        {
            var claimType = GetClaimTypeFromEntity(entityType);
            if (claimType is not null)
            {
                var claim = claims.Where(x => x.Type == claimType).FirstOrDefault();
                if (claim?.Value is not null && int.TryParse(claim.Value, out var levelAsInteger))
                {
                    yield return (PermissionLevel)levelAsInteger;
                }
            }
        }

        public static string? GetClaimTypeFromEntity(PermissionEntityType entityType)
        {
            return entityType switch
            {
                PermissionEntityType.RequirementSpecification => RequirementSpecification,
                PermissionEntityType.Requirement => Requirement,
                PermissionEntityType.TestCase => TestCase,
                PermissionEntityType.TestSuite => TestSuite,
                PermissionEntityType.TestRun => TestRun,
                PermissionEntityType.TestCaseRun => TestCaseRun,
                PermissionEntityType.Tenant => Tenant,
                PermissionEntityType.User => User,
                PermissionEntityType.Project => Project,
                _ => null
            };
        }
    }
}
