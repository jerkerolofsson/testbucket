using System;
using System.Collections.Generic;
using System.Linq;

using TestBucket.Domain.TestAccounts.Models;
using TestBucket.Domain.TestAccounts.Specifications;
using TestBucket.Domain.TestResources.Specifications;

using Xunit;

namespace TestBucket.Domain.UnitTests.TestAccounts;

/// <summary>
/// Unit tests for searching Test Accounts using various specifications.
/// </summary>
[UnitTest]
[EnrichedTest]
[FunctionalTest]
[Component("Test Accounts")]
[Feature("Search")]
public class TestAccountSearchTests
{
    /// <summary>
    /// Tests that the FindAccountByLockOwner specification returns accounts with a matching LockOwner.
    /// </summary>
    [Fact]
    public void FindAccountByLockOwner_ShouldReturnMatchingAccounts()
    {
        // Arrange
        var accounts = new List<TestAccount>
        {
            new TestAccount { Name = "Account1", LockOwner = "Owner1", Owner  = "a", Type = "email" },
            new TestAccount { Name = "Account2", LockOwner = "Owner2", Owner = "b", Type = "email" }
        };
        var spec = new FindAccountByLockOwner("Owner1");

        // Act
        var result = accounts.AsQueryable().Where(spec.Expression).ToList();

        // Assert
        Assert.Single(result);
        Assert.Equal("Account1", result[0].Name);
    }

    /// <summary>
    /// Tests that the FindAccountByLockOwner specification returns an empty list when no accounts match the LockOwner.
    /// </summary>
    [Fact]
    public void FindAccountByLockOwner_ShouldReturnEmptyForNonMatchingAccounts()
    {
        // Arrange
        var accounts = new List<TestAccount>
        {
            new TestAccount { Name = "Account1", LockOwner = "Owner1", Owner  = "a", Type = "email" },
            new TestAccount { Name = "Account2", LockOwner = "Owner2", Owner = "b", Type = "email" }
        };
        var spec = new FindAccountByLockOwner("NonExistentOwner");

        // Act
        var result = accounts.AsQueryable().Where(spec.Expression).ToList();

        // Assert
        Assert.Empty(result);
    }

    /// <summary>
    /// Tests that the FindLockedAccount specification returns accounts that are locked.
    /// </summary>
    [Fact]
    public void FindLockedAccount_ShouldReturnLockedAccounts()
    {
        // Arrange
        var accounts = new List<TestAccount>
        {
            new TestAccount { Name = "Account1", Locked = true, Owner  = "a", Type = "email" },
            new TestAccount { Name = "Account2", Locked = false, Owner  = "a", Type = "email" }
        };
        var spec = new FindLockedAccount();

        // Act
        var result = accounts.AsQueryable().Where(spec.Expression).ToList();

        // Assert
        Assert.Single(result);
        Assert.Equal("Account1", result[0].Name);
    }

    /// <summary>
    /// Tests that the FindLockedAccount specification returns an empty list when no accounts are locked.
    /// </summary>
    [Fact]
    public void FindLockedAccount_ShouldReturnEmptyForUnlockedAccounts()
    {
        // Arrange
        var accounts = new List<TestAccount>
        {
            new TestAccount { Name = "Account1", Locked = false, Owner  = "a", Type = "email" },
            new TestAccount { Name = "Account2", Locked = false, Owner  = "a", Type = "email" }
        };
        var spec = new FindLockedAccount();

        // Act
        var result = accounts.AsQueryable().Where(spec.Expression).ToList();

        // Assert
        Assert.Empty(result);
    }

    /// <summary>
    /// Tests that the FindAccountByType specification returns accounts with a matching Type.
    /// </summary>
    [Fact]
    public void FindAccountByType_ShouldReturnMatchingAccounts()
    {
        // Arrange
        var accounts = new List<TestAccount>
        {
            new TestAccount { Name = "Account1", Type = "Type1", Owner  = "a" },
            new TestAccount { Name = "Account2", Type = "Type2", Owner  = "a" }
        };
        var spec = new FindAccountByType("Type1");

        // Act
        var result = accounts.AsQueryable().Where(spec.Expression).ToList();

        // Assert
        Assert.Single(result);
        Assert.Equal("Account1", result[0].Name);
    }

    /// <summary>
    /// Tests that the FindAccountByType specification returns an empty list when no accounts match the Type.
    /// </summary>
    [Fact]
    public void FindAccountByType_ShouldReturnEmptyForNonMatchingAccounts()
    {
        // Arrange
        var accounts = new List<TestAccount>
        {
            new TestAccount { Name = "Account1", Type = "Type1", Owner  = "a" },
            new TestAccount { Name = "Account2", Type = "Type2", Owner  = "a" }
        };
        var spec = new FindAccountByType("NonExistentType");

        // Act
        var result = accounts.AsQueryable().Where(spec.Expression).ToList();

        // Assert
        Assert.Empty(result);
    }

    /// <summary>
    /// Tests that the FindEnabledAccount specification returns accounts that are enabled.
    /// </summary>
    [Fact]
    public void FindEnabledAccount_ShouldReturnEnabledAccounts()
    {
        // Arrange
        var accounts = new List<TestAccount>
        {
            new TestAccount { Name = "Account1", Enabled = true, Owner  = "a", Type = "email" },
            new TestAccount { Name = "Account2", Enabled = false, Owner  = "a", Type = "email" }
        };
        var spec = new FindEnabledAccount();

        // Act
        var result = accounts.AsQueryable().Where(spec.Expression).ToList();

        // Assert
        Assert.Single(result);
        Assert.Equal("Account1", result[0].Name);
    }

    /// <summary>
    /// Tests that the FindEnabledAccount specification returns an empty list when no accounts are enabled.
    /// </summary>
    [Fact]
    public void FindEnabledAccount_ShouldReturnEmptyForDisabledAccounts()
    {
        // Arrange
        var accounts = new List<TestAccount>
        {
            new TestAccount { Name = "Account1", Enabled = false, Owner  = "a", Type = "email" },
            new TestAccount { Name = "Account2", Enabled = false , Owner  = "a", Type = "email"}
        };
        var spec = new FindEnabledAccount();

        // Act
        var result = accounts.AsQueryable().Where(spec.Expression).ToList();

        // Assert
        Assert.Empty(result);
    }

    /// <summary>
    /// Tests that the FindUnlockedAccount specification returns accounts that are unlocked.
    /// </summary>
    [Fact]
    public void FindUnlockedAccount_ShouldReturnUnlockedAccounts()
    {
        // Arrange
        var accounts = new List<TestAccount>
        {
            new TestAccount { Name = "Account1", Locked = false, Owner  = "a", Type = "email" },
            new TestAccount { Name = "Account2", Locked = true, Owner  = "a", Type = "email" }
        };
        var spec = new FindUnlockedAccount();

        // Act
        var result = accounts.AsQueryable().Where(spec.Expression).ToList();

        // Assert
        Assert.Single(result);
        Assert.Equal("Account1", result[0].Name);
    }

    /// <summary>
    /// Tests that the FindUnlockedAccount specification returns an empty list when no accounts are unlocked.
    /// </summary>
    [Fact]
    public void FindUnlockedAccount_ShouldReturnEmptyForLockedAccounts()
    {
        // Arrange
        var accounts = new List<TestAccount>
        {
            new TestAccount { Name = "Account1", Locked = true, Owner  = "a", Type = "email" },
            new TestAccount { Name = "Account2", Locked = true, Owner  = "a", Type = "email" }
        };
        var spec = new FindUnlockedAccount();

        // Act
        var result = accounts.AsQueryable().Where(spec.Expression).ToList();

        // Assert
        Assert.Empty(result);
    }
}