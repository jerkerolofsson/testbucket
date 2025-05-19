using System.Net;
using System.Text;
using System.Web;

namespace TestBucket.Domain.Testing.TestRuns.Search;
public class SearchTestCaseRunQuery : SearchQuery
{
    public long? TestRunId { get; set; }
    public long? TestCaseId { get; set; }
    public long? TestSuiteId { get; set; }

    /// <summary>
    /// Completed tests
    /// </summary>
    public bool? Completed { get; set; }
    public bool? Unassigned { get; set; }
    public string? AssignedToUser { get; set; }
    public TestResult? Result { get; set; }
    public string? State { get; set; }

    public static SearchTestCaseRunQuery FromUrl(IReadOnlyList<FieldDefinition> fields, string? url)
    {
        var q = new SearchTestCaseRunQuery();
        if(url is null)
        {
            return q;
        }

        var p = url.IndexOf('?');
        var query = url;
        if (p > 0 && url.Length > p+2)
        {
            query = url[(p + 1)..];
        }
        var items = HttpUtility.ParseQueryString(query);

        foreach (var key in items.AllKeys)
        {
            if (key == "q")
            {
                return SearchTestCaseRunQueryParser.Parse(items[key]??"", fields);
            }
        }
        return q;
    }

    public string ToSearchText()
    {
        List<string> items = [];
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
        if (TeamId is not null)
        {
            items.Add($"team-id:{TeamId}");
        }
        if (ProjectId is not null)
        {
            items.Add($"project-id:{ProjectId}");
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

        foreach(var field in Fields)
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


    public string ToQueryString()
    {
        return "q=" + WebUtility.UrlEncode(ToSearchText());
    }
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

        foreach(var field in Fields)
        {
            sb.Append("F=");
            sb.Append(field.FilterDefinitionId);
            sb.Append(field.StringValue);
            sb.Append(field.BooleanValue);
        }

        return sb.ToString();
    }
}
