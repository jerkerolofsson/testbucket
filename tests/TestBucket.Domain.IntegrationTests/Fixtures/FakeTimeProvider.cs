using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestBucket.Domain;
internal class FakeTimeProvider : TimeProvider
{
    private DateTimeOffset _dateTimeOffset;

    public FakeTimeProvider(DateTimeOffset dateTimeOffset)
    {
        _dateTimeOffset = dateTimeOffset;
    }

    public void SetTime(DateTimeOffset now)
    {
        _dateTimeOffset = now;
    }
    public override DateTimeOffset GetUtcNow()
    {
        return _dateTimeOffset;
    }
}
