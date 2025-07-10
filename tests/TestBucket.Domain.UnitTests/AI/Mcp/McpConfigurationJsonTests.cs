using NSubstitute;
using System.Security.Claims;
using TestBucket.Domain.AI.Mcp.Models;
using TestBucket.Domain.AI.Mcp.Services;

namespace TestBucket.Domain.UnitTests.AI.Mcp
{
    /// <summary>
    /// Verification of json serialization tests for mcp configuration files.
    /// </summary>
    [Feature("MCP")]
    [Component("AI")]
    [UnitTest]
    [FunctionalTest]
    public class McpConfigurationJsonTests
    {

        #region JSON Serialization Tests

        /// <summary>
        /// Verifies that <see cref="McpServerConfiguration.FromJson"/> correctly deserializes JSON with "servers" key.
        /// </summary>
        [Fact]
        public void FromJson_WithServersKey_DeserializesCorrectly()
        {
            // Arrange
            var json = """
                {
                  "inputs": [
                    {
                      "id": "api_key",
                      "description": "API Key",
                      "type": "string"
                    }
                  ],
                  "servers": {
                    "testbucket": {
                      "type": "http",
                      "url": "https://api.example.com/mcp"
                    }
                  }
                }
                """;

            // Act
            var config = McpServerConfiguration.FromJson(json);

            // Assert
            Assert.NotNull(config);
            Assert.NotNull(config.Servers);
            Assert.Single(config.Servers);
            Assert.True(config.Servers.ContainsKey("testbucket"));
            Assert.Equal("http", config.Servers["testbucket"].Type);
            Assert.Equal("https://api.example.com/mcp", config.Servers["testbucket"].Url);
            Assert.NotNull(config.Inputs);
            Assert.Single(config.Inputs);
            Assert.Equal("api_key", config.Inputs[0].Id);
        }

        /// <summary>
        /// Verifies that <see cref="McpServerConfiguration.FromJson"/> correctly deserializes JSON with "mcpServers" key.
        /// </summary>
        [Fact]
        public void FromJson_WithMcpServersKey_DeserializesCorrectly()
        {
            // Arrange
            var json = """
                {
                  "inputs": [
                    {
                      "id": "auth_token",
                      "description": "Authentication Token",
                      "type": "string",
                      "password": true
                    }
                  ],
                  "mcpServers": {
                    "test-bucket": {
                      "command": "npx",
                      "args": ["mcp-remote", "https://api.example.com/mcp"],
                      "env": {
                        "AUTH_HEADER": "Bearer ${auth_token}"
                      }
                    }
                  }
                }
                """;

            // Act
            var config = McpServerConfiguration.FromJson(json);

            // Assert
            Assert.NotNull(config);
            Assert.NotNull(config.Servers);
            Assert.Single(config.Servers);
            Assert.True(config.Servers.ContainsKey("test-bucket"));
            Assert.Equal("npx", config.Servers["test-bucket"].Command);
            Assert.NotNull(config.Servers["test-bucket"].Env);
            Assert.NotNull(config.Servers["test-bucket"].Args);
            Assert.Equal(2, config.Servers["test-bucket"].Args!.Length);
            Assert.Equal("mcp-remote", config.Servers!["test-bucket"].Args![0]);
            Assert.NotNull(config.Servers["test-bucket"].Env);
            Assert.Equal("Bearer ${auth_token}", config.Servers!["test-bucket"].Env!["AUTH_HEADER"]);
            Assert.NotNull(config.Inputs);
            Assert.Single(config.Inputs);
            Assert.Equal("auth_token", config.Inputs[0].Id);
            Assert.True(config.Inputs[0].Password);
        }

        /// <summary>
        /// Verifies that <see cref="McpServerConfiguration.FromJson"/> prioritizes "servers" over "mcpServers" when both are present.
        /// </summary>
        [Fact]
        public void FromJson_WithBothServersAndMcpServers_PrioritizesServers()
        {
            // Arrange
            var json = """
                {
                  "servers": {
                    "priority-server": {
                      "type": "http",
                      "url": "https://priority.example.com"
                    }
                  },
                  "mcpServers": {
                    "fallback-server": {
                      "command": "fallback-command"
                    }
                  }
                }
                """;

            // Act
            var config = McpServerConfiguration.FromJson(json);

            // Assert
            Assert.NotNull(config);
            Assert.NotNull(config.Servers);
            Assert.Single(config.Servers);
            Assert.True(config.Servers.ContainsKey("priority-server"));
            Assert.False(config.Servers.ContainsKey("fallback-server"));
            Assert.Equal("https://priority.example.com", config.Servers["priority-server"].Url);
        }

        /// <summary>
        /// Verifies that <see cref="McpServerConfiguration.FromJson"/> returns null servers when neither key is present.
        /// </summary>
        [Fact]
        public void FromJson_WithoutServersOrMcpServers_ReturnsNullServers()
        {
            // Arrange
            var json = """
                {
                  "inputs": [
                    {
                      "id": "test_input",
                      "description": "Test Input"
                    }
                  ]
                }
                """;

            // Act
            var config = McpServerConfiguration.FromJson(json);

            // Assert
            Assert.NotNull(config);
            Assert.Null(config.Servers);
            Assert.NotNull(config.Inputs);
            Assert.Single(config.Inputs);
        }

        #endregion
    }
}