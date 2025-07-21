namespace TestBucket.Domain.AI.Agent.Models;
public record class Suggestion(string Title, string Text)
{
    /// <summary>
    /// SVG icon (optional)
    /// </summary>
    public string? Icon { get; set; }

    /// <summary>
    /// Id of agent to use
    /// </summary>
    public string? AgentId { get; set; }
}
