using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestBucket.Domain.Shared;

/// <summary>
/// Helper class that parses strings with text and embedded search filters.
/// 
/// For example if the input contains is:xyz and keywords contains "is" then result will contain is:xyz
/// 
/// </summary>
internal class SearchStringParser
{
    public static string? Parse(string text, Dictionary<string,string> result, HashSet<string> keywords)
    {
        List<string> words = new();
        foreach(var item in text.Split(' '))
        {
            if(item.Contains(':'))
            {
                var p = item.IndexOf(':');
                var keyword = item.Substring(0, p);
                if (keywords.Contains(keyword))
                {
                    var value = item.Substring(p + 1);
                    result[keyword] = value;
                }
                else
                {
                    words.Add(item);
                }
            }
            else
            {
                words.Add(item);
            }
        }
        if(words.Count > 0)
        {
            return string.Join(' ', words);
        }
        return null;
    }
}
