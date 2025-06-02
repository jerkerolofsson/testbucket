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

    public FakeTimeProvider(DateTimeOffset dateTimeOffset)
    {
        _dateTimeOffset = dateTimeOffset;
    }

    public override DateTimeOffset GetUtcNow()
    {
        return _dateTimeOffset;
    }
}
