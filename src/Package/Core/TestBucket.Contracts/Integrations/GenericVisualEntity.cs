namespace TestBucket.Contracts.Integrations;
public record class GenericVisualEntity
{
    /// <summary>
    /// Title of item
    /// </summary>
    public string? Title { get; set; }

    /// <summary>
    /// Description
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Color of item
    /// </summary>
    public string? Color { get; set; }

    /// <summary>
    /// SVG
    /// </summary>
    public string? Icon { get; set; }

    /// <summary>
    /// Embedding vector for similarity / semantic search
    /// </summary>
    public ReadOnlyMemory<float>? Embedding { get; set; }
}
