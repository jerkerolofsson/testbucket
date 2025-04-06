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
        public static ITestResultSerializer CreateFromContentType(string contentType)
        {
            TestResultFormat format = GetFormatFromContentType(contentType);

            return Create(format);
        }

        public static TestResultFormat GetFormatFromContentType(string? contentType)
        {
            if(contentType is null)
            {
                return TestResultFormat.UnknownFormat;
            }
            var header = new System.Net.Mime.ContentType(contentType);

            return header.MediaType switch
            {
                "application/xml" => TestResultFormat.JUnitXml,
                "application/x-junit" => TestResultFormat.JUnitXml,
                "application/junit" => TestResultFormat.JUnitXml,
                "text/xml" => TestResultFormat.JUnitXml,
                "text/xml+junit" => TestResultFormat.xUnitXml,

                "application/x-xunit" => TestResultFormat.xUnitXml,
                "application/xunit" => TestResultFormat.xUnitXml,
                "text/xml+xunit" => TestResultFormat.xUnitXml,

                "application/x-nunit" => TestResultFormat.NUnitXml,
                "application/nunit" => TestResultFormat.NUnitXml,
                "text/xml+nunit" => TestResultFormat.NUnitXml,

                "application/x-trx" => TestResultFormat.MicrosoftTrx,
                "application/trx" => TestResultFormat.MicrosoftTrx,
                "text/xml+trx" => TestResultFormat.MicrosoftTrx,

                "application/x-ctrf" => TestResultFormat.CommonTestReportFormat,
                "application/ctrf" => TestResultFormat.CommonTestReportFormat,
                "application/json" => TestResultFormat.CommonTestReportFormat,

                _ => TestResultFormat.UnknownFormat
            };
        }

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
