using System.Diagnostics.CodeAnalysis;

namespace TestBucket.Traits.Core.UnitTests;

[ExcludeFromCodeCoverage]
internal class FakeTimeProvider : TimeProvider
{
    private readonly DateTimeOffset _dateTimeOffset;

    public FakeTimeProvider(DateTimeOffset dateTimeOffset)
    {
        _dateTimeOffset = dateTimeOffset;
    }

    public override DateTimeOffset GetUtcNow()
    {
        return _dateTimeOffset;
    }
}
