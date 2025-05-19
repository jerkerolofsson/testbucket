using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Mediator;

using UglyToad.PdfPig.Content;

namespace TestBucket.Domain.Shared;

/// <summary>
/// Helper class that parses strings with text and embedded search filters.
/// 
/// For example if the input contains is:xyz and keywords contains "is" then result will contain is:xyz
/// 
/// </summary>
internal class SearchStringParser
{
    private static readonly Regex _splitRegex = new Regex("(?<=^[^\"]*(?:\"[^\"]*\"[^\"]*)*) (?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)");

    /// <summary>
    /// Like trim, but only removes one
    /// </summary>
    /// <param name="word"></param>
    /// <returns></returns>
    private static string RemoveQuotes(string word)
    {
        if (word.Length >= 2)
        {
            if (word[0] == '"')
            {
                word = word[1..];
            }
            if (word[^1] == '"')
            {
                word = word[..^1];
            }
        }
        return word;
    }

    public static string? Parse(string text, Dictionary<string,string> result, List<FieldFilter> fieldFilters, HashSet<string> keywords, IReadOnlyList<FieldDefinition> fields)
    {
        List<string> words = new();
        //foreach(var item in text.Split(' '))
        foreach(var item in _splitRegex.Split(text))
        {
            var word = item;
            

            if(word.Contains(':'))
            {
                var p = word.IndexOf(':');
                var keyword = RemoveQuotes(word.Substring(0, p));
                var value = RemoveQuotes(word.Substring(p + 1));
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
                        words.Add(word);
                    }
                    else
                    {
                        fieldFilters.Add(new FieldFilter { FilterDefinitionId = field.Id, Name=keyword, StringValue = value });
                    }
                }
            }
            else
            {
                words.Add(word);
            }
        }
        if(words.Count > 0)
        {
            var textSearch = string.Join(' ', words);
            if (textSearch is not null)
            {
                textSearch = textSearch.Trim();
            }
            if(string.IsNullOrWhiteSpace(textSearch))
            {
                return null;
            }
            return textSearch;
        }
        return null;
    }
}
