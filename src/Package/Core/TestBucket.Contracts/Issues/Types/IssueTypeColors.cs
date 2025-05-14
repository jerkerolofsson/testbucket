using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestBucket.Contracts.Issues.Types;
public class IssueTypeColors
{
    public static string? GetDefault(MappedIssueType type)
    {
        return type switch
        {
            MappedIssueType.Enhancement => "cyan",
            MappedIssueType.Incident => "coral",
            MappedIssueType.Issue => "lightyellow",
            _ => "#888"
        };
    }
}
