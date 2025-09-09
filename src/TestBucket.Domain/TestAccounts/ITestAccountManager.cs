using System.Security.Claims;

using TestBucket.Contracts.TestAccounts;
using TestBucket.Domain.TestAccounts.Models;

namespace TestBucket.Domain.TestAccounts;
public interface ITestAccountManager
{
    Task AddAsync(ClaimsPrincipal principal, TestAccount account);
    Task<PagedResult<TestAccount>> SearchAsync(ClaimsPrincipal principal, string text, int offset, int count);
    Task<PagedResult<TestAccount>> BrowseAsync(ClaimsPrincipal principal, int offset, int count);
    Task DeleteAsync(ClaimsPrincipal principal, TestAccount account);
    Task<TestAccount?> GetAccountByIdAsync(ClaimsPrincipal principal, long id);
    Task UpdateAsync(ClaimsPrincipal principal, TestAccount account);
}