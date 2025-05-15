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
    public static string? Parse(string text, Dictionary<string,string> result, List<FieldFilter> fieldFilters, HashSet<string> keywords, IReadOnlyList<FieldDefinition> fields)
    {
        List<string> words = new();
        foreach(var item in text.Split(' '))
        {
            if(item.Contains(':'))
            {
                var p = item.IndexOf(':');
                var keyword = item.Substring(0, p);
                var value = item.Substring(p + 1);
                if (keywords.Contains(keyword))
                {
                    result[keyword] = value;
                }
                else
                {
                    // Check if there is a matching field
                    var field = fields.Where(x => x.Name.ToLower() == keyword.ToLower()).FirstOrDefault();
                    if (field is null)
                    {
                        words.Add(item);
                    }
                    else
                    {
                        fieldFilters.Add(new FieldFilter { FilterDefinitionId = field.Id, Name=keyword, StringValue = value });
                    }
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
