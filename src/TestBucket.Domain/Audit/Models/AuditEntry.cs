namespace TestBucket.Domain.Audit.Models;

[Index(nameof(EntityType), nameof(EntityId), nameof(Created))]
[Index(nameof(TestProjectId))]
public class AuditEntry
{
    public long Id { get; set; }

    /// <summary>
    /// The ID of the entity
    /// </summary>
    public long EntityId { get; set; }

    /// <summary>
    /// Project ID
    /// </summary>
    public required long TestProjectId { get; set; }

    /// <summary>
    /// The type of entity
    /// </summary>
    public required string EntityType { get; set; }

    /// <summary>
    /// Timestamp when the change was added
    /// </summary>
    public required DateTimeOffset Created { get; set; }

    /// <summary>
    /// The changed values
    /// Key is the entity property
    /// </summary>
    [Column(TypeName = "jsonb")]
    public required Dictionary<string, object?> OldValues { get; set; }

    /// <summary>
    /// The changed values
    /// Key is the entity property
    /// </summary>
    [Column(TypeName = "jsonb")]
    public required Dictionary<string, object?> NewValues { get; set; }
}
