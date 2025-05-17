using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Domain.Testing.Models;

namespace TestBucket.Domain.Testing.TestCases.Search;
public class SearchTestCaseQueryParser
{
    private static readonly HashSet<string> _keywords = 
        [
        "is", 
        "team-id", 
        "testsuite-id", 
        "project-id", 
        "testrun-id", 
        ];

    public static SearchTestQuery Parse(string text, IReadOnlyList<FieldDefinition> fields)
    {
        var request = new SearchTestQuery();

        Dictionary<string, string> result = [];
        request.Text = SearchStringParser.Parse(text, result, request.Fields, _keywords, fields);
        foreach (var pair in result)
        {
            switch (pair.Key)
            {
                case "team-id":
                    if (long.TryParse(pair.Value, out var teamId))
                    {
                        request.TeamId = teamId;
                    }
                    break;
                case "testsuite-id":
                    if (long.TryParse(pair.Value, out var testSuiteId))
                    {
                        request.TestSuiteId = testSuiteId;
                    }
                    break;
                case "testrun-id":
                    if (long.TryParse(pair.Value, out var testRunId))
                    {
                        request.TestRunId = testRunId;
                    }
                    break;
                case "is":
                    if (pair.Value == "manual")
                    {
                        request.TestExecutionType = TestExecutionType.Manual;
                    }
                    else if (pair.Value == "automated")
                    {
                        request.TestExecutionType = TestExecutionType.Automated;
                    }
                    else if (pair.Value == "hybrid")
                    {
                        request.TestExecutionType = TestExecutionType.Hybrid;
                    }
                    else if (pair.Value == "hybrid-auto")
                    {
                        request.TestExecutionType = TestExecutionType.HybridAutomated;
                    }
                    break;
            }
        }

        return request;
    }
}
