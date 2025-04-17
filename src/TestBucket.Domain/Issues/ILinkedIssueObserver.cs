using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Domain.Issues.Models;
using TestBucket.Domain.Testing.Models;

namespace TestBucket.Domain.Issues
{
    public interface ILinkedIssueObserver
    {
        Task OnLinkedIssueAddedAsync(LinkedIssue issue);
        Task OnLinkedIssueDeletedAsync(LinkedIssue issue);

    }
}
