using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestBucket.Domain.Issues.Search;
public class SearchIssueRequestParser
{
    private static readonly HashSet<string> _keywords = ["is", "state", "origin"];

    public static SearchIssueRequest Parse(ClaimsPrincipal principal, long projectId, string text)
    {
        var request = new SearchIssueRequest(principal, projectId);

        Dictionary<string, string> result = [];
        request.Text = SearchStringParser.Parse(text, result, _keywords);
        foreach(var pair in result)
        {
            switch(pair.Key)
            {
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
