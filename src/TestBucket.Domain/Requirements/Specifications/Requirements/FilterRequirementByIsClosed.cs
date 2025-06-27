using System.Linq.Expressions;

using TestBucket.Domain.Requirements.Models;
using TestBucket.Domain.Shared.Specifications;

namespace TestBucket.Domain.Requirements.Specifications.Requirements
{
    public class FilterRequirementByIsClosed : FilterSpecification<Requirement>
    {
        public FilterRequirementByIsClosed()
        {
        }

        protected override Expression<Func<Requirement, bool>> GetExpression()
        {
            return x => x.MappedState == Contracts.Requirements.States.MappedRequirementState.Completed ||
            x.MappedState == Contracts.Requirements.States.MappedRequirementState.Canceled;
        }
    }
}
