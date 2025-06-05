using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TestBucket.CodeCoverage.Models;
using TestBucket.CodeCoverage;
using TestBucket.Formats;
using TestBucket.Domain.Testing.ImportExport;

namespace TestBucket.Domain.Resources;
public class MediaTypeDetector
{
    public static readonly Dictionary<string, string> ExtensionToMediaType = new(StringComparer.OrdinalIgnoreCase)
    {
        // Text and data formats
        { ".json", "application/json" },
        { ".xml", "text/xml" },
        { ".txt", "text/plain" },
        { ".csv", "text/csv" },
        { ".yml", "application/x-yaml" },
        { ".yaml", "application/x-yaml" },
        { ".html", "text/html" },
        { ".htm", "text/html" },
        { ".md", "text/markdown" },
        { ".pdf", "application/pdf" },
        { ".zip", "application/zip" },
        { ".tar", "application/x-tar" },
        { ".gz", "application/gzip" },
        { ".7z", "application/x-7z-compressed" },
        { ".rar", "application/vnd.rar" },
        { ".bin", "application/octet-stream" },

        // Images
        { ".png", "image/png" },
        { ".jpg", "image/jpeg" },
        { ".jpeg", "image/jpeg" },
        { ".gif", "image/gif" },
        { ".bmp", "image/bmp" },
        { ".svg", "image/svg+xml" },

        // Audio/Video
        { ".mp3", "audio/mpeg" },
        { ".wav", "audio/wav" },
        { ".mp4", "video/mp4" },
        { ".mov", "video/quicktime" },
        { ".avi", "video/x-msvideo" },

        // Programming languages
        { ".cs", "text/x-csharp" },         // C#
        { ".vb", "text/x-viusualbasic" },         // Visual Basic
        { ".fs", "text/x-fsharp" },         // F#
        { ".java", "text/x-java-source" },
        { ".js", "application/javascript" },
        { ".ts", "application/typescript" },
        { ".jsx", "text/jsx" },
        { ".tsx", "text/tsx" },
        { ".py", "text/x-python" },
        { ".rb", "text/x-ruby" },
        { ".php", "application/x-httpd-php" },
        { ".go", "text/x-go" },
        { ".rs", "text/rust" },
        { ".cpp", "text/x-c++src" },
        { ".cxx", "text/x-c++src" },
        { ".cc", "text/x-c++src" },
        { ".c", "text/x-csrc" },
        { ".h", "text/x-chdr" },
        { ".hpp", "text/x-c++hdr" },
        { ".swift", "text/x-swift" },
        { ".kt", "text/x-kotlin" },
        { ".kts", "text/x-kotlin" },
        { ".scala", "text/x-scala" },
        { ".sh", "application/x-sh" },
        { ".bat", "application/x-bat" },
        { ".ps1", "text/x-powershell" },
        { ".sql", "application/sql" },
        { ".dart", "text/x-dart" },
        { ".lua", "text/x-lua" },
        { ".jsonc", "application/json" },
        { ".r", "text/x-r" },
        { ".m", "text/x-matlab" },       // Could also be Objective-C
        { ".pl", "text/x-perl" },
        { ".groovy", "text/x-groovy" },
        { ".erl", "text/x-erlang" },
        { ".ex", "text/x-elixir" },
        { ".exs", "text/x-elixir" },
        { ".coffee", "text/x-coffeescript" },
        { ".scss", "text/x-scss" },
        { ".sass", "text/x-sass" },
        { ".less", "text/x-less" },
        { ".css", "text/css" },
        { ".razor", "text/x-razor" },      // Blazor/Razor
        { ".razor.cs", "text/x-csharp" },   // Blazor code-behind
    };

    private const string DefaultContentType = "application/octet-stream";

    /// <summary>
    /// Detects media-type from name or from the contents
    /// </summary>
    /// <param name="name"></param>
    /// <param name="contentType"></param>
    /// <param name="data">File content</param>
    /// <returns></returns>
    public static string DetectType(string? name, string? contentType, byte[] data)
    {
        if (contentType is null)
        {
            if (name is not null)
            {
                var ext = Path.GetExtension(name).ToLower();
                if(ExtensionToMediaType.TryGetValue(ext, out var contentTypeFromExtension))
                {
                    contentType = contentTypeFromExtension;
                }
            }
        }
        
        // Try detect format from magic
        if(contentType is null)
        {
            contentType = FileMagicDetector.DetectFileType(data);
        }

        if (contentType is null)
        {
            string text = TextConversionUtils.FromBomEncoded(data);
            if (JsonDetector.IsJson(text))
            {
                contentType = "application/json";
            }
            else if (XmlDetector.IsXml(text))
            {
                contentType = "text/xml";
            }
            else if (HtmlDetector.IsHtml(text))
            {
                contentType = "text/html";
            }
        }

        contentType ??= DefaultContentType;

        if (contentType is "text/xml" or "application/json" or DefaultContentType)
        {
            var resultFormat = TestResultDetector.Detect(data);
            if (resultFormat != TestResultFormat.UnknownFormat)
            {
                // A well-known test result format could be detected
                contentType = TestResultSerializerFactory.GetContentTypeFromFormat(resultFormat) ?? contentType;
            }
            else
            {
                var coverageFormat = CoverageFormatDetector.Detect(data);
                if (coverageFormat != CodeCoverageFormat.UnknownFormat)
                {
                    // A well-known code coverage format could be detected
                    contentType = CodeCoverageParserFactory.GetContentTypeFromFormat(coverageFormat) ?? contentType;
                }
            }
        }
        return contentType ?? DefaultContentType;
    }
}
