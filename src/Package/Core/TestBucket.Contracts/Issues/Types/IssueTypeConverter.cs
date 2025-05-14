using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestBucket.Contracts.Issues.Types;
public class IssueTypeConverter
{
    private static readonly Dictionary<string, MappedIssueType> _map = new Dictionary<string, MappedIssueType>()
    {
        [IssueTypes.Issue] = MappedIssueType.Issue,
        [IssueTypes.Enhancement] = MappedIssueType.Enhancement,
        [IssueTypes.Incident] = MappedIssueType.Incident,
        [IssueTypes.Question] = MappedIssueType.Question,
        [IssueTypes.Other] = MappedIssueType.Other,
    };

    public static MappedIssueType GetMappedIssueTypeFromString(string name)
    {
        if(_map.TryGetValue(name, out var result))
        {
            return result;
        }
        return MappedIssueType.Other;   
    }
}
