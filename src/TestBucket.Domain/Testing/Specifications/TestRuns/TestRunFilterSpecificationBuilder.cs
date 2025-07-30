using TestBucket.Domain.Shared.Specifications;
using TestBucket.Domain.Testing.Models;
using TestBucket.Domain.Testing.Specifications.TestCases;

namespace TestBucket.Domain.Testing.Specifications.TestRuns;

public class TestRunFilterSpecificationBuilder
{
    public static List<FilterSpecification<TestRun>> From(SearchTestRunQuery query)
    {
        var specifications = ProjectEntityFilterSpecificationBuilder.From<TestRun>(query);

        if (query.FolderId is not null)
        {
            specifications.Add(new FilterTestRunsByLabFolder(query.FolderId.Value));
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

        return specifications;
    }
}
