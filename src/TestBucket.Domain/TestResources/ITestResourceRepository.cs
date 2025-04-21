using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Domain.Shared.Specifications;
using TestBucket.Domain.TestResources.Models;

namespace TestBucket.Domain.TestResources;
public interface ITestResourceRepository
{
    Task AddAsync(TestResource resource);
    Task<TestResource?> GetByIdAsync(string tenantId, long id);
    Task DeleteAsync(long id);
    Task<PagedResult<TestResource>> SearchAsync(FilterSpecification<TestResource>[] filters, int offset, int count);
    Task UpdateAsync(TestResource resource);
}
