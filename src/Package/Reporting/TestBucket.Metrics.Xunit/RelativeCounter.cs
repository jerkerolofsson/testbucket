using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace TestBucket.Metrics.Xunit;
internal class RelativeCounter<T> where T : INumber<T>
{
    private T _value = T.Zero;
    private T _baseline = T.Zero;

    public T Value => _value - _baseline;

    public void Increment(T amount)
    {
        _value += amount;
    }

    public void Set(T value)
    {
        _value = value;
    }

    public void Reset()
    {
        _baseline = _value;
    }
}
