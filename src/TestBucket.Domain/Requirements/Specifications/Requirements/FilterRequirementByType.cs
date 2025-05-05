using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Domain.Requirements.Models;
using TestBucket.Domain.Shared.Specifications;

namespace TestBucket.Domain.Requirements.Specifications.Requirements
{
    public class FilterRequirementByType : FilterSpecification<Requirement>
    {
        private readonly string? _type;

        public FilterRequirementByType(string? type)
        {
            _type = type;
        }

        protected override Expression<Func<Requirement, bool>> GetExpression()
        {
            return x => x.RequirementType == _type;
        }
    }
}
