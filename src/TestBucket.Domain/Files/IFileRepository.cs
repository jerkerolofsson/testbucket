using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Domain.Files.Models;

namespace TestBucket.Domain.Files;
public interface IFileRepository
{
    /// <summary>
    /// Gets a resource
    /// </summary>
    /// <param name="tenantId"></param>
    /// <param name="id"></param>
    /// <returns></returns>
    Task<FileResource?> GetResourceByIdAsync(string tenantId, long id);

    /// <summary>
    /// Adds a resource
    /// </summary>
    /// <param name="resource"></param>
    /// <returns></returns>
    Task AddResourceAsync(FileResource resource);

    /// <summary>
    /// Deletes a resource
    /// </summary>
    /// <param name="tenantId"></param>
    /// <param name="id"></param>
    /// <returns></returns>
    Task DeleteResourceByIdAsync(string tenantId, long id);
}
