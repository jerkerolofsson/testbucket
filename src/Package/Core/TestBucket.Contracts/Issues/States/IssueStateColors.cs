using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestBucket.Contracts.Issues.States;
public class IssueStateColors
{
    public static string? GetDefaultBackgroundColor(MappedIssueState? state)
    {
        if(state is null)
        {
            return "#888";
        }
        return state switch
        {
            MappedIssueState.Open => "greenyellow",
            MappedIssueState.Closed => "purple",
            _ => "#888"
        };
    }
    public static string? GetDefaultTextColor(MappedIssueState? state)
    {
        if (state is null)
        {
            return "#000";
        }
        return state switch
        {
            MappedIssueState.Open => "#000",
            MappedIssueState.Closed => "#fff",
            _ => "#000"
        };
    }
}
