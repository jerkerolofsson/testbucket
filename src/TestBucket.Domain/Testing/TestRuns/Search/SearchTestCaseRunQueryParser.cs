using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Domain.Issues.Search;
using TestBucket.Domain.Search;

namespace TestBucket.Domain.Testing.TestRuns.Search;
public class SearchTestCaseRunQueryParser
{
    private static readonly HashSet<string> _keywords = 
        [
        "id",
        "assigned-to", 
        "testsuite-id", 
        "testrun-id", 
        "state", 
        "completed", 
        "result", 
        "unassigned",
        .. BaseQueryParser.Keywords
        ];

    public static SearchTestCaseRunQuery Parse(string text, IReadOnlyList<FieldDefinition> fields, TimeProvider? provider = null)
    {
        var request = new SearchTestCaseRunQuery();

        Dictionary<string, string> result = [];
        request.Text = SearchStringParser.Parse(text, result, request.Fields, _keywords, fields);
        BaseQueryParser.Parse(request, result, provider);

        foreach (var pair in result)
        {
            switch (pair.Key)
            {
                case "id":
                    request.ExternalDisplayId = pair.Value;
                    break;
                case "assigned-to":
                    request.AssignedToUser = pair.Value;
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
                case "completed":
                    request.Completed = pair.Value == "yes" ? true : false;
                    break;
                case "unassigned":
                    request.Unassigned = pair.Value == "yes" ? true : false;
                    break;
                case "result":
                    request.Result = pair.Value switch 
                    {
                        "passed" => TestResult.Passed,
                        "failed" => TestResult.Failed,
                        "blocked" => TestResult.Blocked,
                        "inconclusive" => TestResult.Inconclusive,
                        "crashed" => TestResult.Crashed,
                        "hang" => TestResult.Hang,
                        "assert" => TestResult.Assert,
                        "error" => TestResult.Error,
                        _ => TestResult.Other
                    };
                    break;
                case "state":
                    request.State = pair.Value;
                    break;
            }
        }

        return request;
    }
}
