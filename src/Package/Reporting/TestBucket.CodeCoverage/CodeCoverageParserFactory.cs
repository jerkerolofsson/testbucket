using TestBucket.CodeCoverage.Models;
using TestBucket.CodeCoverage.Parsers;


namespace TestBucket.CodeCoverage
{
    /// <summary>
    /// Class to deserialize test artifact files
    /// </summary>
    public static class CodeCoverageParserFactory
    {
        public static ParserBase CreateFromContentType(string contentType)
        {
            CodeCoverageFormat format = GetFormatFromContentType(contentType);

            return Create(format);
        }

        private static readonly Dictionary<string, CodeCoverageFormat> _mediaTypes = new()
        {
            {"application/x-cobertura" , CodeCoverageFormat.Cobertura},
           
        };


        /// <summary>
        /// Returns media-type from code coverage format 
        /// </summary>
        /// <param name="format"></param>
        /// <returns></returns>
        public static string? GetContentTypeFromFormat(CodeCoverageFormat format)
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
        public static CodeCoverageFormat GetFormatFromContentType(string? contentType)
        {
            if (contentType is null)
            {
                return CodeCoverageFormat.UnknownFormat;
            }
            var header = new System.Net.Mime.ContentType(contentType);

            if (_mediaTypes.TryGetValue(contentType, out var format))
            {
                return format;
            }

            return CodeCoverageFormat.UnknownFormat;
        }

        public static ParserBase Create(CodeCoverageFormat format)
        {
            return format switch
            {
                CodeCoverageFormat.Cobertura => new CoberturaParser(),
                _ => throw new NotImplementedException($"Unknown format: {format}")
            };
        }
    }
}
