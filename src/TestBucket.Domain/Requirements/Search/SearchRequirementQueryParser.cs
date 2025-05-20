using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Domain.Requirements.Models;
using TestBucket.Domain.Search;
using TestBucket.Domain.Testing.TestRuns.Search;

namespace TestBucket.Domain.Requirements.Search;
public class SearchRequirementQueryParser
{
    private static readonly HashSet<string> _keywords =
       [
        "is",
        "state",
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
               
                case "is":
                    request.RequirementType = pair.Value;
                    break;
            }
        }

        return request;
    }
}
