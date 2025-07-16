using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Extensions.AI;

using ModelContextProtocol.Client;

using TestBucket.Domain.AI.Mcp.Models;

namespace TestBucket.Domain.AI.Mcp.Tools;

/// <summary>
/// This is an AI function that is provided by a Model Context Protocol (MCP) server.
/// </summary>
public class McpAIFunction
{
    public bool Enabled { get; set; } = true;
    public required McpServerRegistration McpServerRegistration { get; set; }
    public required McpServer McpServer { get; set; }
    public required AIFunction AIFunction { get; set; }
    public required McpClientTool Tool { get; set; }
}
