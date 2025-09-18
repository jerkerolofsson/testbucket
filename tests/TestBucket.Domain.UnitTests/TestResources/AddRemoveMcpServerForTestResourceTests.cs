using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

using NSubstitute;

using TestBucket.Domain.AI.Mcp;
using TestBucket.Domain.AI.Mcp.Models;
using TestBucket.Domain.AI.Mcp.Services;
using TestBucket.Domain.TestResources.Events;
using TestBucket.Domain.TestResources.Integrations;
using TestBucket.Domain.TestResources.Models;

using Xunit;

namespace TestBucket.Domain.UnitTests.TestResources;

/// <summary>
/// Unit tests for the <see cref="AddRemoveMcpServerForTestResource"/> class.
/// Verifies the behavior of adding, updating, and removing MCP server registrations
/// based on test resource notifications.
/// </summary>
[EnrichedTest]
[UnitTest]
[FunctionalTest]
[Component("Test Resources")]
public class AddRemoveMcpServerForTestResourceTests
{
    private readonly IMcpServerManager _mcpServerManagerMock;
    private readonly AddRemoveMcpServerForTestResource _handler;

    /// <summary>
    /// Initializes a new instance of the <see cref="AddRemoveMcpServerForTestResourceTests"/> class.
    /// Sets up the mock dependencies and the handler under test.
    /// </summary>
    public AddRemoveMcpServerForTestResourceTests()
    {
        _mcpServerManagerMock = Substitute.For<IMcpServerManager>();
        _handler = new AddRemoveMcpServerForTestResource(_mcpServerManagerMock);
    }

    /// <summary>
    /// Tests that an MCP server is added when a <see cref="TestResourceAdded"/> notification is handled.
    /// </summary>
    [Fact]
    public async Task Handle_TestResourceAdded_ShouldAddMcpServer()
    {
        // Arrange
        var notification = new TestResourceAdded(
            new ClaimsPrincipal(),
            new TestResource
            {
                Types = new[] { "mcp-server" },
                Variables = new Dictionary<string, string> { { "url", "http://example.com" } },
                Name = "TestServer",
                Owner = "Owner1",
                ResourceId = "Resource123"
            }
        );

        // Act
        await _handler.Handle(notification, CancellationToken.None);

        // Assert
        await _mcpServerManagerMock.Received(1).AddMcpServerRegistrationAsync(
            notification.Principal,
            Arg.Is<McpServerRegistration>(r =>
                r.Owner == "Owner1" &&
                r.Configuration!.Servers!.ContainsKey("TestServer") &&
                r.Configuration.Servers["TestServer"].Url == "http://example.com"
            ));
    }

    /// <summary>
    /// Tests that the MCP server URL is updated when a <see cref="TestResourceUpdated"/> notification is handled.
    /// </summary>
    [Fact]
    public async Task Handle_TestResourceUpdated_ShouldUpdateMcpServerUrl()
    {
        // Arrange
        var notification = new TestResourceUpdated(
            new ClaimsPrincipal(),
            new TestResource
            {
                Types = new[] { "mcp-server" },
                Variables = new Dictionary<string, string> { { "url", "http://new-url.com" } },
                Name = "TestServer",
                Owner = "Owner1",
                ResourceId = "Resource123"
            }
        );

        _mcpServerManagerMock.GetAllMcpServerRegistationsAsync(notification.Principal)
            .Returns(new[]
            {
                new McpServerRegistration
                {
                    Owner = "Owner1",
                    Configuration = new McpServerConfiguration
                    {
                        Servers = new Dictionary<string, McpServer>
                        {
                            ["TestServer"] = new McpServer { Url = "http://old-url.com" }
                        }
                    }
                }
            });

        // Act
        await _handler.Handle(notification, CancellationToken.None);

        // Assert
        await _mcpServerManagerMock.Received(1).UpdateMcpServerRegistrationAsync(
            notification.Principal,
            Arg.Is<McpServerRegistration>(r =>
                r.Configuration.Servers!["TestServer"].Url == "http://new-url.com"
            ));
    }

    /// <summary>
    /// Tests that an MCP server is deleted when a <see cref="TestResourceRemoved"/> notification is handled.
    /// </summary>
    [Fact]
    public async Task Handle_TestResourceRemoved_ShouldDeleteMcpServer()
    {
        // Arrange
        var notification = new TestResourceRemoved(
            new ClaimsPrincipal(),
            new TestResource
            {
                Types = new[] { "mcp-server" },
                Name = "TestServer",
                Owner = "Owner1",
                ResourceId = "Resource123"
            }
        );

        _mcpServerManagerMock.GetAllMcpServerRegistationsAsync(notification.Principal)
            .Returns(new[]
            {
                new McpServerRegistration
                {
                    Owner = "Owner1",
                    Configuration = new McpServerConfiguration
                    {
                        Servers = new Dictionary<string, McpServer>
                        {
                            ["TestServer"] = new McpServer()
                        }
                    }
                }
            });

        // Act
        await _handler.Handle(notification, CancellationToken.None);

        // Assert
        await _mcpServerManagerMock.Received(1).DeleteMcpServerRegistrationAsync(
            notification.Principal,
            Arg.Any<McpServerRegistration>());
    }
}