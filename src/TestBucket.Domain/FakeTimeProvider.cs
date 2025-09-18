using System;
using System.Diagnostics.CodeAnalysis;

namespace TestBucket.Domain;

[ExcludeFromCodeCoverage]
internal class FakeTimeProvider : TimeProvider
{
    private DateTimeOffset _dateTimeOffset;
    private readonly TimeSpan _offset = TimeSpan.Zero;

    public FakeTimeProvider(DateTimeOffset dateTimeOffset, TimeSpan? offset = null)
    {
        _dateTimeOffset = dateTimeOffset;
        if(offset is not null)
        {
            _offset = offset.Value;
        }
    }

    public DateTimeOffset Advance(TimeSpan amount)
    {
        _dateTimeOffset += amount;
        return _dateTimeOffset;
    }

    public void SetDateTime(DateTimeOffset dateTime)
    {
        _dateTimeOffset = dateTime;
    }

    public override TimeZoneInfo LocalTimeZone
    {
        get
        {
            return TimeZoneInfo.CreateCustomTimeZone("FakeTimeZone", _offset, "FakeTimeZone", "FakeTimeZone");
        }
    }

    public override DateTimeOffset GetUtcNow()
    {
        return _dateTimeOffset;
    }

    public DateTimeOffset GetCurrentTime()
    {
        return GetUtcNow();
    }
}
