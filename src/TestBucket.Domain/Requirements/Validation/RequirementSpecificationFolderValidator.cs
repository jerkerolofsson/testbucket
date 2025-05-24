using FluentValidation;

using Microsoft.Extensions.Localization;

using TestBucket.Domain.Requirements.Models;

public class RequirementSpecificationFolderValidator : AbstractValidator<RequirementSpecificationFolder>
{
    public RequirementSpecificationFolderValidator()
    {
        RuleFor(x => x.Name).NotNull().WithMessage(x => "validation-folder-empty");
        RuleFor(x => x.Name).NotEmpty().WithMessage(x => "validation-folder-empty");
        RuleFor(x => x.Name).Must(x => !x.Contains('/')).WithMessage(x => "validation-folder-cannot-contain-slash");
    }
}