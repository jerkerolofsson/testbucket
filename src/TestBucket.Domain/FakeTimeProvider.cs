using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestBucket.Domain;

[ExcludeFromCodeCoverage]
internal class FakeTimeProvider : TimeProvider
{
    private readonly DateTimeOffset _dateTimeOffset;
    private TimeSpan _offset = TimeSpan.Zero;

    public FakeTimeProvider(DateTimeOffset dateTimeOffset, TimeSpan? offset = null)
    {
        _dateTimeOffset = dateTimeOffset;
        if(offset is not null)
        {
            _offset = offset.Value;
        }
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
}
