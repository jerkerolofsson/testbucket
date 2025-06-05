using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace TestBucket.Domain.Resources;
internal class JsonDetector
{
    /// <summary>
    /// Detects if the given content is in JSON format.
    /// </summary>
    /// <param name="content">The content to check.</param>
    /// <returns>True if the content is JSON, otherwise false.</returns>
    public static bool IsJson(string content)
    {
        if (string.IsNullOrWhiteSpace(content))
        {
            return false;
        }
        try
        {
            JsonDocument.Parse(content);
            return true;
        }
        catch (JsonException)
        {
            return false;
        }
        catch (NotSupportedException)
        {
            return false;
        }
    }
}
