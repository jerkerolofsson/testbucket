using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Domain.Shared.Specifications;
using TestBucket.Domain.Testing.Models;
using TestBucket.Domain.Testing.Specifications;

namespace TestBucket.Domain.Fields.Specifications
{
    public class TestCaseFilterSpecificationBuilder
    {
        public static IReadOnlyList<FilterSpecification<TestCase>> From(SearchTestQuery query)
        {
            var specifications = new List<FilterSpecification<TestCase>>();

            if (query.TeamId is not null)
            {
                specifications.Add(new FilterByTeam<TestCase>(query.TeamId.Value));
            }
            if (query.ProjectId is not null)
            {
                specifications.Add(new FilterByProject<TestCase>(query.ProjectId.Value));
            }
            if (query.TestSuiteId is not null)
            {
                specifications.Add(new FilterTestCasesByTestSuite(query.TestSuiteId.Value));
            }
            if (query.Text is not null)
            {
                specifications.Add(new FilterTestCasesByText(query.Text));
            }
            if (query.CompareFolder)
            {
                specifications.Add(new FilterTestCasesByTestSuiteFolder(query.FolderId, query.Recurse));
            }

            return specifications;
        }
    }
}
