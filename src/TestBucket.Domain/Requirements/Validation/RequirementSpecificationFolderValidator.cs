using FluentValidation;

using TestBucket.Domain.Requirements.Models;

namespace TestBucket.Domain.Requirements.Models;
public class RequirementSpecificationFolderValidator : AbstractValidator<RequirementSpecificationFolder>
{
    public RequirementSpecificationFolderValidator()
    {
        RuleFor(x => x.Name).NotNull().WithMessage(x => "validation-folder-empty");
        RuleFor(x => x.Name).NotNull().NotEmpty().WithMessage(x => "validation-folder-empty");
        RuleFor(x => x.Name).NotNull().Must(x => !x.Contains('/')).WithMessage(x => "validation-folder-cannot-contain-slash");
    }
}