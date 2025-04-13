using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Domain.Requirements.Models;
using TestBucket.Domain.Shared.Specifications;

namespace TestBucket.Domain.Requirements.Specifications.Folders
{
    public class FilterRequirementFoldersByText : FilterSpecification<RequirementSpecificationFolder>
    {
        private readonly string _text;

        public FilterRequirementFoldersByText(string text)
        {
            _text = text.ToLower();
        }

        protected override Expression<Func<RequirementSpecificationFolder, bool>> GetExpression()
        {
            return x => x.Name.ToLower().Contains(_text) || (x.Description != null && x.Description.ToLower().Contains(_text));
        }
    }
}
