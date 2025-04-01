using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Formats.Ctrf;
using TestBucket.Formats.JUnit;
using TestBucket.Formats.XUnit;

namespace TestBucket.Formats
{
    public static class TestResultSerializerFactory
    {
        public static ITestResultSerializer Create(TestResultFormat format)
        {
            return format switch
            {
                TestResultFormat.JUnitXml => new JUnitSerializer(),
                TestResultFormat.xUnitXml => new XUnitSerializer(),
                TestResultFormat.CommonTestReportFormat => new CtrfXunitSerializer(),
                _ => throw new NotImplementedException($"Unknown format: {format}")
            };
        }
    }
}
