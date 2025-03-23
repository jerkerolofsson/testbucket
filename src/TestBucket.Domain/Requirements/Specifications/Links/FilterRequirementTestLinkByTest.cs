using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Domain.Requirements.Models;
using TestBucket.Domain.Shared.Specifications;

namespace TestBucket.Domain.Requirements.Specifications.Links;
public class FilterRequirementTestLinkByTest : FilterSpecification<RequirementTestLink>
{
    private readonly long _id;

    public FilterRequirementTestLinkByTest(long id)
    {
        _id = id;
    }

    protected override Expression<Func<RequirementTestLink, bool>> GetExpression()
    {
        return x => x.TestCaseId == _id;
    }
}
