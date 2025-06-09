namespace TestBucket.Domain.Testing.Heuristics.Models;

[Table("heuristics")]
public class Heuristic : ProjectEntity
{
    /// <summary>
    /// Database ID
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// Name of the heuristic
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// Description of the heuristic
    /// </summary>
    public required string Description { get; set; }
}
