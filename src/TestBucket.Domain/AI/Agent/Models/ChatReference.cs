namespace TestBucket.Domain.AI.Agent.Models;

public record class ChatReference
{
    public required long Id { get; set; }
    public required string Name { get; set; }
    public required string Text { get; set; }
    public string? EntityTypeName { get; set; }
    public bool IsActiveDocument { get; set; }

    public bool IsCompiled { get; set; }
    public bool CanBeCompiled { get; set; }
}
