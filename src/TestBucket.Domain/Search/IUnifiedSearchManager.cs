
using System.Security.Claims;

using TestBucket.Domain.Search.Models;

namespace TestBucket.Domain.Search;
public interface IUnifiedSearchManager
{
    Task<List<SearchResult>> SearchAsync(ClaimsPrincipal principal, TestProject? testProject, string text, CancellationToken cancellationToken);
}