using ModelContextProtocol.Protocol;

using Xunit;

namespace TestBucket.McpTest.Xunit;

public static class CallToolResponseExtensions
{
    public static void AssertIsSuccess(this CallToolResponse toolResponse)
    {
        Assert.False(toolResponse.IsError, "CallToolResponse.IsError is true");
    }
}
