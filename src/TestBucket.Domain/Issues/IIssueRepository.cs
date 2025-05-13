using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Domain.Issues.Models;
using TestBucket.Domain.Shared.Specifications;

namespace TestBucket.Domain.Issues;
public interface IIssueRepository
{
    Task AddLinkedIssueAsync(LinkedIssue linkedIssue);
    Task AddLocalIssueAsync(LocalIssue localIssue);
    Task DeleteLinkedIssueAsync(long linkedIssueId);
    Task DeleteLocalIssueAsync(long localIssueId);
    Task<IReadOnlyList<LinkedIssue>> GetLinkedIssuesAsync(long testCaseRun);
    Task<PagedResult<LinkedIssue>> SearchAsync(List<FilterSpecification<LinkedIssue>> filters, int count, int offset);
    Task<PagedResult<LocalIssue>> SearchAsync(List<FilterSpecification<LocalIssue>> filters, int offset, int count);
    Task UpdateLinkedIssueAsync(LinkedIssue linkedIssue);
    Task UpdateLocalIssueAsync(LocalIssue localIssue);
}
