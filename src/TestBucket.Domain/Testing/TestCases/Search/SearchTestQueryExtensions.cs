using TestBucket.Domain.Search;
using TestBucket.Domain.Testing.Models;

namespace TestBucket.Domain.Testing.TestCases.Search
{
    public static class SearchTestQueryExtensions
    {
        public static string ToSearchText(this SearchTestQuery query)
        {
            List<string> items = [];

            if (query.TestExecutionType is not null)
            {
                var value = query.TestExecutionType.Value switch
                {
                    TestExecutionType.Automated => "automated",
                    TestExecutionType.Hybrid => "hybrid",
                    TestExecutionType.HybridAutomated => "hybrid-auto",
                    TestExecutionType.Manual => "manual",
                    _ => "manual"
                };
                items.Add($"is:{value}");
            }

            BaseQueryParser.Serialize(query, items);
            if (query.ExternalDisplayId is not null)
            {
                items.Add($"id:{query.ExternalDisplayId}");
            }
            if (query.ReviewAssignedTo is not null)
            {
                items.Add($"review-assigned-to:{query.ReviewAssignedTo}");
            }
            if (query.State is not null)
            {
                items.Add($"state:{query.State}");
            }
            if (query.RequirementId is not null)
            {
                items.Add($"requirement:{query.RequirementId.Value}");
            }
            if (query.TestRunId is not null)
            {
                items.Add($"testrun-id:{query.TestRunId}");
            }
            if (query.TestSuiteId is not null)
            {
                items.Add($"testsuite-id:{query.TestSuiteId}");
            }
            if (query.FolderId is not null)
            {
                items.Add($"folder-id:{query.FolderId}");
            }
            if (query.CompareFolder == true)
            {
                items.Add($"compare-folder:yes");
            }

            foreach (var field in query.Fields)
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

            return (string.Join(' ', items) + " " + query.Text).Trim();
        }
    }
}
