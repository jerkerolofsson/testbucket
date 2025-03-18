using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Domain.Requirements.Models;
using TestBucket.Domain.Shared.Specifications;

namespace TestBucket.Domain.Requirements.Specifications
{
    public class RequirementSpecificationBuilder
    {
        public static IReadOnlyList<FilterSpecification<RequirementSpecification>> From(SearchQuery query)
        {
            var specifications = new List<FilterSpecification<RequirementSpecification>>();

            if (query.TeamId is not null)
            {
                specifications.Add(new FilterByTeam<RequirementSpecification>(query.TeamId.Value));
            }
            if (query.ProjectId is not null)
            {
                specifications.Add(new FilterByProject<RequirementSpecification>(query.ProjectId.Value));
            }

            return specifications;
        }
    }
}
