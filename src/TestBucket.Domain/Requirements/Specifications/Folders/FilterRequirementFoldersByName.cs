using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using TestBucket.Domain.Requirements.Models;

using TestBucket.Domain.Shared.Specifications;

namespace TestBucket.Domain.Requirements.Specifications.Folders;
internal class FilterRequirementFoldersByName : FilterSpecification<RequirementSpecificationFolder>
{
    private readonly string _name;

    public FilterRequirementFoldersByName(string name)
    {
        _name = name;
    }

    protected override Expression<Func<RequirementSpecificationFolder, bool>> GetExpression()
    {
        return x => x.Name == _name;
    }
}
