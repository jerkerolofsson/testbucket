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
    public class FilterRequirementByAssignedTo : FilterSpecification<Requirement>
    {
        private readonly string _assignedTo;

        public FilterRequirementByAssignedTo(string assignedTo)
        {
            _assignedTo = assignedTo;
        }

        protected override Expression<Func<Requirement, bool>> GetExpression()
        {
            return x => x.AssignedTo == _assignedTo;
        }
    }
}
