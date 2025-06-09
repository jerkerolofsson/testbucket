using TestBucket.Domain.Shared.Specifications;
using TestBucket.Domain.Testing.Heuristics.Models;

namespace TestBucket.Domain.Testing.Heuristics;

/// <summary>
/// Repository interface for managing <see cref="Heuristic"/> entities.
/// </summary>
public interface IHeuristicsRepository
{
    /// <summary>
    /// Deletes a heuristic by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the heuristic to delete.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task DeleteAsync(long id);

    /// <summary>
    /// Adds a new heuristic to the repository.
    /// </summary>
    /// <param name="account">The <see cref="Heuristic"/> entity to add.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task AddAsync(Heuristic account);

    /// <summary>
    /// Updates an existing heuristic in the repository.
    /// </summary>
    /// <param name="account">The <see cref="Heuristic"/> entity to update.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task UpdateAsync(Heuristic account);

    /// <summary>
    /// Searches for heuristics matching the specified filters, with paging support.
    /// </summary>
    /// <param name="filters">An array of filter specifications to apply.</param>
    /// <param name="offset">The number of items to skip before starting to collect the result set.</param>
    /// <param name="count">The maximum number of items to return.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains a <see cref="PagedResult{Heuristic}"/>
    /// with the search results.
    /// </returns>
    Task<PagedResult<Heuristic>> SearchAsync(FilterSpecification<Heuristic>[] filters, int offset, int count);

    /// <summary>
    /// Gets a heuristic by its tenant and unique identifier.
    /// </summary>
    /// <param name="tenantId">The tenant identifier.</param>
    /// <param name="id">The unique identifier of the heuristic.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains the <see cref="Heuristic"/> if found; otherwise, <c>null</c>.
    /// </returns>
    Task<Heuristic?> GetByIdAsync(string tenantId, long id);
}