using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Domain.Issues.Models;
using TestBucket.Domain.Shared.Specifications;

namespace TestBucket.Domain.Issues.Specifications;
internal class FindLinkedIssueByTestCaseRunId : FilterSpecification<LinkedIssue>
{
    private readonly long _id;

    public FindLinkedIssueByTestCaseRunId(long testCaseRunId)
    {
        _id = testCaseRunId;
    }

    protected override Expression<Func<LinkedIssue, bool>> GetExpression()
    {
        return x => x.TestCaseRunId == _id;
    }
}
