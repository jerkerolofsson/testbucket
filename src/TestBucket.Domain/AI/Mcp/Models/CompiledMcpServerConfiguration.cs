using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestBucket.Domain.AI.Mcp.Models;
public class CompiledMcpServerConfiguration
{
    public McpServerConfiguration CompiledConfiguration { get; set; } = new();

    public List<McpInput> MissingInputs { get; set; } = [];

    /// <summary>
    /// User variables. The id of the input is the key, and the value is the user input value.
    /// </summary>
    public Dictionary<string, string> Variables { get; set; } = [];
}
