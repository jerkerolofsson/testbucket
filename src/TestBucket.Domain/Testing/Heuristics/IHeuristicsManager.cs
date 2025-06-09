using TestBucket.Domain.Shared.Specifications;
using TestBucket.Domain.Testing.Heuristics.Models;

namespace TestBucket.Domain.Testing.Heuristics;
public interface IHeuristicsManager
{
    Task AddAsync(ClaimsPrincipal principal, Heuristic heuristic);
    Task DeleteAsync(ClaimsPrincipal principal, Heuristic heuristic);
    Task UpdateAsync(ClaimsPrincipal principal, Heuristic heuristic);
    Task<PagedResult<Heuristic>> SearchAsync(ClaimsPrincipal principal, FilterSpecification<Heuristic>[] filters, int offset, int count);
}
