using System.Linq.Expressions;

using TestBucket.Domain.Requirements.Models;
using TestBucket.Domain.Shared.Specifications;

namespace TestBucket.Domain.Requirements.Specifications.Requirements
{
    public class FilterRequirementBySpecification : FilterSpecification<Requirement>
    {
        private readonly long _requirementSpecificationId;

        public FilterRequirementBySpecification(long requirementSpecificationId)
        {
            _requirementSpecificationId = requirementSpecificationId;
        }

        protected override Expression<Func<Requirement, bool>> GetExpression()
        {
            return x => x.RequirementSpecificationId == _requirementSpecificationId;
        }
    }
}
