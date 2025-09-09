using FluentValidation;

using TestBucket.Domain.Requirements.Models;

namespace TestBucket.Domain.Requirements.Models;
public class RequirementSpecificationValidator : AbstractValidator<RequirementSpecification>
{
    public RequirementSpecificationValidator()
    {
        RuleFor(x => x.Name).NotNull().WithMessage(x => "validation-name-empty");
        RuleFor(x => x.Name).NotEmpty().WithMessage(x => "validation-name-empty");
    }
}