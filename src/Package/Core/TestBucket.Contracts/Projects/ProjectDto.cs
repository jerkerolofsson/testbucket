using TestBucket.Contracts.Integrations;
using TestBucket.Contracts.Testing.States;

namespace TestBucket.Contracts.Projects;
public record class ProjectDto
{
    public long Id { get; set; }

    /// <summary>
    /// Name of the project
    /// </summary>
    public required string Name { get; set;  }

    /// <summary>
    /// Project slug (when creating set to an empty string)
    /// </summary>
    public required string Slug { get; set; }

    /// <summary>
    /// External systems / integrations
    /// </summary>
    public required ExternalSystemDto[] ExternalSystems { get; set; }

    /// <summary>
    /// Short name (when creating set to an empty string)
    /// </summary>
    public required string ShortName { get; set; }

    /// <summary>
    /// States for test case runs
    /// </summary>
    public TestState[]? TestStates { get; set; }

    /// <summary>
    /// Slug for team
    /// </summary>
    public string? Team { get; set; }
}
