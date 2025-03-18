using System.Security.Claims;

using TestBucket.Domain.Files.Models;
using TestBucket.Domain.Requirements.Models;

namespace TestBucket.Domain.Requirements.Import;
public interface IRequirementImporter
{
    /// <summary>
    /// Imports a file as a specification. 
    /// 
    /// This will take the whole document and convert it to markdown as RequirementSpecification.Description
    /// </summary>
    /// <param name="principal"></param>
    /// <param name="teamId"></param>
    /// <param name="testProjectId"></param>
    /// <param name="fileResource"></param>
    /// <returns></returns>
    Task<RequirementSpecification?> ImportFileAsync(ClaimsPrincipal principal, long? teamId, long? testProjectId, FileResource fileResource);

    /// <summary>
    /// Parses the requirement specification and extracts individual requirements
    /// </summary>
    /// <param name="specification"></param>
    /// <returns></returns>
    Task<IReadOnlyList<Requirement>> ExtractRequirementsAsync(RequirementSpecification specification, CancellationToken cancellationToken);
}