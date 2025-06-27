using TestBucket.Domain.Shared.Specifications;
using TestBucket.Domain.Testing.Heuristics.Models;

namespace TestBucket.Domain.Testing.Heuristics;
public interface IHeuristicsManager
{
    /// <summary>
    /// Adds a hueristic to the repository 
    /// This requires the Write permission
    /// 
    /// Created, Modified, CreatedBy and ModifiedBy will be set automatically
    /// </summary>
    /// <param name="principal"></param>
    /// <param name="heuristic"></param>
    /// <returns></returns>
    Task AddAsync(ClaimsPrincipal principal, Heuristic heuristic);

    /// <summary>
    /// Deletes a heuristic from the repository
    /// This requires the Delete permission
    /// </summary>
    /// <param name="principal"></param>
    /// <param name="heuristic"></param>
    /// <returns></returns>
    Task DeleteAsync(ClaimsPrincipal principal, Heuristic heuristic);

    /// <summary>
    /// This requires the Write permission
    /// 
    /// Modified and ModifiedBy will be set automatically
    /// </summary>
    /// <param name="principal"></param>
    /// <param name="heuristic"></param>
    /// <returns></returns>
    Task UpdateAsync(ClaimsPrincipal principal, Heuristic heuristic);


    /// <summary>
    /// Searches for heuristics based on the provided fillters.
    /// 
    /// This requires Read permission
    /// </summary>
    /// <param name="principal"></param>
    /// <param name="filters"></param>
    /// <param name="offset"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    Task<PagedResult<Heuristic>> SearchAsync(ClaimsPrincipal principal, FilterSpecification<Heuristic>[] filters, int offset, int count);
}
