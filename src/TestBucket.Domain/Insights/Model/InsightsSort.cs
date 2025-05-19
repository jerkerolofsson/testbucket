using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestBucket.Domain.Insights.Model;
public enum InsightsSort
{
    Unsorted,

    /// <summary>
    /// Sort by the label, ascending
    /// </summary>
    LabelAscending,

    LabelDescending,

    ValueAscending,

    ValueDescending,    
}
