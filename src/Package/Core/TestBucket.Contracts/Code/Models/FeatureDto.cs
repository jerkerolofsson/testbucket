

namespace TestBucket.Contracts.Code.Models;
public class AritecturalComponentDto
{
    public long Id { get; set; }
    public required string Name { get; set; }
    public required string Description { get; set; }

    public string? DevelopmentLead { get; set; }
    public string? TestLead { get; set; }
    public List<string>? GlobPatterns { get; set; }
}
