using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Domain.Issues.Models;

namespace TestBucket.Domain.Issues;
public interface IIssueRepository
{
    Task AddLinkedIssueAsync(LinkedIssue linkedIssue);
    Task DeleteLinkedIssueAsync(long linkedIssueId);
    Task<IReadOnlyList<LinkedIssue>> GetLinkedIssuesAsync(long testCaseRun);
    Task UpdateLinkedIssueAsync(LinkedIssue linkedIssue);
}
