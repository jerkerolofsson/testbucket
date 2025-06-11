# MCP test extensions for xunit v3

The `TestBucket.McpTest.Xunit` package provides helpers for writing integration tests against Model Context Protocol (MCP) servers using xUnit v3. It is designed to work with the `IMcpClient` interface and the `McpClientFactory` from the `ModelContextProtocol` library.

## Getting Started

1. **Add Package Reference**

   Ensure your test project references the following NuGet packages:
   - `TestBucket.McpTest.Xunit`
   - `ModelContextProtocol.Core`
   - `xunit.v3.assert`
   - `xunit.v3.extensibility.core`

2. **Create an MCP Client**

   Use `McpClientFactory` to create an `IMcpClient` instance. Typically, you will need to provide a transport (e.g., `SseClientTransport`) and authentication headers (if needed).

## Example

```csharp
    [Fact]
    public async Task Should_Invoke_MyTool_Successfully()
    {
        // Arrange: create your IMcpClient (using your factory or fixture)
        IMcpClient client = /* get or create your client, e.g. from a fixture */;

        // Tool name and arguments
        string toolName = "myTool";
        var arguments = new Dictionary<string, object?>
        {
            { "param1", "value1" },
            { "param2", 42 }
        };

        // Act: call the tool
        var response = await client.TestCallToolAsync(
            toolName,
            arguments,
            progress: null, // or provide a progress reporter if needed
            jsonSerializerOptions: new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
        );

        // Assert: check the response
        response.ShouldBeSuccess();
        response.ShouldHaveContent();
    }
```