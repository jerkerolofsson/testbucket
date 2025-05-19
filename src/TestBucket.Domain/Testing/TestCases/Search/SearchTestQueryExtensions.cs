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
            if (query.TeamId is not null)
            {
                items.Add($"team-id:{query.TeamId}");
            }
            if (query.ProjectId is not null)
            {
                items.Add($"project-id:{query.ProjectId}");
            }
            if (query.TestRunId is not null)
            {
                items.Add($"testrun-id:{query.TestRunId}");
            }
            if (query.TestSuiteId is not null)
            {
                items.Add($"testsuite-id:{query.TestSuiteId}");
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
