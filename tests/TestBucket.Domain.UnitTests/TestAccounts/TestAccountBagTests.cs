using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using NSubstitute;
using TestBucket.Domain.TestAccounts;
using TestBucket.Domain.TestAccounts.Allocation;
using TestBucket.Domain.TestAccounts.Models;

namespace TestBucket.Domain.UnitTests.TestAccounts
{
    /// <summary>
    /// Unit tests for <see cref="TestAccountBag"/>.
    /// </summary>
    [UnitTest]
    [EnrichedTest]
    [FunctionalTest]
    [Component("Test Accounts")]
    public class TestAccountBagTests
    {
        /// <summary>
        /// Verifies that <see cref="TestAccountBag.AddAsync"/> adds the account and sets lock properties.
        /// </summary>
        [Fact]
        public async Task AddAsync_Should_Add_Account_And_Set_Lock_Properties()
        {
            // Arrange
            var principal = new ClaimsPrincipal();
            var manager = Substitute.For<ITestAccountManager>();
            var bag = new TestAccountBag(principal, manager);
            var account = new TestAccount { Name = "A", Type = "wifi", Owner = "owner", Enabled = true };
            var lockExpires = DateTimeOffset.UtcNow.AddMinutes(5);
            var lockOwner = "test-owner";

            // Act
            await bag.AddAsync(account, lockExpires, lockOwner);

            // Assert
            Assert.Contains(account, bag.Accounts);
            Assert.True(account.Locked);
            Assert.Equal(lockOwner, account.LockOwner);
            Assert.Equal(lockExpires, account.LockExpires);
            await manager.Received(1).UpdateAsync(principal, account);
        }

        /// <summary>
        /// Verifies that <see cref="TestAccountBag.ResolveVariables"/> populates the variables dictionary as expected.
        /// </summary>
        [Fact]
        public async Task ResolveVariables_Should_Populate_Variables_Dictionary()
        {
            // Arrange
            var principal = new ClaimsPrincipal();
            var manager = Substitute.For<ITestAccountManager>();
            var bag = new TestAccountBag(principal, manager);
            var account = new TestAccount
            {
                Name = "A",
                Type = "wifi",
                Owner = "owner",
                Enabled = true,
                Variables = new Dictionary<string, string> { { "username", "user1" }, { "password", "pass1" } }
            };
            await bag.AddAsync(account, DateTimeOffset.UtcNow, "owner");
            var variables = new Dictionary<string, string>();

            // Act
            bag.ResolveVariables(account, variables);

            // Assert
            Assert.Equal("user1", variables["accounts__wifi__0__username"]);
            Assert.Equal("pass1", variables["accounts__wifi__0__password"]);
        }
    }
}
