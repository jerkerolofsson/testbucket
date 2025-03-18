using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Domain.Shared.Specifications;

namespace TestBucket.Domain.Fields.Specifications
{
    public class FieldSpecificationBuilder
    {
        public static IReadOnlyList<FilterSpecification<FieldDefinition>> From(SearchFieldQuery query)
        {
            var specifications = new List<FilterSpecification<FieldDefinition>>();

            if (query.TeamId is not null)
            {
                specifications.Add(new FilterByTeam<FieldDefinition>(query.TeamId.Value));
            }
            if (query.ProjectId is not null)
            {
                specifications.Add(new FilterByProject<FieldDefinition>(query.ProjectId.Value));
            }
            if (query.Target is not null)
            {
                specifications.Add(new FilterFieldDefinitionByTarget(query.Target.Value));
            }

            return specifications;
        }
    }
}
