using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Domain.Requirements.Models;
using TestBucket.Domain.Requirements.Specifications.Folders;
using TestBucket.Domain.Shared.Specifications;

namespace TestBucket.Domain.Requirements.Specifications
{
    public class RequirementSpecificationBuilder
    {
        public static IReadOnlyList<FilterSpecification<RequirementSpecificationFolder>> From(SearchRequirementFolderQuery query)
        {
            var specifications = new List<FilterSpecification<RequirementSpecificationFolder>>();

            if (!string.IsNullOrWhiteSpace(query.Name))
            {
                specifications.Add(new FilterRequirementFoldersByName(query.Name));
            }
            if (!string.IsNullOrWhiteSpace(query.Text))
            {
                specifications.Add(new FilterRequirementFoldersByText(query.Text));
            }
            if (query.RequirementSpecificationId is not null)
            {
                specifications.Add(new FilterRequirementFoldersBySpecification(query.RequirementSpecificationId.Value));
            }
            if (query.TeamId is not null)
            {
                specifications.Add(new FilterByTeam<RequirementSpecificationFolder>(query.TeamId.Value));
            }
            if (query.ProjectId is not null)
            {
                specifications.Add(new FilterByProject<RequirementSpecificationFolder>(query.ProjectId.Value));
            }
            if (query.CompareFolder)
            {
                specifications.Add(new FilterRequirementFoldersByParentId(query.FolderId));
            }

            return specifications;
        }
        public static IReadOnlyList<FilterSpecification<Requirement>> From(SearchRequirementQuery query)
        {
            var specifications = new List<FilterSpecification<Requirement>>();

            if (!string.IsNullOrWhiteSpace(query.Text))
            {
                specifications.Add(new FilterRequirementByText(query.Text));
            }
            if (query.RequirementSpecificationId is not null)
            {
                specifications.Add(new FilterRequirementBySpecification(query.RequirementSpecificationId.Value));
            }
            if (query.TeamId is not null)
            {
                specifications.Add(new FilterByTeam<Requirement>(query.TeamId.Value));
            }
            if (query.ProjectId is not null)
            {
                specifications.Add(new FilterByProject<Requirement>(query.ProjectId.Value));
            }
            if (query.CompareFolder)
            {
                specifications.Add(new FilterRequirementByParentFolder(query.FolderId));
            }

            return specifications;
        }
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
