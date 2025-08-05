using FluentValidation;

using TestBucket.Domain.Testing.Models;

public class TestCaseValidator : AbstractValidator<TestCase>
{
    public TestCaseValidator()
    {
        RuleFor(x => x.Name).NotNull().WithMessage(x => "validation-name-empty");
        RuleFor(x => x.Name).NotEmpty().WithMessage(x => "validation-name-empty");

        RuleFor(x => x.TestSuiteId).NotNull().WithMessage(x => "validation-testsuite-required");
        RuleFor(x => x.TestSuiteId).NotEqual(0).WithMessage(x => "validation-testsuite-required");

        RuleFor(x => x.TestProjectId).NotNull().WithMessage(x => "validation-project-required");
        RuleFor(x => x.TestProjectId).NotEqual(0).WithMessage(x => "validation-project-required");
    }
}