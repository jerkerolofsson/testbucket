using TestBucket.Contracts.Requirements;
using TestBucket.Domain.Export.Handlers;
using TestBucket.Domain.Requirements.Models;

namespace TestBucket.Domain.Requirements.Mapping;
public static class RequirementMapper
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
    public static RequirementSpecification ToDbo(this RequirementSpecificationDto specification)
    {
        return new RequirementSpecification
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

    public static Requirement ToDbo(this RequirementDto requirement)
    {
        return new Requirement
        {
            Name = requirement.Name,
            Description = requirement.Description,
            ExternalId = requirement.ExternalId,
            ExternalProvider = requirement.Provider,
            Path = requirement.Path ?? "",
            ReadOnly = requirement.ReadOnly,
            State = requirement.State,
            Slug = requirement.Slug,
            RequirementType = requirement.RequirementType,
            MappedType = requirement.MappedType,
            MappedState = requirement.MappedState,
            StartDate = requirement.StartDate,
            DueDate = requirement.DueDate,
            Progress = requirement.Progress,
            Comments = CommentSerializer.Deserialize(requirement.Comments),
        };
    }
    public static RequirementDto ToDto(this Requirement requirement)
    {
        return new RequirementDto
        {
            Name = requirement.Name,
            Description = requirement.Description,
            Provider = requirement.ExternalProvider,
            ExternalId = requirement.ExternalId,
            Path = requirement.Path,
            ReadOnly = requirement.ReadOnly,
            Slug = requirement.Slug,
            Comments = CommentSerializer.Serialize(requirement.Comments),

            RequirementType = requirement.RequirementType,
            MappedType = requirement.MappedType,

            State = requirement.State,
            MappedState = requirement.MappedState,

            StartDate = requirement.StartDate,
            DueDate = requirement.DueDate,
            Progress = requirement.Progress,
        };
    }
}
