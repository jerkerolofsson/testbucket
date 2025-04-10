using System.Security.Claims;

using TestBucket.Domain.Files.Models;

namespace TestBucket.Domain.Files;
public interface IFileResourceManager
{
    Task AddResourceAsync(ClaimsPrincipal principal, FileResource resource);
}