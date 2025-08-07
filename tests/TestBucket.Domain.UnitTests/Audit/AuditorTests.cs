using NSubstitute;

using TestBucket.Domain.Audit;
using TestBucket.Domain.Audit.Models;

namespace TestBucket.Domain.UnitTests.Audit;
/// <summary>
/// Unit tests for the <see cref="Auditor"/> class.
/// </summary>
[Feature("Review")]
[Component("Auditor")]
[UnitTest]
[EnrichedTest]
[FunctionalTest]
public class AuditorTests
{
    /// <summary>
    /// Simple test entity for audit testing.
    /// </summary>
    private class TestEntity
    {
        /// <summary>
        /// Gets or sets the name of the entity.
        /// </summary>
        public string Name { get; set; } = string.Empty;
        /// <summary>
        /// Gets or sets the value of the entity.
        /// </summary>
        public int Value { get; set; }
    }

    private readonly IAuditRepository _repository = Substitute.For<IAuditRepository>();
    private readonly FakeTimeProvider _timeProvider = new FakeTimeProvider(new DateTimeOffset(2024, 1, 1, 0, 0, 0, TimeSpan.Zero));
    private readonly Auditor _auditor;

    /// <summary>
    /// Creates sut
    /// </summary>
    public AuditorTests()
    {
        _auditor = new Auditor(_repository, _timeProvider);
    }

    /// <summary>
    /// Verifies that CreateEntry returns null if no properties are provided.
    /// </summary>
    [Fact]
    public void CreateEntry_ReturnsNull_IfNoProperties()
    {
        var oldEntity = new TestEntity { Name = "A", Value = 1 };
        var newEntity = new TestEntity { Name = "A", Value = 1 };
        var result = _auditor.CreateEntry(Array.Empty<string>(), 1, 2, oldEntity, newEntity);
        Assert.Null(result);
    }

    /// <summary>
    /// Verifies that CreateEntry returns null if there are no changes between entities.
    /// </summary>
    [Fact]
    public void CreateEntry_ReturnsNull_IfNoChanges()
    {
        var oldEntity = new TestEntity { Name = "A", Value = 1 };
        var newEntity = new TestEntity { Name = "A", Value = 1 };
        var result = _auditor.CreateEntry(new[] { "Name", "Value" }, 1, 2, oldEntity, newEntity);
        Assert.Null(result);
    }

    /// <summary>
    /// Verifies that CreateEntry returns only changed values when some properties are changed and others remain the same.
    /// </summary>
    [Fact]
    public void CreateEntry_ReturnsOnlyChangedValues_WhenSomePropertiesAreChangedAndOtherstheSame()
    {
        var oldEntity = new TestEntity { Name = "A", Value = 10 };
        var newEntity = new TestEntity { Name = "A", Value = 20 };
        var result = _auditor.CreateEntry(new[] { "Name", "Value" }, 1, 2, oldEntity, newEntity);
        Assert.NotNull(result);
        Assert.Single(result.OldValues);
        Assert.Single(result.NewValues);
        Assert.True(result.NewValues.ContainsKey("Value"));
        Assert.True(result.OldValues.ContainsKey("Value"));
        Assert.Equal(10, result.OldValues["Value"]);
        Assert.Equal(20, result.NewValues["Value"]);
    }

    /// <summary>
    /// Verifies that CreateEntry returns an AuditEntry if changes are detected.
    /// </summary>
    [Fact]
    public void CreateEntry_ReturnsAuditEntry_IfChanged()
    {
        var oldEntity = new TestEntity { Name = "A", Value = 1 };
        var newEntity = new TestEntity { Name = "B", Value = 2 };
        var result = _auditor.CreateEntry(new[] { "Name", "Value" }, 1, 2, oldEntity, newEntity);
        Assert.NotNull(result);
        Assert.Equal("TestEntity", result.EntityType);
        Assert.Equal(1, result.TestProjectId);
        Assert.Equal(2, result.EntityId);
        Assert.Equal(_timeProvider.GetUtcNow(), result.Created);
        Assert.Equal("A", result.OldValues["Name"]);
        Assert.Equal(1, result.OldValues["Value"]);
        Assert.Equal("B", result.NewValues["Name"]);
        Assert.Equal(2, result.NewValues["Value"]);
    }

    /// <summary>
    /// Verifies that LogAsync calls the repository if changes are detected.
    /// </summary>
    [Fact]
    public async Task LogAsync_CallsRepository_IfChanged()
    {
        var oldEntity = new TestEntity { Name = "A", Value = 1 };
        var newEntity = new TestEntity { Name = "B", Value = 2 };
        await _auditor.LogAsync(new[] { "Name", "Value" }, 1, 2, oldEntity, newEntity);
        await _repository.Received(1).AddEntryAsync(Arg.Any<AuditEntry>());
    }

    /// <summary>
    /// Verifies that LogAsync does not call the repository if no changes are detected.
    /// </summary>
    [Fact]
    public async Task LogAsync_DoesNotCallRepository_IfNoChange()
    {
        var oldEntity = new TestEntity { Name = "A", Value = 1 };
        var newEntity = new TestEntity { Name = "A", Value = 1 };
        await _auditor.LogAsync(new[] { "Name", "Value" }, 1, 2, oldEntity, newEntity);
        await _repository.DidNotReceive().AddEntryAsync(Arg.Any<AuditEntry>());
    }
}
