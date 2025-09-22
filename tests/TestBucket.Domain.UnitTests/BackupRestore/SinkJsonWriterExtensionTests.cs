using System.Text.Json;
using TestBucket.Domain.Export;
using TestBucket.Domain.Export.Handlers;
using NSubstitute;
using Xunit;


namespace TestBucket.Domain.UnitTests.BackupRestore;

/// <summary>
/// Unit tests for <see cref="SinkJsonWriterExtension"/> extension methods related to writing JSON entities to a sink.
/// </summary>
[Component("Backup")]
[UnitTest]
[EnrichedTest]
[FunctionalTest]
public class SinkJsonWriterExtensionTests
{
    internal class DummyDto
    {
        public string? Name { get; set; }
        public int? Value { get; set; }
    }

    /// <summary>
    /// Verifies that items are written to the sink with correct source, type, ID and serialized JSON content.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task WriteJsonEntityAsync_WritesSerializedJsonToSink()
    {
        // Arrange
        var dto = new DummyDto { Name = "Test", Value = 42 };
        string? capturedSource = null, capturedEntityType = null, capturedEntityId = null;
        MemoryStream? capturedStream = null;

        var substituteSink = Substitute.For<IDataExporterSink>();
        substituteSink
            .WhenForAnyArgs(x => x.WriteEntityAsync(default!, default!, default!, default!, default))
            .Do(callInfo =>
            {
                capturedSource = callInfo.ArgAt<string>(0);
                capturedEntityType = callInfo.ArgAt<string>(1);
                capturedEntityId = callInfo.ArgAt<string>(2);
                var stream = callInfo.ArgAt<System.IO.Stream>(3);
                capturedStream = new MemoryStream();
                stream.CopyTo(capturedStream);
                capturedStream.Position = 0;
            });

        // Act
        await substituteSink.WriteJsonEntityAsync("src", "type", "id", dto, CancellationToken.None);

        // Assert
        Assert.Equal("src", capturedSource);
        Assert.Equal("type", capturedEntityType);
        Assert.Equal("id", capturedEntityId);
        Assert.NotNull(capturedStream);

        // Verify JSON content
        capturedStream.Position = 0;
        using var reader = new StreamReader(capturedStream, System.Text.Encoding.UTF8);
        var json = await reader.ReadToEndAsync(TestContext.Current.CancellationToken);
        var deserialized = JsonSerializer.Deserialize<DummyDto>(json);
        Assert.Equal(dto.Name, deserialized?.Name);
        Assert.Equal(dto.Value, deserialized?.Value);
    }
}