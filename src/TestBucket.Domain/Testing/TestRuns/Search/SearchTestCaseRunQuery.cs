using System.Net;
using System.Text;
using System.Web;

using TestBucket.Domain.Search;

namespace TestBucket.Domain.Testing.TestRuns.Search;

/// <summary>
/// Represents a search query for test case runs, supporting filtering by test run, test case, suite, completion, assignment, result, and state.
/// </summary>
public class SearchTestCaseRunQuery : SearchQuery
{
    /// <summary>
    /// Gets or sets the test run ID to filter by.
    /// </summary>
    public long? TestRunId { get; set; }

    /// <summary>
    /// Gets or sets the test case ID to filter by.
    /// </summary>
    public long? TestCaseId { get; set; }

    /// <summary>
    /// Gets or sets the test suite ID to filter by.
    /// </summary>
    public long? TestSuiteId { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to filter for completed tests.
    /// </summary>
    public bool? Completed { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to filter for unassigned test case runs.
    /// </summary>
    public bool? Unassigned { get; set; }

    /// <summary>
    /// Gets or sets the user to whom the test case run is assigned.
    /// </summary>
    public string? AssignedToUser { get; set; }

    /// <summary>
    /// Gets or sets the test result to filter by.
    /// </summary>
    public TestResult? Result { get; set; }

    /// <summary>
    /// Gets or sets the state to filter by.
    /// </summary>
    public string? State { get; set; }

    /// <summary>
    /// Creates a <see cref="SearchTestCaseRunQuery"/> from a URL query string.
    /// </summary>
    /// <param name="fields">The list of field definitions for parsing custom fields.</param>
    /// <param name="url">The URL containing the query string.</param>
    /// <returns>A populated <see cref="SearchTestCaseRunQuery"/> instance.</returns>
    public static SearchTestCaseRunQuery FromUrl(IReadOnlyList<FieldDefinition> fields, string? url)
    {
        var q = new SearchTestCaseRunQuery();
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
                return SearchTestCaseRunQueryParser.Parse(items[key] ?? "", fields);
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
        if (AssignedToUser is not null)
        {
            items.Add($"assigned-to:{AssignedToUser}");
        }
        if (Unassigned is not null)
        {
            if (Unassigned == true)
            {
                items.Add("unassigned:yes");
            }
            else
            {
                items.Add("unassigned:no");
            }
        }
        if (Result is not null)
        {
            items.Add($"result:{Result.ToString()!.ToLower()}");
        }
        if (State is not null)
        {
            items.Add($"state:{State}");
        }
        if (TestRunId is not null)
        {
            items.Add($"testrun-id:{TestRunId}");
        }
        if (Completed is not null)
        {
            if (Completed == true)
            {
                items.Add("completed:yes");
            }
            else
            {
                items.Add("completed:no");
            }
        }
        if (TestSuiteId is not null)
        {
            items.Add($"testsuite-id:{TestSuiteId}");
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

    /// <summary>
    /// Generates a cache key string based on the current query parameters.
    /// </summary>
    /// <returns>A string suitable for use as a cache key.</returns>
    public string AsCacheKey()
    {
        var sb = new StringBuilder();
        if (TeamId is not null)
        {
            sb.Append("t=");
            sb.Append(TeamId);
        }
        if (Completed is not null)
        {
            sb.Append("Completed=");
            sb.Append(Completed);
        }
        if (Unassigned is not null)
        {
            sb.Append("Unassigned=");
            sb.Append(Unassigned);
        }
        if (AssignedToUser is not null)
        {
            sb.Append("AssignedToUser=");
            sb.Append(AssignedToUser);
        }
        if (CreatedFrom is not null)
        {
            sb.Append("from=");
            sb.Append(CreatedFrom);
        }
        if (CreatedUntil is not null)
        {
            sb.Append("until=");
            sb.Append(CreatedUntil);
        }
        if (Text is not null)
        {
            sb.Append("x=");
            sb.Append(Text);
        }
        if (ProjectId is not null)
        {
            sb.Append("p=");
            sb.Append(ProjectId);
        }
        if (TestRunId is not null)
        {
            sb.Append("TestRunId=");
            sb.Append(TestRunId);
        }
        if (TestSuiteId is not null)
        {
            sb.Append("TestSuiteId=");
            sb.Append(TestSuiteId);
        }
        if (Result is not null)
        {
            sb.Append("Result=");
            sb.Append(Result);
        }
        if (State is not null)
        {
            sb.Append("State=");
            sb.Append(State);
        }
        sb.Append("offset=");
        sb.Append(Offset);
        sb.Append("count=");
        sb.Append(Count);

        foreach (var field in Fields)
        {
            sb.Append("F=");
            sb.Append(field.FilterDefinitionId);
            sb.Append(field.StringValue);
            sb.Append(field.BooleanValue);
        }

        return sb.ToString();
    }
}