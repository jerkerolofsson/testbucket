using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestBucket.Jira.Models;

internal class JiraPagedIssuesResponse<T>
{
    public string? nextPageToken { get; set; }
    public bool isLast { get; set; }
    public T[]? issues { get; set; }
}
