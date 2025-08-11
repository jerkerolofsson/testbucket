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
    public class FilterRequirementByText : FilterSpecification<Requirement>
    {
        internal string Text { get; private set; }

        public FilterRequirementByText(string text)
        {
            Text = text.ToLower();
        }

        protected override Expression<Func<Requirement, bool>> GetExpression()
        {
            return x => x.Name.ToLower().Contains(Text) || 
            (x.Description != null && x.Description.ToLower().Contains(Text)) ||
            (x.ExternalId != null && x.ExternalId.ToLower().Contains(Text));
        }
    }
}
