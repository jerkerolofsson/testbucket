using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Claims;
using TestBucket.Domain.AI.Mcp.Helpers;
using TestBucket.Domain.AI.Mcp.Models;
using Xunit;

namespace TestBucket.Domain.UnitTests.AI.Mcp;

/// <summary>
/// Unit tests for the McpToolAccessChecker class.
/// </summary>
[Feature("MCP")]
[Component("AI")]
[UnitTest]
[FunctionalTest]
[EnrichedTest]
public class McpToolAccessCheckerTests
{
    /// <summary>
    /// Creates a default configuration for MCP server.
    /// </summary>
    /// <returns>A default instance of McpServerConfiguration.</returns>
    private McpServerConfiguration CreateDefaultConfiguration()
    {
        return new McpServerConfiguration
        {
            Inputs = new List<McpInput>(),
            Servers = new Dictionary<string, McpServer>()
        };
    }

    /// <summary>
    /// Verifies that HasAccess returns false when the user's name is null.
    /// </summary>
    [CoveredRequirement("TB-MCP-001")]
    [Fact]
    public void HasAccess_UserNameIsNull_ReturnsFalse()
    {
        // Arrange
        var projectId = 1L;
        var user = new ClaimsPrincipal(new ClaimsIdentity());
        var registration = new McpServerRegistration { TestProjectId = projectId, CreatedBy = "creator", Configuration = CreateDefaultConfiguration() };

        // Act
        var result = McpToolAccessChecker.HasAccess(projectId, user, registration);

        // Assert
        Assert.False(result);
    }

    /// <summary>
    /// Verifies that HasAccess returns false when the project ID does not match.
    /// </summary>
    [Fact]
    [CoveredRequirement("TB-MCP-001")]
    public void HasAccess_ProjectIdMismatch_ReturnsFalse()
    {
        // Arrange
        var projectId = 1L;
        var user = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, "user") }));
        var registration = new McpServerRegistration { TestProjectId = 2L, CreatedBy = "creator", Configuration = CreateDefaultConfiguration() };

        // Act
        var result = McpToolAccessChecker.HasAccess(projectId, user, registration);

        // Assert
        Assert.False(result);
    }

    /// <summary>
    /// Verifies that HasAccess returns true when the user is the creator.
    /// </summary>
    [Fact]
    [CoveredRequirement("TB-MCP-001")]
    public void HasAccess_UserIsCreator_ReturnsTrue()
    {
        // Arrange
        var projectId = 1L;
        var user = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, "creator") }));
        var registration = new McpServerRegistration { TestProjectId = projectId, CreatedBy = "creator", Configuration = CreateDefaultConfiguration() };

        // Act
        var result = McpToolAccessChecker.HasAccess(projectId, user, registration);

        // Assert
        Assert.True(result);
    }

    /// <summary>
    /// Verifies that HasAccess returns true when the registration is public for the project.
    /// </summary>
    [Fact]
    [CoveredRequirement("TB-MCP-001")]
    public void HasAccess_PublicForProject_ReturnsTrue()
    {
        // Arrange
        var projectId = 1L;
        var user = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, "user") }));
        var registration = new McpServerRegistration { TestProjectId = projectId, PublicForProject = true, Configuration = CreateDefaultConfiguration() };

        // Act
        var result = McpToolAccessChecker.HasAccess(projectId, user, registration);

        // Assert
        Assert.True(result);
    }

    /// <summary>
    /// Verifies that HasAccess returns false when the user does not have access.
    /// </summary>
    [Fact]
    [CoveredRequirement("TB-MCP-001")]
    public void HasAccess_NoAccess_ReturnsFalse()
    {
        // Arrange
        var projectId = 1L;
        var user = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, "user") }));
        var registration = new McpServerRegistration { TestProjectId = projectId, CreatedBy = "creator", Configuration = CreateDefaultConfiguration() };

        // Act
        var result = McpToolAccessChecker.HasAccess(projectId, user, registration);

        // Assert
        Assert.False(result);
    }
}
