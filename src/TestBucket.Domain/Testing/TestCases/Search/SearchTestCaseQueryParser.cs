using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Domain.Search;
using TestBucket.Domain.Testing.Models;

namespace TestBucket.Domain.Testing.TestCases.Search;
public class SearchTestCaseQueryParser
{
    private static readonly HashSet<string> _keywords = 
        [
        "is", 
        "id",
        "testsuite-id", 
        "project-id", 
        "testrun-id", 
        .. BaseQueryParser.Keywords
        ];

    public static SearchTestQuery Parse(string text, IReadOnlyList<FieldDefinition> fields, TimeProvider? provider = null)
    {
        var request = new SearchTestQuery();

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
                case "compare-folder":
                    request.CompareFolder = pair.Value == "yes";
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
