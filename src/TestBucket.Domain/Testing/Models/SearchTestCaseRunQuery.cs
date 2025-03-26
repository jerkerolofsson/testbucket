using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestBucket.Domain.Testing.Models;
public class SearchTestCaseRunQuery : SearchQuery
{
    public long? TestRunId { get; set; }
    public long? TestSuiteId { get; set; }

    public TestResult? Result { get; set; }
    public string? State { get; set; }

    public string AsCacheKey()
    {
        var sb = new StringBuilder();
        if (TeamId is not null)
        {
            sb.Append("t=");
            sb.Append(TeamId);
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
