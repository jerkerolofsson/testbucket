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
    public class FilterRequirementByState : FilterSpecification<Requirement>
    {
        private readonly string? _state;

        public FilterRequirementByState(string? state)
        {
            _state = state;
        }

        protected override Expression<Func<Requirement, bool>> GetExpression()
        {
            return x => x.State == _state;
        }
    }
}
