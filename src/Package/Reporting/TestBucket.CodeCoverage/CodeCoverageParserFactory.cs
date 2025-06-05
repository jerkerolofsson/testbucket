using TestBucket.CodeCoverage.Models;
using TestBucket.CodeCoverage.Parsers;

namespace TestBucket.CodeCoverage
{
    /// <summary>
    /// Factory class for creating code coverage parsers and handling content type/format conversions.
    /// </summary>
    public static class CodeCoverageParserFactory
    {
        /// <summary>
        /// Creates a <see cref="ParserBase"/> instance based on the specified content type.
        /// </summary>
        /// <param name="contentType">The content type (media type) of the code coverage report.</param>
        /// <returns>A <see cref="ParserBase"/> for the specified content type.</returns>
        public static ParserBase CreateFromContentType(string contentType)
        {
            CodeCoverageFormat format = GetFormatFromContentType(contentType);

            return Create(format);
        }

        /// <summary>
        /// Maps supported media types to their corresponding <see cref="CodeCoverageFormat"/>.
        /// </summary>
        private static readonly Dictionary<string, CodeCoverageFormat> _mediaTypes = new()
        {
            {"application/x-cobertura" , CodeCoverageFormat.Cobertura},
        };

        /// <summary>
        /// Returns the media type (content type) string for a given <see cref="CodeCoverageFormat"/>.
        /// </summary>
        /// <param name="format">The code coverage format.</param>
        /// <returns>The media type string if found; otherwise, <c>null</c>.</returns>
        public static string? GetContentTypeFromFormat(CodeCoverageFormat format)
        {
            var matches = _mediaTypes.Where(kvp => kvp.Value == format).ToList();
            if (matches.Count > 0)
            {
                return matches[0].Key;
            }

            return null;
        }

        /// <summary>
        /// Returns the <see cref="CodeCoverageFormat"/> for a given media type (content type) string.
        /// </summary>
        /// <param name="contentType">The content type string.</param>
        /// <returns>The corresponding <see cref="CodeCoverageFormat"/> if recognized; otherwise, <see cref="CodeCoverageFormat.UnknownFormat"/>.</returns>
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

        /// <summary>
        /// Creates a <see cref="ParserBase"/> instance for the specified <see cref="CodeCoverageFormat"/>.
        /// </summary>
        /// <param name="format">The code coverage format.</param>
        /// <returns>A <see cref="ParserBase"/> for the specified format.</returns>
        /// <exception cref="ArgumentException">Thrown if the format is unknown or unsupported.</exception>
        public static ParserBase Create(CodeCoverageFormat format)
        {
            return format switch
            {
                CodeCoverageFormat.Cobertura => new CoberturaParser(),
                _ => throw new ArgumentException($"Unknown format: {format}")
            };
        }
    }
}