using Microsoft.Extensions.AI;

using TestBucket.Domain.AI.Mcp.Models;
using TestBucket.Domain.AI.Mcp.Tools;

namespace TestBucket.Domain.AI.Mcp;

/// <summary>
/// Manages Model Context Protocol (MCP) server registrations and user inputs for projects.
/// Provides functionality to register, configure, and manage MCP servers that can be used
/// for AI agent interactions within the TestBucket platform.
/// </summary>
public interface IMcpServerManager
{
    /// <summary>
    /// Gets all MCP server registrations that are accessible to the current user for a specific project.
    /// This includes both user-specific registrations and public registrations for the project.
    /// </summary>
    /// <param name="principal">The authenticated user's security principal</param>
    /// <param name="projectId">The ID of the project to get registrations for</param>
    /// <returns>A read-only list of MCP server registrations accessible to the user</returns>
    Task<IReadOnlyList<McpServerRegistration>> GetUserMcpServerRegistationsAsync(ClaimsPrincipal principal, long projectId);

    /// <summary>
    /// Gets all MCP server registrations for a specific project, regardless of visibility settings.
    /// This method typically requires elevated permissions and returns all registrations in the project.
    /// </summary>
    /// <param name="principal">The authenticated user's security principal</param>
    
    /// <returns>A read-only list of all MCP server registrations in the project</returns>
    Task<IReadOnlyList<McpServerRegistration>> GetAllMcpServerRegistationsAsync(ClaimsPrincipal principal);

    /// <summary>
    /// Gets all MCP server registrations for a specific project, regardless of visibility settings.
    /// This method typically requires elevated permissions and returns all registrations in the project.
    /// </summary>
    /// <param name="principal">The authenticated user's security principal</param>
    /// <param name="projectId">The ID of the project to get all registrations for</param>
    /// <returns>A read-only list of all MCP server registrations in the project</returns>
    Task<IReadOnlyList<McpServerRegistration>> GetAllMcpServerRegistationsAsync(ClaimsPrincipal principal, long projectId);

    /// <summary>
    /// Adds a new MCP server registration to the system.
    /// The registration will be associated with the user's tenant and project.
    /// </summary>
    /// <param name="principal">The authenticated user's security principal</param>
    /// <param name="registration">The MCP server registration to add</param>
    /// <returns>A task representing the asynchronous operation</returns>
    Task AddMcpServerRegistrationAsync(ClaimsPrincipal principal, McpServerRegistration registration);

    /// <summary>
    /// Updates an existing MCP server registration.
    /// The user must have write permissions and the registration must belong to their tenant.
    /// </summary>
    /// <param name="principal">The authenticated user's security principal</param>
    /// <param name="registration">The MCP server registration with updated values</param>
    /// <returns>A task representing the asynchronous operation</returns>
    Task UpdateMcpServerRegistrationAsync(ClaimsPrincipal principal, McpServerRegistration registration);

    /// <summary>
    /// Deletes an MCP server registration from the system.
    /// The user must have delete permissions and the registration must belong to their tenant.
    /// </summary>
    /// <param name="principal">The authenticated user's security principal</param>
    /// <param name="registration">The MCP server registration to delete</param>
    /// <returns>A task representing the asynchronous operation</returns>
    Task DeleteMcpServerRegistrationAsync(ClaimsPrincipal principal, McpServerRegistration registration);

    /// <summary>
    /// Compiles and returns the complete MCP server configuration with resolved user inputs.
    /// This method processes the server configuration template and fills in user-provided values
    /// for any required inputs, returning both the compiled configuration and any missing inputs.
    /// </summary>
    /// <param name="principal">The authenticated user's security principal</param>
    /// <param name="projectId">The ID of the project</param>
    /// <param name="server">The MCP server registration to compile configuration for</param>
    /// <returns>A compiled configuration containing the resolved settings, variables, and any missing inputs</returns>
    Task<CompiledMcpServerConfiguration> GetMcpServerConfigurationAsync(ClaimsPrincipal principal, long projectId, McpServerRegistration server);

    /// <summary>
    /// Deletes all user inputs for a specific user and MCP server registration.
    /// This clears all user-provided values for configuration inputs.
    /// </summary>
    /// <param name="principal">The authenticated user's security principal</param>
    /// <param name="projectId">The ID of the project</param>
    /// <param name="mcpServerRegistrationId">The ID of the MCP server registration</param>
    /// <returns>A task representing the asynchronous operation</returns>
    Task ClearUserInputsAsync(ClaimsPrincipal principal, long projectId, long mcpServerRegistrationId);

    /// <summary>
    /// Adds a new user input value for an MCP server configuration parameter.
    /// User inputs are used to provide values for configurable parameters in MCP server configurations.
    /// </summary>
    /// <param name="principal">The authenticated user's security principal</param>
    /// <param name="userInput">The user input to add</param>
    /// <returns>A task representing the asynchronous operation</returns>
    Task AddUserInputAsync(ClaimsPrincipal principal, McpServerUserInput userInput);

    /// <summary>
    /// Updates an existing user input value for an MCP server configuration parameter.
    /// The user input must belong to the authenticated user's tenant.
    /// </summary>
    /// <param name="principal">The authenticated user's security principal</param>
    /// <param name="userInput">The user input with updated values</param>
    /// <returns>A task representing the asynchronous operation</returns>
    Task UpdateUserInputAsync(ClaimsPrincipal principal, McpServerUserInput userInput);

    /// <summary>
    /// Deletes a user input value for an MCP server configuration parameter.
    /// The user must have delete permissions and the input must belong to their tenant.
    /// </summary>
    /// <param name="principal">The authenticated user's security principal</param>
    /// <param name="userInput">The user input to delete</param>
    /// <returns>A task representing the asynchronous operation</returns>
    Task DeleteUserInputAsync(ClaimsPrincipal principal, McpServerUserInput userInput);
}
