using TestBucket.Contracts.Requirements;
using TestBucket.Domain.Requirements.Models;

namespace TestBucket.Domain.Requirements.Mapping;
internal static class RequirementMapper
{
    public static RequirementSpecificationDto ToDto(this RequirementSpecification specification)
    {
        return new RequirementSpecificationDto
        {
            Slug = specification.Slug,
            Name = specification.Name,
            Description = specification.Description,
            ExternalId = specification.ExternalId,
            Icon = specification.Icon,
            ReadOnly = specification.ReadOnly,
            Color = specification.Color,
        };
    }

    public static RequirementDto ToDto(this Requirement requirement)
    {
        return new RequirementDto
        {
            Name = requirement.Name,
            Description = requirement.Description,
            ExternalId = requirement.ExternalId,
            Path = requirement.Path,
            ReadOnly = requirement.ReadOnly,
            State = requirement.State,
            Slug = requirement.Slug,
            Provider = requirement.ExternalProvider,
            RequirementType = requirement.RequirementType,
            MappedType = requirement.MappedType,
            MappedState = requirement.MappedState,
        };
    }
}
