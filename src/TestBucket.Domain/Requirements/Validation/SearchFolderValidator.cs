using FluentValidation;

public class SearchFolderValidator : AbstractValidator<SearchFolder>
{
    public SearchFolderValidator()
    {
        RuleFor(x => x.Name).NotNull().WithMessage(x => "validation-name-empty");
        RuleFor(x => x.Name).NotEmpty().WithMessage(x => "validation-name-empty");
        RuleFor(x => x.Query).NotNull().WithMessage(x => "validation-query-empty");
        RuleFor(x => x.Query).NotEmpty().WithMessage(x => "validation-query-empty");
    }
}