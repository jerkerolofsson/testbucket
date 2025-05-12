using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TestBucket.CodeCoverage.Models;
using TestBucket.CodeCoverage;
using TestBucket.Formats;

namespace TestBucket.Domain.Resources;
public class MediaTypeDetector
{
    public static string DetectType(string contentType, byte[] data)
    {
        if (contentType is "text/xml" or "application/json" or "application/octet-stream")
        {
            var resultFormat = TestResultDetector.Detect(data);
            if (resultFormat != TestResultFormat.UnknownFormat)
            {
                contentType = TestResultSerializerFactory.GetContentTypeFromFormat(resultFormat) ?? contentType;
            }
            else
            {
                var coverageFormat = CoverageFormatDetector.Detect(data);
                if (coverageFormat != CodeCoverageFormat.UnknownFormat)
                {
                    contentType = CodeCoverageParserFactory.GetContentTypeFromFormat(coverageFormat) ?? contentType;
                }
            }
        }
        return contentType;
    }
}
