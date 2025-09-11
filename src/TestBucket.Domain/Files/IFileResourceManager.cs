using TestBucket.Domain.Files.Models;

namespace TestBucket.Domain.Files;
public interface IFileResourceManager
{
    /// <summary>
    /// Adds a file resource
    /// </summary>
    /// <param name="principal"></param>
    /// <param name="resource"></param>
    /// <returns></returns>
    Task AddResourceAsync(ClaimsPrincipal principal, FileResource resource);
}