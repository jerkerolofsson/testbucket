namespace TestBucket.Domain.AI.Agent.Models;
public record class Suggestion(string Title, string Text)
{
    /// <summary>
    /// SVG icon (optional)
    /// </summary>
    public string? Icon { get; set; }
}
