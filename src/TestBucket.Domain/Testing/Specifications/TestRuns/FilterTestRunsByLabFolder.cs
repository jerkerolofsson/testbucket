using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Domain.Shared.Specifications;
using TestBucket.Domain.Testing.Models;

namespace TestBucket.Domain.Testing.Specifications.TestRuns;
internal class FilterTestRunsByLabFolder : FilterSpecification<TestRun>
{
    private readonly long _folderId;

    public FilterTestRunsByLabFolder(long folderId)
    {
        _folderId = folderId;
    }

    protected override Expression<Func<TestRun, bool>> GetExpression()
    {
        return x => x.FolderId == _folderId;
    }
}
