using Mediator;

using Microsoft.Extensions.Caching.Memory;

using TestBucket.Contracts.Fields;
using TestBucket.Domain.Fields;
using TestBucket.Domain.Identity;
using TestBucket.Domain.Issues.Models;
using TestBucket.Domain.UnitTests.Fakes.NewFolder;

namespace TestBucket.Domain.UnitTests.Fields;

/// <summary>
/// Tests for FieldManager
/// </summary>
[Feature("Custom Fields")]
[UnitTest]
[Component("Fields")]
[EnrichedTest]
[FunctionalTest]
public class FieldManagerTests
{
    private readonly IMediator _mediator = NSubstitute.Substitute.For<IMediator>();
    private readonly IMemoryCache _memoryCache = NSubstitute.Substitute.For<IMemoryCache>();
    private const string _tenantId = "tenant-1";    


    /// <summary>
    /// Verifies that GetIssueFields only returns fields with definitions with Target=Issue
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task GetIssueFieldsAsync_ShouldReturnFieldsWithDefaults()
    {
        var repo = new FakeFieldRepository();
        var fieldManager = new FieldManager(repo, _mediator, _memoryCache);
        var fieldDefinitionManager = new FieldDefinitionManager(_memoryCache, [], repo);

        // Arrange
        var principal = Impersonation.Impersonate(_tenantId);
        
        await repo.AddAsync(new FieldDefinition { Id = 1, Name = "Field1", TenantId = _tenantId, Target = FieldTarget.Issue });
        await repo.AddAsync(new FieldDefinition { Id = 2, Name = "Field2", TenantId = _tenantId, Target = FieldTarget.Issue });
        await repo.AddAsync(new FieldDefinition { Id = 3, Name = "Field3", TenantId = _tenantId, Target = FieldTarget.Requirement });

        var fieldDefinitions = await fieldDefinitionManager.GetDefinitionsAsync(principal, null, FieldTarget.Issue);

        // Act
        var fields = await fieldManager.GetIssueFieldsAsync(principal, 1, fieldDefinitions);

        // Assert
        Assert.NotNull(fields);
        Assert.Equal(2, fields.Count);
        Assert.Contains(fields, f => f.FieldDefinitionId == 1);
        Assert.Contains(fields, f => f.FieldDefinitionId == 2);
    }

    /// <summary>
    /// Verifies that upsert adds a field
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task UpsertIssueFieldAsync_ShouldAddOrUpdateField()
    {
        // Arrange
        var repo = new FakeFieldRepository();
        var fieldManager = new FieldManager(repo, _mediator, _memoryCache);
        var principal = Impersonation.Impersonate(_tenantId);
        var fieldDefinition = new FieldDefinition
        {
            Id = 1,
            Name = "Test Field",
            TenantId = _tenantId,
            Target = FieldTarget.Issue
        };
        await repo.AddAsync(fieldDefinition);
        var field = new IssueField
        {
            FieldDefinitionId = 1,
            LocalIssueId = 1,
            TenantId = _tenantId
        };

        // Act
        await fieldManager.UpsertIssueFieldAsync(principal, field);

        // Assert
        var storedField = await repo.GetIssueFieldsAsync("tenant-1", 1);
        Assert.Contains(storedField, f => f.FieldDefinitionId == 1);
    }
}
