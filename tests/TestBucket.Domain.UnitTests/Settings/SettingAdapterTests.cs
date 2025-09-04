using System.Security.Claims;
using System.Threading.Tasks;

using TestBucket.Domain.Settings;
using TestBucket.Domain.Settings.Models;

namespace TestBucket.Domain.UnitTests.Settings;

/// <summary>
/// Unit tests for <see cref="SettingAdapter"/>.
/// </summary>
[UnitTest]
[Component("Settings")]
[Feature("Settings 1.0")]
[EnrichedTest]
[FunctionalTest]

public class SettingAdapterTests
{
    /// <summary>
    /// A test implementation of <see cref="SettingAdapter"/> for testing purposes.
    /// </summary>
    private class TestSettingAdapter : SettingAdapter
    {
        public FieldValue? LastWrittenValue { get; private set; }
        public override Task<FieldValue> ReadAsync(SettingContext context)
        {
            // Return a dummy value for testing
            return Task.FromResult(new FieldValue { StringValue = "TestValue", FieldDefinitionId = 1 });
        }

        public override Task WriteAsync(SettingContext context, FieldValue value)
        {
            LastWrittenValue = value;
            return Task.CompletedTask;
        }
    }

    /// <summary>
    /// Verifies that <see cref="SettingAdapter.Metadata"/> is initialized correctly.
    /// </summary>
    [Fact]
    public void Metadata_ShouldBeInitialized()
    {
        var adapter = new TestSettingAdapter();
        Assert.NotNull(adapter.Metadata);
        Assert.Equal("General", adapter.Metadata.Category.Name);
        Assert.Equal("Common", adapter.Metadata.Section.Name);
        Assert.Equal("Undefined", adapter.Metadata.Name);
    }

    /// <summary>
    /// Verifies that <see cref="SettingAdapter.ReadAsync"/> returns the expected value.
    /// </summary>
    [Fact]
    public async Task ReadAsync_ShouldReturnExpectedValue()
    {
        var adapter = new TestSettingAdapter();
        var context = new SettingContext() { Principal = new ClaimsPrincipal(), TenantId = "tenant-1" };
        var value = await adapter.ReadAsync(context);
        Assert.NotNull(value);
        Assert.Equal("TestValue", value.StringValue);
    }

    /// <summary>
    /// Verifies that <see cref="SettingAdapter.WriteAsync"/> stores the value.
    /// </summary>
    [Fact]
    public async Task WriteAsync_ShouldStoreValue()
    {
        var adapter = new TestSettingAdapter();
        var context = new SettingContext() { Principal = new ClaimsPrincipal(), TenantId = "tenant-1" };
        var fieldValue = new FieldValue { StringValue = "WrittenValue", FieldDefinitionId = 1 };
        await adapter.WriteAsync(context, fieldValue);
        Assert.Equal("WrittenValue", adapter.LastWrittenValue?.StringValue);
    }
}