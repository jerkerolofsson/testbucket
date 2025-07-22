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
    public class FilterRequirementSpecificationByText : FilterSpecification<RequirementSpecification>
    {
        private readonly string _text;

        public FilterRequirementSpecificationByText(string text)
        {
            _text = text.ToLower();
        }

        protected override Expression<Func<RequirementSpecification, bool>> GetExpression()
        {
            return x => x.Name.ToLower().Contains(_text);
        }
    }
}
