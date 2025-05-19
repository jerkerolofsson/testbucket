using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestBucket.Domain.Insights.Model;
public record class InsightsDataPoint<T,U>(T Label, U Value)
{
    public double ToDouble()
    {
        if (Value is double d)
        {
            return d;
        }
        else if (Value is int i)
        {
            return i;
        }
        else if (Value is long l)
        {
            return l;
        }
        else if (Value is float f)
        {
            return f;
        }
        else if (Value is decimal m)
        {
            return (double)m;
        }
        else
        {
            throw new InvalidOperationException($"Cannot convert {Value} to double");
        }
    }
}
