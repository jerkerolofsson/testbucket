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
    public class FilterRequirementBySlug : FilterSpecification<Requirement>
    {
        private readonly string _slug;

        public FilterRequirementBySlug(string slug)
        {
            _slug = slug;
        }

        protected override Expression<Func<Requirement, bool>> GetExpression()
        {
            return x => x.Slug == _slug;
        }
    }
}
