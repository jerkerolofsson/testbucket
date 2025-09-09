using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Mediator;

using TestBucket.Domain.Shared.Specifications;
using TestBucket.Domain.TestAccounts;
using TestBucket.Domain.TestAccounts.Models;
using TestBucket.Domain.TestAccounts.Specifications;
using TestBucket.Domain.Testing.Models;
using TestBucket.Domain.TestResources.Allocation;
using TestBucket.Domain.TestResources.Specifications;

namespace TestBucket.Domain.TestAccounts.Allocation;

/// <summary>
/// Collects resources required to run a test case and returns a "bag" that has those resources
/// </summary>
public class TestAccountDependencyAllocator
{
    private readonly ITestAccountManager _testResourceManager;
    private readonly ITestAccountRepository _resourceRepository;
    private readonly IMediator _mediator;
    private static readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1);

    public TestAccountDependencyAllocator(ITestAccountManager testResourceManager, ITestAccountRepository resourceRepository, IMediator mediator)
    {
        _testResourceManager = testResourceManager;
        _resourceRepository = resourceRepository;
        _mediator = mediator;
    }

    public async Task<TestAccountBag> CollectDependenciesAsync(
        ClaimsPrincipal principal, 
        TestExecutionContext context,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(principal);
        ArgumentNullException.ThrowIfNull(context);

        var bag = new TestAccountBag(principal, _testResourceManager);

        await _semaphore.WaitAsync(cancellationToken);
        try
        {
            if (context.Dependencies is not null)
            {
                foreach (var dependency in context.Dependencies)
                {
                    if (dependency.AccountType is not null)
                    {
                        TestAccount? account = await AllocateAccountAsync(principal, dependency.AccountType);
                        if(account is null)
                        {
                            context.CompilerErrors.Add(new CompilerError 
                            { 
                                Code = 123,
                                Message = $"Failed to allocate account of type {dependency.AccountType}",
                                Line = -1,
                                Column = -1,
                            });
                        }
                        else
                        {
                            await bag.AddAsync(account, context.ResourceExpiry, context.Guid);
                            bag.ResolveVariables(account,context.Variables);
                        }
                    }
                }
            }
        }
        catch(UnauthorizedAccessException)
        {
            await _mediator.Send(new ReleaseAccountsRequest(context.Guid, principal.GetTenantIdOrThrow()));
            throw;
        }
        catch
        {
            // Release any resources already allocated
            await _mediator.Send(new ReleaseAccountsRequest(context.Guid, principal.GetTenantIdOrThrow()));
        }
        finally
        {
            _semaphore.Release();
        }

        return bag;
    }

    private async Task<TestAccount?> AllocateAccountAsync(ClaimsPrincipal principal, string resourceType)
    {
        FilterSpecification<TestAccount>[] filters = [
            new FindUnlockedAccount(),
            new FindEnabledAccount(),
            new FindAccountByType(resourceType),
            new FilterByTenant<TestAccount>(principal.GetTenantIdOrThrow())
        ];

        principal.ThrowIfNoPermission(PermissionEntityType.TestAccount, PermissionLevel.Read);

        var page = await _resourceRepository.SearchAsync(filters, 0, 1);
        if(page.Items.Length > 0)
        {
            return page.Items[0];
        }
        return null;
    }
}
