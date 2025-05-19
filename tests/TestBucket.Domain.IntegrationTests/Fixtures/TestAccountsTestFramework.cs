using Mediator;
using TestBucket.Contracts;
using TestBucket.Domain.TestAccounts;
using TestBucket.Domain.TestAccounts.Allocation;
using TestBucket.Domain.TestAccounts.Models;

namespace TestBucket.Domain.IntegrationTests.Fixtures
{
    public class TestAccountsTestFramework(ProjectFixture Fixture)
    {
        internal async Task<TestAccount> AddTestAccountAsync(string owner, string type)
        {
            var manager = Fixture.Services.GetRequiredService<ITestAccountManager>();

            var account = new TestAccount { Name = Guid.NewGuid().ToString(), Type = type, Owner = owner };
            await manager.AddAsync(Impersonation.Impersonate(Fixture.App.Tenant), account);
            return account;
        }
        internal async Task<TestAccount> AddTestAccountAsync(string owner, string type, Dictionary<string,string> variables)
        {
            var manager = Fixture.Services.GetRequiredService<ITestAccountManager>();

            var account = new TestAccount { Name = Guid.NewGuid().ToString(), Type = type, Owner = owner, Variables = variables };
            await manager.AddAsync(Impersonation.Impersonate(Fixture.App.Tenant), account);
            return account;
        }


        internal async Task DeleteAccountAsync(TestAccount account)
        {
            var manager = Fixture.Services.GetRequiredService<ITestAccountManager>();

            await manager.DeleteAsync(Impersonation.Impersonate(Fixture.App.Tenant), account);
        }

        internal async Task ReleaseAsync(string owner)
        {
            var princiapal = Impersonation.Impersonate(Fixture.App.Tenant);
            var mediator = Fixture.Services.GetRequiredService<IMediator>();
            await mediator.Send(new ReleaseAccountsRequest(owner, princiapal.GetTenantIdOrThrow()));
        }

        internal async Task<TestAccountBag> AllocateAsync(TestRun run, TestExecutionContext context)
        {
            var allocator = Fixture.Services.GetRequiredService<TestAccountDependencyAllocator>();
            var accountBag = await allocator.CollectDependenciesAsync(Impersonation.Impersonate(Fixture.App.Tenant), context);
            return accountBag;

        }
        internal async Task<TestAccountBag> AllocateAsync(TestRun run, string guid, IEnumerable<TestCaseDependency> dependencies)
        {
            var allocator = Fixture.Services.GetRequiredService<TestAccountDependencyAllocator>();

            var context = new Contracts.Testing.Models.TestExecutionContext
            {
                Guid = guid,
                ProjectId = Fixture.ProjectId,
                TeamId = Fixture.TeamId,
                TestRunId = run.Id,
                Dependencies = new List<TestCaseDependency>(dependencies)
            };
            return await allocator.CollectDependenciesAsync(Impersonation.Impersonate(Fixture.App.Tenant), context);
        }

        internal async Task<TestAccount?> GetByIdAsync(long id)
        {
            var manager = Fixture.Services.GetRequiredService<ITestAccountManager>();

            return await manager.GetAccountByIdAsync(Impersonation.Impersonate(Fixture.App.Tenant), id);
        }
        internal async Task<PagedResult<TestAccount>> BrowseAsync()
        {
            var manager = Fixture.Services.GetRequiredService<ITestAccountManager>();
            return await manager.BrowseAsync(Impersonation.Impersonate(Fixture.App.Tenant), 0, 100);
        }

        internal async Task UpdateAsync(TestAccount account1)
        {
            var manager = Fixture.Services.GetRequiredService<ITestAccountManager>();
            await manager.UpdateAsync(Impersonation.Impersonate(Fixture.App.Tenant), account1);
        }
    }
}
