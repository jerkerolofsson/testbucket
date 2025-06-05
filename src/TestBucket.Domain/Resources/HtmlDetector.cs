using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static System.Net.Mime.MediaTypeNames;

namespace TestBucket.Domain.Resources;
internal class HtmlDetector
{
    public static bool IsHtml(string content)
    {
        if (string.IsNullOrWhiteSpace(content))
        {
            return false;
        }

        content = content.Trim();
        if (content.StartsWith("<!DOCTYPE html") || content.StartsWith("<html"))
        {
            return true;
        }

        return false;
    }
}
