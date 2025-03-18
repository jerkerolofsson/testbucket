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
    public class FilterRequirementByText : FilterSpecification<Requirement>
    {
        private readonly string _text;

        public FilterRequirementByText(string text)
        {
            _text = text.ToLower();
        }

        protected override Expression<Func<Requirement, bool>> GetExpression()
        {
            return x => x.Name.ToLower().Contains(_text) || (x.Description != null && x.Description.ToLower().Contains(_text));
        }
    }
}
