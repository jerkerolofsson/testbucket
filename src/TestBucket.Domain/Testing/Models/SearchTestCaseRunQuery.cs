using System.Net;
using System.Text;

namespace TestBucket.Domain.Testing.Models;
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

    public static SearchTestCaseRunQuery FromUrl(string? url)
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

        var pairs = query.Split('&');
        foreach(var pair in pairs)
        {
            var equalSignPos = pair.IndexOf('=');
            if(equalSignPos > 0 && pair.Length > equalSignPos+1)
            {
                var key = pair.Substring(0, equalSignPos);
                var value = WebUtility.UrlDecode(pair.Substring(equalSignPos+1));
                switch(key)
                {
                    case "AssignedToUser":
                        q.AssignedToUser = value;
                        break;
                    case "State":
                        q.State = value;
                        break;
                    case "Unassigned":
                        if (bool.TryParse(value, out var unassigned))
                        {
                            q.Unassigned = unassigned;
                        }
                        break;
                    case "Completed":
                        if (bool.TryParse(value, out var completed))
                        {
                            q.Completed = completed;
                        }
                        break;
                    case "Result":
                        if (Enum.TryParse<TestResult>(value, out var result))
                        {
                            q.Result = result;
                        }
                        break;
                }
            }
        }

        return q;
    }

    public string ToQueryString()
    {
        List<string> items = [];
        if (AssignedToUser is not null)
        {
            items.Add($"AssignedToUser={WebUtility.UrlEncode(AssignedToUser)}");
        }
        if (Unassigned is not null)
        {
            items.Add($"Unassigned={Unassigned}");
        }
        if (Result is not null)
        {
            items.Add($"Result={Result}");
        }
        if (State is not null)
        {
            items.Add($"State={WebUtility.UrlEncode(State)}");
        }
        if (TeamId is not null)
        {
            items.Add($"TeamId={TeamId}");
        }
        if (Completed is not null)
        {
            items.Add($"Completed={Completed}");
        }
        if (TestSuiteId is not null)
        {
            items.Add($"TestSuiteId={TestSuiteId}");
        }
        return string.Join('&', items);
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

        return sb.ToString();
    }
}
