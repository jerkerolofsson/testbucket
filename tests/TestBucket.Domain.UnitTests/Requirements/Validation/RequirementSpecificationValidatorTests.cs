using FluentValidation.TestHelper;
using TestBucket.Domain.Requirements.Models;

namespace TestBucket.Domain.UnitTests.Requirements.Validation;

/// <summary>
/// Unit tests for <see cref="RequirementSpecificationValidator"/>.
/// Validates the rules for the <c>Name</c> property of <see cref="RequirementSpecification"/>.
/// </summary>
[EnrichedTest]
[UnitTest]
[FunctionalTest]
[Component("Requirements")]
public class RequirementSpecificationValidatorTests
{
    private readonly RequirementSpecificationValidator _validator = new();

    /// <summary>
    /// Verifies that a validation error is produced when <c>Name</c> is <c>null</c>.
    /// </summary>
    [Fact]
    public void Should_Have_Error_When_Name_Is_Null()
    {
        var model = new RequirementSpecification { Name = null! };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Name)
            .WithErrorMessage("validation-name-empty");
    }

    /// <summary>
    /// Verifies that a validation error is produced when <c>Name</c> is an empty string.
    /// </summary>
    [Fact]
    public void Should_Have_Error_When_Name_Is_Empty()
    {
        var model = new RequirementSpecification { Name = "" };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Name)
            .WithErrorMessage("validation-name-empty");
    }

    /// <summary>
    /// Verifies that no validation error is produced when <c>Name</c> is a valid non-empty string.
    /// </summary>
    [Fact]
    public void Should_Not_Have_Error_When_Name_Is_Valid()
    {
        var model = new RequirementSpecification { Name = "Valid Name" };
        var result = _validator.TestValidate(model);
        result.ShouldNotHaveValidationErrorFor(x => x.Name);
    }
}