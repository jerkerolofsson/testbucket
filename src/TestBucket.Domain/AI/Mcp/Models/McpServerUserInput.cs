namespace TestBucket.Domain.AI.Mcp.Models;

/// <summary>
/// Represents a user defined input value
/// </summary>
public class McpServerUserInput : ProjectEntity
{
    public long Id { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public required string InputId { get; set; }

    /// <summary>
    /// Username
    /// </summary>
    public required string UserName { get; set; }

    /// <summary>
    /// The value added by the user
    /// </summary>
    public required string Value { get; set; }

    // Navigation

    public long McpServerRegistrationId { get; set; }

    public McpServerRegistration? McpServerRegistration { get; set; }
}
