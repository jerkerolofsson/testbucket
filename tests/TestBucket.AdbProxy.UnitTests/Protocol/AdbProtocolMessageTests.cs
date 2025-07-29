using System.Text;

using TestBucket.AdbProxy.Protocol;
using TestBucket.Traits.Xunit;

namespace TestBucket.AdbProxy.UnitTests.Protocol;

/// <summary>
/// Unit tests for the <see cref="AdbProtocolMessage"/> class.
/// </summary>
[EnrichedTest]
[UnitTest]
[Component("ADB Protocol")]
[Feature("Test Resources")]
public class AdbProtocolMessageTests
{
    /// <summary>
    /// Tests the <see cref="AdbProtocolMessage.Create(uint, uint, uint, string)"/> method with valid parameters.
    /// Verifies that the returned message has the correct properties.
    /// </summary>
    [Fact]
    public void Create_WithValidParameters_ShouldReturnMessageWithCorrectProperties()
    {
        // Arrange
        uint command = 0x12345678;
        uint arg0 = 1;
        uint arg1 = 2;
        string text = "TestPayload";

        // Act
        var message = AdbProtocolMessage.Create(command, arg0, arg1, text);

        // Assert
        Assert.Equal(command, message.Header.Command);
        Assert.Equal(arg0, message.Header.Arg0);
        Assert.Equal(arg1, message.Header.Arg1);
        Assert.NotNull(message.Payload);
        Assert.Equal(text + "\0", Encoding.ASCII.GetString(message.Payload!));
    }

    /// <summary>
    /// Tests the <see cref="AdbProtocolMessage.DecodePayloadAsString()"/> method with a valid payload.
    /// Verifies that the decoded string matches the expected value.
    /// </summary>
    [Fact]
    public void DecodePayloadAsString_WithValidPayload_ShouldReturnCorrectString()
    {
        // Arrange
        var payload = Encoding.UTF8.GetBytes("TestPayload");
        var message = AdbProtocolMessage.Create(0, 0, 0, payload);

        // Act
        var result = message.DecodePayloadAsString();

        // Assert
        Assert.Equal("TestPayload", result);
    }
}
