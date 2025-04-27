using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Domain.Shared.Specifications;
using TestBucket.Domain.Testing.Models;
using TestBucket.Domain.Testing.Specifications.TestCases;

namespace TestBucket.Domain.Testing.Specifications.TestCaseRuns;

public static class TestCaseRunsFilterSpecificationBuilder
{
    internal static List<FilterSpecification<TestCaseRun>> From(SearchTestCaseRunQuery query)
    {
        var specifications = ProjectEntityFilterSpecificationBuilder.From<TestCaseRun>(query);

        if (query.Unassigned == true)
        {
            specifications.Add(new FilterTestCaseRunsUnassigned());
        }
        else if (query.Unassigned == false)
        {
            specifications.Add(new FilterTestCaseRunsAssigned());
        }

        if (query.Completed == true)
        {
            specifications.Add(new FilterTestCaseRunsCompleted());
        }
        else if (query.Completed == false)
        {
            specifications.Add(new FilterTestCaseRunsIncomplete());
        }

        if (query.AssignedToUser is not null)
        {
            specifications.Add(new FilterTestCaseRunsByAssignment(query.AssignedToUser));
        }

        if (query.TestCaseId is not null)
        {
            specifications.Add(new FilterTestCaseRunsByTestCase(query.TestCaseId.Value));
        }
        if (query.TestSuiteId is not null)
        {
            specifications.Add(new FilterTestCaseRunsByTestSuite(query.TestSuiteId.Value));
        }
        if (query.Text is not null)
        {
            specifications.Add(new FilterTestCaseRunsByText(query.Text));
        }
        if (query.State is not null)
        {
            specifications.Add(new FilterTestCaseRunsByState(query.State));
        }
        if (query.Result is not null)
        {
            specifications.Add(new FilterTestCaseRunsByResult(query.Result.Value));
        }
        if (query.TestRunId is not null)
        {
            specifications.Add(new FilterTestCaseRunsByRun(query.TestRunId.Value));
        }

        return specifications;
    }
}
