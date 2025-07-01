using TestBucket.Domain.Requirements.Models;
using TestBucket.Domain.Search;

namespace TestBucket.Domain.Requirements.Search;
public class SearchRequirementQueryParser
{
    private static readonly HashSet<string> _keywords =
       [
        "is",
        "state",
        "assigned-to",
        "collection-id",
        "open",
        .. BaseQueryParser.Keywords
       ];

    public static SearchRequirementQuery Parse(string text, IReadOnlyList<FieldDefinition> fields, TimeProvider? provider = null)
    {
        var request = new SearchRequirementQuery();

        Dictionary<string, string> result = [];
        request.Text = SearchStringParser.Parse(text, result, request.Fields, _keywords, fields);
        BaseQueryParser.Parse(request, result, provider);

        foreach (var pair in result)
        {
            switch (pair.Key)
            {
                case "state":
                    request.RequirementState = pair.Value;
                    break;

                case "collection-id":
                    if (long.TryParse(pair.Value, out var id))
                    {
                        request.RequirementSpecificationId = id;
                    }
                    break;

                case "assigned-to":
                    request.AssignedTo = pair.Value;
                    break;
                case "open":
                    if (pair.Value == "yes" || pair.Value == "true")
                    {
                        request.IsOpen = true;
                    }
                    else if(pair.Value == "no" || pair.Value == "false")
                    {
                        request.IsOpen = false;
                    }
                    break;
                case "is":
                    request.RequirementType = pair.Value;
                    break;
            }
        }

        return request;
    }
}
