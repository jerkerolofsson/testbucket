using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Domain.Requirements.Models;
using TestBucket.Domain.Shared.Specifications;

namespace TestBucket.Domain.Requirements.Specifications
{
    public class FilterRequirementSpecificationById : FilterSpecification<RequirementSpecification>
    {
        private readonly long _id;

        public FilterRequirementSpecificationById(long id)
        {
            _id = id;
        }

        protected override Expression<Func<RequirementSpecification, bool>> GetExpression()
        {
            return x => x.Id == _id;
        }
    }
}
