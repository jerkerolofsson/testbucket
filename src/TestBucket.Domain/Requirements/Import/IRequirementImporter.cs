using System.Security.Claims;

using TestBucket.Domain.Files.Models;
using TestBucket.Domain.Requirements.Models;

namespace TestBucket.Domain.Requirements.Import;
public interface IRequirementImporter
{
    Task<RequirementSpecification?> ImportAsync(ClaimsPrincipal principal, long? teamId, long? testProjectId, FileResource fileResource);
}