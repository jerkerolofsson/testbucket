using System.Security.Claims;

using TestBucket.Contracts.TestAccounts;
using TestBucket.Domain.TestAccounts.Models;

namespace TestBucket.Domain.TestAccounts;
public interface ITestAccountManager
{
    //Task<TestAccountDto> AddAsync(ClaimsPrincipal principal, TestAccountDto account);
    //Task UnlockAsync(ClaimsPrincipal principal, TestAccountDto account);
    //Task<TestAccountDto> LockAsync(ClaimsPrincipal principal, string type);

    Task AddAsync(ClaimsPrincipal principal, TestAccount account);
    Task<PagedResult<TestAccount>> BrowseAsync(ClaimsPrincipal principal, int offset, int count);
    Task DeleteAsync(ClaimsPrincipal principal, TestAccount account);
    Task<TestAccount?> GetAccountByIdAsync(ClaimsPrincipal principal, long id);
    Task UpdateAsync(ClaimsPrincipal principal, TestAccount account);
}