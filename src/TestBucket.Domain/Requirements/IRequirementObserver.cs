using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Domain.Requirements.Models;

namespace TestBucket.Domain.Requirements;
public interface IRequirementObserver
{
    Task OnSpecificationCreatedAsync(RequirementSpecification spec);
    Task OnSpecificationDeletedAsync(RequirementSpecification spec);
    Task OnSpecificationSavedAsync(RequirementSpecification spec);

    Task OnFolderCreatedAsync(RequirementSpecificationFolder folder);
    Task OnFolderDeletedAsync(RequirementSpecificationFolder folder);
    Task OnFolderSavedAsync(RequirementSpecificationFolder folder);

    Task OnRequirementCreatedAsync(Requirement requirement);
    Task OnRequirementDeletedAsync(Requirement requirement);
    Task OnRequirementSavedAsync(Requirement requirement);
}