using NSubstitute;
using System.Security.Claims;
using TestBucket.Domain.AI.Mcp.Models;
using TestBucket.Domain.AI.Mcp.Services;

namespace TestBucket.Domain.UnitTests.AI.Mcp
{
    /// <summary>
    /// Contains unit tests for the <see cref="McpConfigurationCompiler"/> class, verifying correct compilation and variable replacement in MCP server configurations.
    /// </summary>
    [Feature("MCP")]
    [Component("AI")]
    [UnitTest]
    [FunctionalTest]
    [EnrichedTest]
    public class McpConfigurationCompilerTests
    {
        private readonly IMcpServerUserInputProvider _mockUserInputProvider;
        private readonly McpConfigurationCompiler _compiler;
        private readonly ClaimsPrincipal _testUser;
        private const long TestProjectId = 123L;
        private const long TestMcpServerRegistrationId = 456L;
        private const string TestUserName = "testuser@example.com";

        /// <summary>
        /// Initializes a new instance of the <see cref="McpConfigurationCompilerTests"/> class with mock dependencies and test data.
        /// </summary>
        public McpConfigurationCompilerTests()
        {
            _mockUserInputProvider = Substitute.For<IMcpServerUserInputProvider>();
            _compiler = new McpConfigurationCompiler(_mockUserInputProvider);
            _testUser = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, TestUserName)
            }));
        }

        #region Constructor Tests

        /// <summary>
        /// Verifies that the constructor throws an <see cref="ArgumentNullException"/> when a null user input provider is provided.
        /// </summary>
        [Fact]
        public void Constructor_WithNullUserInputProvider_ThrowsArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new McpConfigurationCompiler(null!));
        }

        #endregion

        #region Compilation Tests

        /// <summary>
        /// Verifies that <see cref="McpConfigurationCompiler.CompileAsync"/> throws an <see cref="ArgumentNullException"/> when the user identity name is null.
        /// </summary>
        [Fact]
        public async Task CompileAsync_WithNullUserIdentityName_ThrowsArgumentNullException()
        {
            // Arrange
            var userWithoutName = new ClaimsPrincipal(new ClaimsIdentity());
            var config = new McpServerConfiguration();

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                _compiler.CompileAsync(userWithoutName, TestProjectId, TestMcpServerRegistrationId, config));
        }

        /// <summary>
        /// Verifies that <see cref="McpConfigurationCompiler.CompileAsync"/> returns an empty configuration when inputs and servers are null.
        /// </summary>
        [Fact]
        public async Task CompileAsync_WithNullInputs_ReturnsEmptyConfiguration()
        {
            // Arrange
            var config = new McpServerConfiguration
            {
                Inputs = null,
                Servers = null
            };

            // Act
            var result = await _compiler.CompileAsync(_testUser, TestProjectId, TestMcpServerRegistrationId, config);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result.Variables);
            Assert.Empty(result.MissingInputs);
        }

        /// <summary>
        /// Verifies that <see cref="McpConfigurationCompiler.CompileAsync"/> returns an empty configuration when inputs and servers collections are empty.
        /// </summary>
        [Fact]
        public async Task CompileAsync_WithEmptyInputs_ReturnsEmptyConfiguration()
        {
            // Arrange
            var config = new McpServerConfiguration
            {
                Inputs = new List<McpInput>(),
                Servers = new Dictionary<string, McpServer>()
            };

            // Act
            var result = await _compiler.CompileAsync(_testUser, TestProjectId, TestMcpServerRegistrationId, config);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result.Variables);
            Assert.Empty(result.MissingInputs);
        }

        /// <summary>
        /// Verifies that <see cref="McpConfigurationCompiler.CompileAsync"/> correctly populates variables when user input is available.
        /// </summary>
        [Fact]
        public async Task CompileAsync_WithAvailableUserInput_PopulatesVariables()
        {
            // Arrange
            var input = new McpInput { Id = "api_key", Description = "API Key", Type = "string" };
            var userInput = new McpServerUserInput
            {
                InputId = "api_key",
                UserName = TestUserName,
                Value = "secret-key-123"
            };

            var config = new McpServerConfiguration
            {
                Inputs = new List<McpInput> { input }
            };

            _mockUserInputProvider
                .GetUserInputAsync(TestProjectId, TestUserName, TestMcpServerRegistrationId, input.Id)
                .Returns(userInput);

            // Act
            var result = await _compiler.CompileAsync(_testUser, TestProjectId, TestMcpServerRegistrationId, config);

            // Assert
            Assert.Single(result.Variables);
            Assert.Equal("secret-key-123", result.Variables["api_key"]);
            Assert.Empty(result.MissingInputs);
        }

        /// <summary>
        /// Verifies that <see cref="McpConfigurationCompiler.CompileAsync"/> adds missing inputs and creates empty variables when user input is not available.
        /// </summary>
        [Fact]
        public async Task CompileAsync_WithMissingUserInput_AddsMissingInputsAndEmptyVariable()
        {
            // Arrange
            var input = new McpInput { Id = "api_key", Description = "API Key", Type = "string" };
            var config = new McpServerConfiguration
            {
                Inputs = new List<McpInput> { input }
            };

            _mockUserInputProvider
                .GetUserInputAsync(TestProjectId, TestUserName, TestMcpServerRegistrationId, input.Id)
                .Returns((McpServerUserInput?)null);

            // Act
            var result = await _compiler.CompileAsync(_testUser, TestProjectId, TestMcpServerRegistrationId, config);

            // Assert
            Assert.Single(result.Variables);
            Assert.Equal(string.Empty, result.Variables["api_key"]);
            Assert.Single(result.MissingInputs);
            Assert.Equal(input, result.MissingInputs[0]);
        }

        /// <summary>
        /// Verifies that <see cref="McpConfigurationCompiler.CompileAsync"/> correctly handles multiple inputs with mixed availability.
        /// </summary>
        [Fact]
        public async Task CompileAsync_WithMultipleInputs_HandlesMixedAvailabilityCorrectly()
        {
            // Arrange
            var input1 = new McpInput { Id = "api_key", Description = "API Key", Type = "string" };
            var input2 = new McpInput { Id = "username", Description = "Username", Type = "string" };
            var input3 = new McpInput { Id = "password", Description = "Password", Type = "password", Password = true };

            var userInput1 = new McpServerUserInput { UserName = TestUserName, Value = "secret-key-123", InputId = "api_key" };
            var userInput3 = new McpServerUserInput { UserName = TestUserName, Value = "secret-password", InputId = "password" };

            var config = new McpServerConfiguration
            {
                Inputs = new List<McpInput> { input1, input2, input3 }
            };

            _mockUserInputProvider
                .GetUserInputAsync(TestProjectId, TestUserName, TestMcpServerRegistrationId, input1.Id)
                .Returns(userInput1);
            _mockUserInputProvider
                .GetUserInputAsync(TestProjectId, TestUserName, TestMcpServerRegistrationId, input2.Id)
                .Returns((McpServerUserInput?)null);
            _mockUserInputProvider
                .GetUserInputAsync(TestProjectId, TestUserName, TestMcpServerRegistrationId, input3.Id)
                .Returns(userInput3);

            // Act
            var result = await _compiler.CompileAsync(_testUser, TestProjectId, TestMcpServerRegistrationId, config);

            // Assert
            Assert.Equal(3, result.Variables.Count);
            Assert.Equal("secret-key-123", result.Variables["api_key"]);
            Assert.Equal(string.Empty, result.Variables["username"]);
            Assert.Equal("secret-password", result.Variables["password"]);
            Assert.Single(result.MissingInputs);
            Assert.Equal(input2, result.MissingInputs[0]);
        }

        /// <summary>
        /// Verifies that <see cref="McpConfigurationCompiler.CompileAsync"/> correctly replaces variable placeholders in server command strings.
        /// </summary>
        [Fact]
        public async Task CompileAsync_WithServerCommand_ReplacesPlaceholders()
        {
            // Arrange
            var input = new McpInput { Id = "api_key", Description = "API Key", Type = "string" };
            var userInput = new McpServerUserInput { UserName = TestUserName, Value = "secret-123", InputId = "api_key" };

            var server = new McpServer
            {
                Command = "curl -H 'Authorization: Bearer ${api_key}' https://api.example.com",
                Type = "http"
            };

            var config = new McpServerConfiguration
            {
                Inputs = new List<McpInput> { input },
                Servers = new Dictionary<string, McpServer> { { "test-server", server } }
            };

            _mockUserInputProvider
                .GetUserInputAsync(TestProjectId, TestUserName, TestMcpServerRegistrationId, input.Id)
                .Returns(userInput);

            // Act
            var result = await _compiler.CompileAsync(_testUser, TestProjectId, TestMcpServerRegistrationId, config);

            // Assert
            Assert.Equal("curl -H 'Authorization: Bearer secret-123' https://api.example.com",
                config.Servers!["test-server"].Command);
        }

        /// <summary>
        /// Verifies that <see cref="McpConfigurationCompiler.CompileAsync"/> correctly replaces variable placeholders in server URL strings.
        /// </summary>
        [Fact]
        public async Task CompileAsync_WithServerUrl_ReplacesPlaceholders()
        {
            // Arrange
            var input = new McpInput { Id = "base_url", Description = "Base URL", Type = "string" };
            var userInput = new McpServerUserInput { UserName = TestUserName, Value = "https://api.example.com", InputId = "base_url" };

            var server = new McpServer
            {
                Url = "${base_url}/mcp/endpoint",
                Type = "http"
            };

            var config = new McpServerConfiguration
            {
                Inputs = new List<McpInput> { input },
                Servers = new Dictionary<string, McpServer> { { "test-server", server } }
            };

            _mockUserInputProvider
                .GetUserInputAsync(TestProjectId, TestUserName, TestMcpServerRegistrationId, input.Id)
                .Returns(userInput);

            // Act
            var result = await _compiler.CompileAsync(_testUser, TestProjectId, TestMcpServerRegistrationId, config);

            // Assert
            Assert.Equal("https://api.example.com/mcp/endpoint",
                config.Servers!["test-server"].Url);
        }

        /// <summary>
        /// Verifies that <see cref="McpConfigurationCompiler.CompileAsync"/> correctly replaces variable placeholders in server headers.
        /// </summary>
        [Fact]
        public async Task CompileAsync_WithServerHeaders_ReplacesPlaceholders()
        {
            // Arrange
            var input1 = new McpInput { Id = "api_key", Description = "API Key", Type = "string" };
            var input2 = new McpInput { Id = "user_agent", Description = "User Agent", Type = "string" };

            var userInput1 = new McpServerUserInput { UserName = TestUserName, Value = "secret-123", InputId = "api_key" };
            var userInput2 = new McpServerUserInput { UserName = TestUserName, Value = "TestBucket/1.0", InputId = "user_agent" };

            var server = new McpServer
            {
                Type = "http",
                Headers = new McpHttpHeaders
                {
                    { "Authorization", "Bearer ${api_key}" },
                    { "User-Agent", "${user_agent}" },
                    { "Content-Type", "application/json" }
                }
            };

            var config = new McpServerConfiguration
            {
                Inputs = new List<McpInput> { input1, input2 },
                Servers = new Dictionary<string, McpServer> { { "test-server", server } }
            };

            _mockUserInputProvider
                .GetUserInputAsync(TestProjectId, TestUserName, TestMcpServerRegistrationId, input1.Id)
                .Returns(userInput1);
            _mockUserInputProvider
                .GetUserInputAsync(TestProjectId, TestUserName, TestMcpServerRegistrationId, input2.Id)
                .Returns(userInput2);

            // Act
            var result = await _compiler.CompileAsync(_testUser, TestProjectId, TestMcpServerRegistrationId, config);

            // Assert
            var headers = config.Servers!["test-server"].Headers!;
            Assert.Equal("Bearer secret-123", headers["Authorization"]);
            Assert.Equal("TestBucket/1.0", headers["User-Agent"]);
            Assert.Equal("application/json", headers["Content-Type"]);
        }

        /// <summary>
        /// Verifies that <see cref="McpConfigurationCompiler.CompileAsync"/> correctly replaces variable placeholders across multiple servers.
        /// </summary>
        [Fact]
        public async Task CompileAsync_WithMultipleServers_ReplacesPlaceholdersInAll()
        {
            // Arrange
            var input = new McpInput { Id = "token", Description = "Access Token", Type = "string" };
            var userInput = new McpServerUserInput { UserName = TestUserName, Value = "abc123", InputId = "token" };

            var server1 = new McpServer
            {
                Command = "server1 --token ${token}",
                Type = "stdio"
            };

            var server2 = new McpServer
            {
                Url = "https://api.example.com/${token}",
                Type = "http"
            };

            var config = new McpServerConfiguration
            {
                Inputs = new List<McpInput> { input },
                Servers = new Dictionary<string, McpServer>
                {
                    { "server1", server1 },
                    { "server2", server2 }
                }
            };

            _mockUserInputProvider
                .GetUserInputAsync(TestProjectId, TestUserName, TestMcpServerRegistrationId, input.Id)
                .Returns(userInput);

            // Act
            var result = await _compiler.CompileAsync(_testUser, TestProjectId, TestMcpServerRegistrationId, config);

            // Assert
            Assert.Equal("server1 --token abc123", config.Servers!["server1"].Command);
            Assert.Equal("https://api.example.com/abc123", config.Servers!["server2"].Url);
        }

        /// <summary>
        /// Verifies that <see cref="McpConfigurationCompiler.CompileAsync"/> leaves empty replacements when variables are missing.
        /// </summary>
        [Fact]
        public async Task CompileAsync_WithMissingVariable_LeavesEmptyReplacement()
        {
            // Arrange
            var input = new McpInput { Id = "api_key", Description = "API Key", Type = "string" };

            var server = new McpServer
            {
                Command = "curl -H 'Authorization: Bearer ${api_key}' https://api.example.com",
                Type = "http"
            };

            var config = new McpServerConfiguration
            {
                Inputs = new List<McpInput> { input },
                Servers = new Dictionary<string, McpServer> { { "test-server", server } }
            };

            _mockUserInputProvider
                .GetUserInputAsync(TestProjectId, TestUserName, TestMcpServerRegistrationId, input.Id)
                .Returns((McpServerUserInput?)null);

            // Act
            var result = await _compiler.CompileAsync(_testUser, TestProjectId, TestMcpServerRegistrationId, config);

            // Assert
            Assert.Equal("curl -H 'Authorization: Bearer ' https://api.example.com",
                config.Servers!["test-server"].Command);
            Assert.Single(result.MissingInputs);
        }

        /// <summary>
        /// Verifies that <see cref="McpConfigurationCompiler.CompileAsync"/> correctly handles complex placeholder patterns with multiple variable replacements in a single string.
        /// </summary>
        [Fact]
        public async Task CompileAsync_WithComplexPlaceholderPattern_ReplacesCorrectly()
        {
            // Arrange
            var input1 = new McpInput { Id = "host", Description = "Host", Type = "string" };
            var input2 = new McpInput { Id = "port", Description = "Port", Type = "string" };
            var input3 = new McpInput { Id = "path", Description = "Path", Type = "string" };

            var userInput1 = new McpServerUserInput { UserName = TestUserName, Value = "localhost", InputId = "host" };
            var userInput2 = new McpServerUserInput { UserName = TestUserName, Value = "8080", InputId = "port" };
            var userInput3 = new McpServerUserInput { UserName = TestUserName, Value = "/api/v1", InputId = "path" };

            var server = new McpServer
            {
                Url = "https://${host}:${port}${path}/endpoint?param=${host}",
                Type = "http"
            };

            var config = new McpServerConfiguration
            {
                Inputs = new List<McpInput> { input1, input2, input3 },
                Servers = new Dictionary<string, McpServer> { { "test-server", server } }
            };

            _mockUserInputProvider
                .GetUserInputAsync(TestProjectId, TestUserName, TestMcpServerRegistrationId, input1.Id)
                .Returns(userInput1);
            _mockUserInputProvider
                .GetUserInputAsync(TestProjectId, TestUserName, TestMcpServerRegistrationId, input2.Id)
                .Returns(userInput2);
            _mockUserInputProvider
                .GetUserInputAsync(TestProjectId, TestUserName, TestMcpServerRegistrationId, input3.Id)
                .Returns(userInput3);

            // Act
            var result = await _compiler.CompileAsync(_testUser, TestProjectId, TestMcpServerRegistrationId, config);

            // Assert
            Assert.Equal("https://localhost:8080/api/v1/endpoint?param=localhost",
                config.Servers!["test-server"].Url);
        }

        /// <summary>
        /// Verifies that <see cref="McpConfigurationCompiler.CompileAsync"/> does not throw exceptions when server properties are null.
        /// </summary>
        [Fact]
        public async Task CompileAsync_WithNullServerProperties_DoesNotThrow()
        {
            // Arrange
            var server = new McpServer
            {
                Command = null,
                Url = null,
                Headers = null,
                Type = "http"
            };

            var config = new McpServerConfiguration
            {
                Inputs = new List<McpInput>(),
                Servers = new Dictionary<string, McpServer> { { "test-server", server } }
            };

            // Act & Assert
            var result = await _compiler.CompileAsync(_testUser, TestProjectId, TestMcpServerRegistrationId, config);

            // Should not throw and should complete successfully
            Assert.NotNull(result);
        }

        #endregion
    }
}