using System.Net;
using System.Web;

using TestBucket.Domain.Search;

namespace TestBucket.Domain.Testing.TestRuns.Search;
public class SearchTestRunQuery : SearchQuery
{
    public long? TestRunId { get; set; }
    public long? FolderId { get; set; }
    public bool? Archived { get; set; }
    public bool? Open { get; set; }


    /// <summary>
    /// Creates a <see cref="SearchTestCaseRunQuery"/> from a URL query string.
    /// </summary>
    /// <param name="fields">The list of field definitions for parsing custom fields.</param>
    /// <param name="url">The URL containing the query string.</param>
    /// <returns>A populated <see cref="SearchTestCaseRunQuery"/> instance.</returns>
    public static SearchTestRunQuery FromUrl(IReadOnlyList<FieldDefinition> fields, string? url)
    {
        var q = new SearchTestRunQuery();
        if (url is null)
        {
            return q;
        }

        var p = url.IndexOf('?');
        var query = url;
        if (p > 0 && url.Length > p + 2)
        {
            query = url[(p + 1)..];
        }
        var items = HttpUtility.ParseQueryString(query);

        foreach (var key in items.AllKeys)
        {
            if (key == "q")
            {
                return TestRunQueryParser.Parse(items[key] ?? "", fields);
            }
        }
        return q;
    }

    /// <summary>
    /// Serializes the current query to a search text string.
    /// </summary>
    /// <returns>A string representing the search query.</returns>
    public string ToSearchText()
    {
        List<string> items = [];
        BaseQueryParser.Serialize(this, items);

        if (TestRunId is not null)
        {
            items.Add($"testrun-id:{TestRunId}");
        }
        if (FolderId is not null)
        {
            items.Add($"folder-id:{FolderId}");
        }
        if (Archived is not null)
        {
            var archivedString = Archived.Value ? "yes" : "no";
            items.Add($"archived:{archivedString}");
        }
        if (Open is not null)
        {
            var openString = Open.Value ? "yes" : "no";
            items.Add($"open:{openString}");
        }

        foreach (var field in Fields)
        {
            var name = field.Name.ToLower();
            var value = field.GetValueAsString();
            if (value.Contains(' '))
            {
                value = $"\"{value}\"";
            }
            if (name.Contains(' '))
            {
                name = $"\"{name}\"";
            }
            items.Add($"{name}:{value}");
        }

        return string.Join(' ', items) + " " + Text;
    }

    /// <summary>
    /// Serializes the current query to a URL query string.
    /// </summary>
    /// <returns>A URL-encoded query string representing the search query.</returns>
    public string ToQueryString()
    {
        return "q=" + WebUtility.UrlEncode(ToSearchText());
    }
}
