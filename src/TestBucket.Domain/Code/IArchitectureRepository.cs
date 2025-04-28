using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Domain.Code.Models;
using TestBucket.Domain.Shared.Specifications;

namespace TestBucket.Domain.Code;

/// <summary>
/// Persistance storage for code architecture components
/// </summary>
public interface IArchitectureRepository
{
    Task AddSystemAsync(ProductSystem system);
    Task<PagedResult<ProductSystem>> SearchSystemsAsync(FilterSpecification<ProductSystem>[] filters, int offset, int count);
    Task UpdateSystemAsync(ProductSystem existingSystem);
}
