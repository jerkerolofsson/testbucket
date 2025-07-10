using TestBucket.Domain.AI.Mcp.Models;

namespace TestBucket.Domain.AI.Mcp.Services;

/// <summary>
/// Replaces ${input} with the user input value in the MCP configuration.
/// </summary>
public class McpConfigurationCompiler
{
    private readonly IMcpServerUserInputProvider _userInputProvider;

    /// <summary>
    /// Initializes a new instance of the <see cref="McpConfigurationCompiler"/> class.
    /// </summary>
    /// <param name="userInputProvider">The provider for retrieving user input values.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="userInputProvider"/> is null.</exception>

    public McpConfigurationCompiler(IMcpServerUserInputProvider userInputProvider)
    {
        _userInputProvider = userInputProvider ?? throw new ArgumentNullException(nameof(userInputProvider));
    }

    /// <summary>
    /// Compiles an MCP server configuration by replacing variable placeholders with actual user input values.
    /// </summary>
    /// <param name="user">The claims principal representing the current user. This is used to filter user inputs.</param>
    /// <param name="projectId">The current project. This is used to filter user inputs.</param>
    /// <param name="mcpServerRegistrationId">The ID of the MCP server registration.</param>
    /// <param name="source">The source MCP server configuration to compile.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="user.Identity.Name"/> is null.</exception>
    public async Task<CompiledMcpServerConfiguration> CompileAsync(ClaimsPrincipal user, long projectId, long mcpServerRegistrationId, McpServerConfiguration source)
    {
        var result = new CompiledMcpServerConfiguration();
        var userName = user. Identity?.Name ?? throw new ArgumentNullException(nameof(user.Identity.Name));

        if (source.Inputs != null)
        {
            foreach (var input in source.Inputs)
            {
                McpServerUserInput? userInput = await _userInputProvider.GetUserInputAsync(projectId, userName, mcpServerRegistrationId, input.Id);
                if (userInput == null)
                {
                    result.MissingInputs.Add(input);
                }

                result.Variables[input.Id] = userInput?.Value ?? string.Empty;
            }
        }

        // Create a copy of the configuration and replace placeholders with variables
        if (source.Servers is not null)
        {
            foreach (var server in source.Servers)
            {
                var compiledServer = server.Value;
                if (compiledServer.Command != null)
                {
                    compiledServer.Command = ReplacePlaceholders(compiledServer.Command, result.Variables);
                }
                if (compiledServer.Url != null)
                {
                    compiledServer.Url = ReplacePlaceholders(compiledServer.Url, result.Variables);
                }
                // If headers are present, replace placeholders in headers
                if (compiledServer.Headers != null)
                {
                    foreach (var header in compiledServer.Headers)
                    {
                        compiledServer.Headers[header.Key] = ReplacePlaceholders(header.Value, result.Variables);
                    }
                }
            }
        }

        return result;
    }

    /// <summary>
    /// Replaces variable placeholders in the format ${variableName} with their corresponding values.
    /// </summary>
    /// <param name="value">The string containing placeholders to replace.</param>
    /// <param name="variables">A dictionary containing variable names as keys and their replacement values.</param>
    /// <returns>The string with all placeholders replaced by their corresponding variable values.</returns>
    private string ReplacePlaceholders(string value, Dictionary<string, string> variables)
    {
        foreach(var variable in variables)
        {
            // Replace ${variable} with the actual value
            value = value.Replace($"${{{variable.Key}}}", variable.Value);
        }
        return value;
    }
}
