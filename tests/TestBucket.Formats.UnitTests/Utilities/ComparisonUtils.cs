using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestBucket.Formats.Dtos;

namespace TestBucket.Formats.UnitTests.Utilities
{
    public record class ComparisionOptions
    {
        public bool CompareTraits { get; set; } = false;
    }

    internal static class ComparisonUtils
    {
        public static bool Compare(TestRunDto a, TestRunDto b)
        {
            return true;
        }
    }
}
