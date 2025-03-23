using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Domain.Requirements.Models;
using TestBucket.Domain.Shared.Specifications;

namespace TestBucket.Domain.Requirements.Specifications.Folders;
internal class FilterRequirementFoldersByParentId : FilterSpecification<RequirementSpecificationFolder>
{
    private readonly long? _id;

    public FilterRequirementFoldersByParentId(long? id)
    {
        _id = id;
    }

    protected override Expression<Func<RequirementSpecificationFolder, bool>> GetExpression()
    {
        return x => x.ParentId == _id;
    }
}
