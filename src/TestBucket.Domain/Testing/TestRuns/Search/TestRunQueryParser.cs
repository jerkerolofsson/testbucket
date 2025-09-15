using TestBucket.Domain.Search;

namespace TestBucket.Domain.Testing.TestRuns.Search;
public class TestRunQueryParser
{
    private static readonly HashSet<string> _keywords = 
        [
        "testrun-id",
        "folder-id",
        "archived",
        "open",
        .. BaseQueryParser.Keywords
        ];

    public static SearchTestRunQuery Parse(string text, IReadOnlyList<FieldDefinition> fields, TimeProvider? provider = null)
    {
        var request = new SearchTestRunQuery();

        Dictionary<string, string> result = [];
        request.Text = SearchStringParser.Parse(text, result, request.Fields, _keywords, fields);
        BaseQueryParser.Parse(request, result, provider);

        foreach (var pair in result)
        {
            switch (pair.Key)
            {
                case "archived":
                    request.Archived = pair.Value == "yes" || pair.Value == "true";
                    break;
                case "open":
                    request.Open = pair.Value == "yes" || pair.Value == "true";
                    break;
                case "folder-id":
                    if (long.TryParse(pair.Value, out var folderId))
                    {
                        request.FolderId = folderId;
                    }
                    break;
                case "testrun-id":
                    if (long.TryParse(pair.Value, out var testRunId))
                    {
                        request.TestRunId = testRunId;
                    }
                    break;
            }
        }

        return request;
    }
}
