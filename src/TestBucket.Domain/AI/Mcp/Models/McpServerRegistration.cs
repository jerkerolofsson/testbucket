namespace TestBucket.Domain.AI.Mcp.Models;
public class McpServerRegistration : ProjectEntity
{
    /// <summary>
    /// Database ID
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// Visible to all project members
    /// </summary>
    public bool PublicForProject { get; set; }

    /// <summary>
    /// Enabled flag
    /// </summary>
    public bool Enabled { get; set; }

    /// <summary>
    /// Locked / in use
    /// </summary>
    public bool Locked { get; set; }

    /// <summary>
    /// MCP server configuration
    /// </summary>
    [Column(TypeName = "jsonb")]
    public required McpServerConfiguration Configuration { get; set; }
}
