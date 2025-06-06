using TestBucket.Traits.Core.Metrics;
using TestBucket.Traits.Xunit;

namespace TestBucket.Traits.Core.UnitTests.Metrics;

/// <summary>
/// Unit tests for the <see cref="MetricSerializer"/> class, verifying serialization and deserialization
/// of <see cref="TestResultMetric"/> objects, including handling of units and error cases.
/// </summary>
[UnitTest]
[Component("Traits")]
[Feature("Import Test Results")]
[EnrichedTest]
public class MetricSerializerTests
{
    /// <summary>
    /// Verifies that <see cref="MetricSerializer.SerializeName"/> returns the expected string format.
    /// </summary>
    [Fact]
    public void SerializeName_ReturnsExpectedFormat()
    {
        var metric = new TestResultMetric("meter-name", "foo", 1.23, "ms");
        var result = MetricSerializer.SerializeName(metric);
        Assert.Equal("metric:meter-name:foo", result);
    }

    /// <summary>
    /// Verifies that <see cref="MetricSerializer.SerializeValue"/> appends the unit to the value when present.
    /// </summary>
    [Fact]
    public void SerializeValue_WithUnit_AppendsUnit()
    {
        var metric = new TestResultMetric("meter-name", "foo", 1.23, "ms");
        var result = MetricSerializer.SerializeValue(metric);
        Assert.Equal("1.23ms", result);
    }

    /// <summary>
    /// Verifies that <see cref="MetricSerializer.SerializeValue"/> returns only the value when the unit is null.
    /// </summary>
    [Fact]
    public void SerializeValue_WithoutUnit_OnlyValue()
    {
        var metric = new TestResultMetric("meter-name", "foo", 1.23, null);
        var result = MetricSerializer.SerializeValue(metric);
        Assert.Equal("1.23", result);
    }

    /// <summary>
    /// Verifies that <see cref="MetricSerializer.Deserialize"/> correctly parses a metric with a unit.
    /// </summary>
    [Fact]
    public void Deserialize_ValidInputWithUnit_ParsesCorrectly()
    {
        var metric = MetricSerializer.Deserialize("metric:meter-name:foo", "1.23ms");
        Assert.Equal("meter-name", metric.MeterName);
        Assert.Equal("foo", metric.Name);
        Assert.Equal(1.23, metric.Value, 3);
        Assert.Equal("ms", metric.Unit);
    }

    /// <summary>
    /// Verifies that <see cref="MetricSerializer.Deserialize"/> correctly parses a metric without a unit.
    /// </summary>
    [Fact]
    public void Deserialize_ValidInputWithoutUnit_ParsesCorrectly()
    {
        var metric = MetricSerializer.Deserialize("metric:meter-name:foo", "1.23");
        Assert.Equal("meter-name", metric.MeterName);
        Assert.Equal("foo", metric.Name);
        Assert.Equal(1.23, metric.Value, 3);
        Assert.Null(metric.Unit);
    }

    /// <summary>
    /// Verifies that <see cref="MetricSerializer.Deserialize"/> throws a <see cref="FormatException"/>
    /// when the metric name format is invalid.
    /// </summary>
    [Fact]
    public void Deserialize_InvalidName_ThrowsFormatException()
    {
        Assert.Throws<FormatException>(() => MetricSerializer.Deserialize("invalid", "1.23ms"));
    }

    /// <summary>
    /// Verifies that <see cref="MetricSerializer.Deserialize"/> throws a <see cref="FormatException"/>
    /// when the value cannot be parsed as a number.
    /// </summary>
    [Fact]
    public void Deserialize_InvalidValue_ThrowsFormatException()
    {
        Assert.Throws<FormatException>(() => MetricSerializer.Deserialize("metric:meter-name:foo", "abc"));
    }

    /// <summary>
    /// Verifies that <see cref="MetricSerializer.Deserialize"/> trims spaces from the unit.
    /// </summary>
    [Fact]
    public void Deserialize_UnitWithSpaces_ParsesUnitTrimmed()
    {
        var metric = MetricSerializer.Deserialize("metric:meter-name:foo", "1.23 ms");
        Assert.Equal("ms", metric.Unit);
    }

    /// <summary>
    /// Verifies that <see cref="MetricSerializer.SerializeName"/> throws a <see cref="FormatException"/>
    /// when the meter-name or name is invalid.
    /// </summary>
    /// <param name="meterName">The namespace to test.</param>
    /// <param name="name">The name to test.</param>
    [Theory]
    [InlineData("invalid namespace", "foo")]
    [InlineData("ns!", "foo")]
    [InlineData("", "foo")]
    [InlineData("meter-name", "invalid name")]
    [InlineData("meter-name", "foo!")]
    [InlineData("meter-name", "")]
    public void SerializeName_InvalidNamespaceOrName_ThrowsFormatException(string meterName, string name)
    {
        var metric = new TestResultMetric(meterName, name, 1.23, "ms");
        Assert.Throws<FormatException>(() => MetricSerializer.SerializeName(metric));
    }

    /// <summary>
    /// Verifies that <see cref="MetricSerializer.Deserialize"/> throws a <see cref="FormatException"/>
    /// when the serialized name or value is invalid.
    /// </summary>
    /// <param name="serializedName">The serialized metric name to test.</param>
    /// <param name="value">The value to test.</param>
    [Theory]
    [InlineData("metric:invalid namespace:foo", "1.23ms")]
    [InlineData("metric:ns!:foo", "1.23ms")]
    [InlineData("metric::foo", "1.23ms")]
    [InlineData("metric:meter-name:invalid name", "1.23ms")]
    [InlineData("metric:meter-name:foo!", "1.23ms")]
    [InlineData("metric:meter-name:", "1.23ms")]
    public void Deserialize_InvalidNamespaceOrName_ThrowsFormatException(string serializedName, string value)
    {
        Assert.Throws<FormatException>(() => MetricSerializer.Deserialize(serializedName, value));
    }

    /// <summary>
    /// Verifies that <see cref="MetricSerializer.SerializeValue"/> and <see cref="MetricSerializer.Deserialize"/>
    /// correctly handle units with a degree symbol (°).
    /// </summary>
    [Fact]
    public void SerializeValue_WithDegreeSymbolUnit_AppendsUnit()
    {
        var metric = new TestResultMetric("meter-name", "temperature", 23.5, "°C");
        var result = MetricSerializer.SerializeValue(metric);

        Assert.Equal("23.5°C", result);
    }

    /// <summary>
    /// Verifies that <see cref="MetricSerializer.Deserialize"/> correctly parses a metric with a degree symbol unit.
    /// </summary>
    [Fact]
    public void Deserialize_ValidInputWithDegreeSymbolUnit_ParsesCorrectly()
    {
        var metric = MetricSerializer.Deserialize("metric:meter-name:temperature", "23.5°C");
        Assert.Equal("meter-name", metric.MeterName);
        Assert.Equal("temperature", metric.Name);
        Assert.Equal(23.5, metric.Value, 3);
        Assert.Equal("°C", metric.Unit);
    }

    /// <summary>
    /// Verifies that <see cref="MetricSerializer.Deserialize"/> trims spaces from a unit with a degree symbol.
    /// </summary>
    [Fact]
    public void Deserialize_UnitWithDegreeSymbolAndSpaces_ParsesUnitTrimmed()
    {
        var metric = MetricSerializer.Deserialize("metric:meter-name:temperature", "23.5 °C");
        Assert.Equal("°C", metric.Unit);
    }

    /// <summary>
    /// Verifies that <see cref="MetricSerializer.Deserialize"/> trims spaces from a unit with a degree symbol.
    /// </summary>
    [Fact]
    public void Deserialize_UnitWithDegreeSymbolWithoutMetricPrefix_ParsesUnitTrimmed()
    {
        var metric = MetricSerializer.Deserialize("xunit:temperature", "23.5 °C");
        Assert.Equal("xunit", metric.MeterName);
        Assert.Equal("temperature", metric.Name);
        Assert.Equal("°C", metric.Unit);
    }


    /// <summary>
    /// Verifies that <see cref="MetricSerializer.Deserialize"/> parses the created date if present
    /// </summary>
    [Fact]
    public void Deserialize_WithCreatedSuffix_ExtractsCreatedDate()
    {
        var metric = MetricSerializer.Deserialize("xunit:duration", "23ms@1700000000000");

        Assert.Equal("xunit", metric.MeterName);
        Assert.Equal("duration", metric.Name);
        Assert.Equal("ms", metric.Unit);

        var expectedDateTimeOffset = DateTimeOffset.FromUnixTimeMilliseconds(1700000000000);
        Assert.Equal(expectedDateTimeOffset, metric.Created);
    }


    /// <summary>
    /// Verifies that <see cref="MetricSerializer.Deserialize"/> sets the correct default time if not sspecified
    /// </summary>
    [Fact]
    public void Deserialize_WithoutCreatedSuffix_UsesTimeProvider()
    {
        var timeProvider = new FakeTimeProvider(new DateTimeOffset(2025, 6, 6, 10, 1, 2, TimeSpan.Zero));
        var metric = MetricSerializer.Deserialize("xunit:duration", "23ms", timeProvider);

        Assert.Equal("xunit", metric.MeterName);
        Assert.Equal("duration", metric.Name);
        Assert.Equal("ms", metric.Unit);

        var expectedDateTimeOffset = timeProvider.GetUtcNow();
        Assert.Equal(expectedDateTimeOffset, metric.Created);
    }
}