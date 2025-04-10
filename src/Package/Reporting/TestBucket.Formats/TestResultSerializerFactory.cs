using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Formats.Ctrf;
using TestBucket.Formats.JUnit;
using TestBucket.Formats.MicrosoftTrx;
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

        private static readonly Dictionary<string, TestResultFormat> _mediaTypes = new() 
        {
            {"application/xml", TestResultFormat.JUnitXml },
            {"application/x-junit" , TestResultFormat.JUnitXml},
            {"application/junit" , TestResultFormat.JUnitXml},
            {"text/xml" , TestResultFormat.JUnitXml},
            {"text/xml+junit" , TestResultFormat.JUnitXml},
            {"application/xml+junit" , TestResultFormat.JUnitXml},
            {"application/x-xunit" , TestResultFormat.xUnitXml},
            {"application/xunit" , TestResultFormat.xUnitXml},
            {"application/xml+xunit" , TestResultFormat.xUnitXml},
            {"text/xml+xunit" , TestResultFormat.xUnitXml},
            {"application/x-nunit" , TestResultFormat.NUnitXml},
            {"application/nunit" , TestResultFormat.NUnitXml},
            {"application/xml+nunit" , TestResultFormat.NUnitXml},
            {"text/xml+nunit" , TestResultFormat.NUnitXml},
            {"application/x-trx" , TestResultFormat.MicrosoftTrx},
            {"application/trx" , TestResultFormat.MicrosoftTrx},
            {"application/xml+trx" , TestResultFormat.MicrosoftTrx},
            {"text/xml+trx" , TestResultFormat.MicrosoftTrx},
            {"application/x-ctrf" , TestResultFormat.CommonTestReportFormat},
            {"application/ctrf" , TestResultFormat.CommonTestReportFormat},
            {"application/json" , TestResultFormat.CommonTestReportFormat},
            {"application/json+ctrf" , TestResultFormat.CommonTestReportFormat}

        };


        /// <summary>
        /// Returns media-type from test result format 
        /// </summary>
        /// <param name="format"></param>
        /// <returns></returns>
        public static string? GetContentTypeFromFormat(TestResultFormat format)
        {
            var matches = _mediaTypes.Where(kvp => kvp.Value == format).ToList();
            if (matches is not null && matches.Count > 0)
            {
                return matches[0].Key;
            }

            return null;
        }

        /// <summary>
        /// Returns test result format from media-type / content-type
        /// </summary>
        /// <param name="contentType"></param>
        /// <returns></returns>
        public static TestResultFormat GetFormatFromContentType(string? contentType)
        {
            if(contentType is null)
            {
                return TestResultFormat.UnknownFormat;
            }
            var header = new System.Net.Mime.ContentType(contentType);

            if(_mediaTypes.TryGetValue(contentType, out var format))
            {
                return format;
            }

            return TestResultFormat.UnknownFormat;
        }

        public static ITestResultSerializer Create(TestResultFormat format)
        {
            return format switch
            {
                TestResultFormat.MicrosoftTrx => new TrxSerializer(),
                TestResultFormat.JUnitXml => new JUnitSerializer(),
                TestResultFormat.xUnitXml => new XUnitSerializer(),
                TestResultFormat.CommonTestReportFormat => new CtrfXunitSerializer(),
                _ => throw new NotImplementedException($"Unknown format: {format}")
            };
        }
    }
}
