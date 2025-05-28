using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Domain.Issues.Models;

namespace TestBucket.Domain.Issues
{
    public interface ILocalIssueObserver
    {
        Task OnLocalIssueAddedAsync(LocalIssue issue);
        Task OnLocalIssueUpdatedAsync(LocalIssue issue);
        Task OnLocalIssueFieldChangedAsync(LocalIssue issue);
        Task OnLocalIssueDeletedAsync(LocalIssue issue);

    }
}
