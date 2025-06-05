using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace TestBucket.Domain.Resources;
internal class XmlDetector
{
    /// <summary>
    /// Returns true if content is valid XML
    /// </summary>
    /// <param name="content"></param>
    /// <returns></returns>
    public static bool IsXml(string content)
    {
        if (string.IsNullOrWhiteSpace(content))
        {
            return false;
        }
        // Check for XML declaration
        if (content.TrimStart().StartsWith("<?xml", StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        try
        {
            // Try to parse the content as XML using XDocument
            XDocument.Parse(content);
            return true;
        }
        catch
        {
            return false;
        }
    }
}
