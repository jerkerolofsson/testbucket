using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestBucket.AdbProxy.DeviceHandling;
public class GetPropParser
{
    private static string? ExtractTextInBrackets(string line, out string? remainder)
    {
        remainder = null;
        var bracketStart = line.IndexOf('[');
        if (bracketStart >= 0)
        {
            var bracketEnd = line.IndexOf(']', bracketStart);
            if (bracketEnd >= 0 && line.Length > bracketEnd)
            {
                var length = bracketEnd - bracketStart - 1;

                var textInBrackets = line.Substring(bracketStart + 1, length);
                remainder = line.Substring(bracketEnd + 1);
                return textInBrackets;
            }
        }
        return null;
    }

    public static Dictionary<string,string> Parse(string text)
    {
        var result = new Dictionary<string,string>();
        foreach(var line in text.Split('\n'))
        {
            var key = ExtractTextInBrackets(line, out string? remainder);
            if(key is not null && remainder is not null)
            {
                var value = ExtractTextInBrackets(remainder, out var _);
                if(value is not null)
                {
                    result[key] = value;
                }
            }
        }
        return result;
    }
}
