using ModelContextProtocol.Protocol;

using Xunit;

namespace TestBucket.McpTest.Xunit;

/// <summary>
/// Provides assertion helpers for validating <see cref="CallToolResponse"/> results in tests.
/// </summary>
public class TestCallToolResponse
{
    /// <summary>
    /// Gets the <see cref="CallToolResponse"/> instance under test.
    /// </summary>
    public CallToolResponse CallToolResponse { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="TestCallToolResponse"/> class.
    /// </summary>
    /// <param name="callToolResponse">The <see cref="CallToolResponse"/> to wrap and validate.</param>
    internal TestCallToolResponse(CallToolResponse callToolResponse)
    {
        CallToolResponse = callToolResponse;
    }

    /// <summary>
    /// Asserts that the <see cref="CallToolResponse"/> does not indicate an error.
    /// </summary>
    public void ShouldBeSuccess()
    {
        Assert.False(this.CallToolResponse.IsError, "CallToolResponse.IsError is true");
    }

    /// <summary>
    /// Asserts that the <see cref="CallToolResponse"/> contains non-empty content.
    /// </summary>
    public void ShouldHaveContent()
    {
        Assert.NotEmpty(this.CallToolResponse.Content);
    }
}