using System.Linq.Expressions;

using TestBucket.Domain.Requirements.Models;
using TestBucket.Domain.Shared.Specifications;

namespace TestBucket.Domain.Requirements.Specifications.Requirements
{
    public class FilterRequirementByType : FilterSpecification<Requirement>
    {
        private readonly string? _type;

        public FilterRequirementByType(string? type)
        {
            _type = type?.ToLower();
        }

        protected override Expression<Func<Requirement, bool>> GetExpression()
        {
            if(_type is null)
            {
                return x => x.RequirementType == null;
            }
            return x => x.RequirementType != null && x.RequirementType.ToLower() == _type;
        }
    }
}
