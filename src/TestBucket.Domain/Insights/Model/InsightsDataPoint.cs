using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestBucket.Domain.Insights.Model;
public record class InsightsDataPoint<T,U>(T Label, U Value)
{
}
