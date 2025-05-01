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
    public class FilterRequirementSpecificationBySlug : FilterSpecification<RequirementSpecification>
    {
        private readonly string _slug;

        public FilterRequirementSpecificationBySlug(string slug)
        {
            _slug = slug;
        }

        protected override Expression<Func<RequirementSpecification, bool>> GetExpression()
        {
            return x => x.Slug == _slug;
        }
    }
}
