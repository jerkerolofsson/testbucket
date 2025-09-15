using TestBucket.Domain.Shared.Specifications;
using TestBucket.Domain.Testing.Models;
using TestBucket.Domain.Testing.Specifications.TestCases;
using TestBucket.Domain.Testing.TestRuns.Search;

namespace TestBucket.Domain.Testing.Specifications.TestRuns;

public class TestRunFilterSpecificationBuilder
{
    public static List<FilterSpecification<TestRun>> From(SearchTestRunQuery query)
    {
        var specifications = ProjectEntityFilterSpecificationBuilder.From<TestRun>(query);


        if (query.Fields is not null)
        {
            foreach (var fieldFilter in query.Fields)
            {
                if (!string.IsNullOrEmpty(fieldFilter.StringValue))
                {
                    specifications.Add(new FilterTestRunsByStringField(fieldFilter.FilterDefinitionId, fieldFilter.StringValue));
                }
                if (fieldFilter.BooleanValue is not null)
                {
                    specifications.Add(new FilterTestRunsByBooleanField(fieldFilter.FilterDefinitionId, fieldFilter.BooleanValue.Value));
                }
            }
        }

        if (query.FolderId is not null)
        {
            specifications.Add(new FilterTestRunsByLabFolder(query.FolderId.Value));
        }
        if (query.TestRunId is not null)
        {
            specifications.Add(new FilterTestRunsById(query.TestRunId.Value));
        }
        if (query.Archived is not null)
        {
            if (query.Archived == true)
            {
                specifications.Add(new OnlyArchivedTestRuns());
            }
            else if (query.Archived == false)
            {
                specifications.Add(new ExcludeArchivedTestRuns());
            }
        }
        if (query.Open is not null)
        {
            if (query.Open == true)
            {
                specifications.Add(new OnlyOpenTestRuns());
            }
            else if (query.Open == false)
            {
                specifications.Add(new OnlyClosedTestRuns());
            }
        }

        return specifications;
    }
}
