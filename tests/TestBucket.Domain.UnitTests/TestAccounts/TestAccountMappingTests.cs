using TestBucket.Contracts.TestAccounts;
using TestBucket.Domain.TestAccounts.Mapping;
using TestBucket.Domain.TestAccounts.Models;

namespace TestBucket.Domain.UnitTests.TestAccounts;

/// <summary>
/// Unit tests for verifying the mapping logic of TestAccount to DTO and DBO objects.
/// </summary>
[UnitTest]
[EnrichedTest]
[FunctionalTest]
[Component("Test Accounts")]
public class TestAccountMappingTests
{
    /// <summary>
    /// Tests that the TestAccount.ToDto method correctly maps all properties from a TestAccount to a DTO.
    /// </summary>
    [Fact]
    public void ToDto_ShouldMapAllPropertiesCorrectly()
    {
        // Arrange
        var testAccount = new TestAccount
        {
            Name = "TestAccount1",
            Variables = new Dictionary<string, string> { { "Key", "Value" } },
            Type = "Email",
            SubType = "Gmail",
            Owner = "Owner1",
            TenantId = "Tenant1",
            Enabled = true,
            Locked = false,
            ModifiedBy = "User1",
            CreatedBy = "User2",
            Created = DateTime.UtcNow.AddDays(-1),
            Modified = DateTime.UtcNow,
            LockExpires = DateTime.UtcNow.AddHours(1),
            LockOwner = "LockOwner1"
        };

        // Act
        var dto = testAccount.ToDto();

        // Assert
        Assert.Equal(testAccount.Name, dto.Name);
        Assert.Equal(testAccount.Variables, dto.Variables);
        Assert.Equal(testAccount.Type, dto.Type);
        Assert.Equal(testAccount.SubType, dto.SubType);
        Assert.Equal(testAccount.Owner, dto.Owner);
        Assert.Equal(testAccount.TenantId, dto.Tenant);
        Assert.Equal(testAccount.Enabled, dto.Enabled);
        Assert.Equal(testAccount.Locked, dto.Locked);
        Assert.Equal(testAccount.ModifiedBy, dto.ModifiedBy);
        Assert.Equal(testAccount.CreatedBy, dto.CreatedBy);
        Assert.Equal(testAccount.Created, dto.Created);
        Assert.Equal(testAccount.Modified, dto.Modified);
        Assert.Equal(testAccount.LockExpires, dto.LockExpires);
        Assert.Equal(testAccount.LockOwner, dto.LockOwner);
    }

    /// <summary>
    /// Tests that the TestAccountDto.ToDbo method correctly maps all properties from a <see cref="TestAccountDto"/> to a DBO.
    /// </summary>
    [Fact]
    public void ToDbo_ShouldMapAllPropertiesCorrectly()
    {
        // Arrange
        var testAccountDto = new TestAccountDto
        {
            Name = "TestAccount1",
            Variables = new Dictionary<string, string> { { "Key", "Value" } },
            Type = "Email",
            SubType = "Gmail",
            Owner = null,
            Tenant = "Tenant1",
            Enabled = true,
            Locked = false,
            ModifiedBy = "User1",
            CreatedBy = "User2",
            Created = DateTime.UtcNow.AddDays(-1),
            Modified = DateTime.UtcNow,
            LockExpires = DateTime.UtcNow.AddHours(1),
            LockOwner = "LockOwner1"
        };

        // Act
        var dbo = testAccountDto.ToDbo();

        // Assert
        Assert.Equal(testAccountDto.Name, dbo.Name);
        Assert.Equal(testAccountDto.Variables, dbo.Variables);
        Assert.Equal(testAccountDto.Type, dbo.Type);
        Assert.Equal(testAccountDto.SubType, dbo.SubType);
        Assert.Equal("internal", dbo.Owner); // Default value for Owner
        Assert.Equal(testAccountDto.Tenant, dbo.TenantId);
        Assert.Equal(testAccountDto.Enabled, dbo.Enabled);
        Assert.Equal(testAccountDto.Locked, dbo.Locked);
        Assert.Equal(testAccountDto.ModifiedBy, dbo.ModifiedBy);
        Assert.Equal(testAccountDto.CreatedBy, dbo.CreatedBy);
        Assert.Equal(testAccountDto.Created, dbo.Created);
        Assert.Equal(testAccountDto.Modified, dbo.Modified);
        Assert.Equal(testAccountDto.LockExpires, dbo.LockExpires);
        Assert.Equal(testAccountDto.LockOwner, dbo.LockOwner);
    }
}