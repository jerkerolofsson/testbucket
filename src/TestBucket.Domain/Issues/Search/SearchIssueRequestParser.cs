using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Domain.Search;

namespace TestBucket.Domain.Issues.Search;
public class SearchIssueRequestParser
{
    private static readonly HashSet<string> _keywords = [
        "is", 
        "state", 
        "origin",
        .. BaseQueryParser.Keywords
        ];

    public static SearchIssueQuery Parse(long? projectId, string text, IReadOnlyList<FieldDefinition> fields, TimeProvider? provider = null)
    {
        var request = new SearchIssueQuery
        {
            ProjectId = projectId
        };

        Dictionary<string, string> result = [];
        request.Text = SearchStringParser.Parse(text, result, request.Fields, _keywords, fields);
        BaseQueryParser.Parse(request, result, provider);
        foreach (var pair in result)
        {
            switch(pair.Key)
            {
                case "assigned-to":
                    request.AssignedToUser = pair.Value;
                    break;
                case "is":
                    request.Type = pair.Value;
                    break;
                case "origin":
                    request.ExternalSystemName = pair.Value;
                    break;
                case "state":
                    request.State = pair.Value;
                    break;
            }
        }

        return request;
    }
}
