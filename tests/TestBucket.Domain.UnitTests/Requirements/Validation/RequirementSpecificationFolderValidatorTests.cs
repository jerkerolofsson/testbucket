using FluentValidation.TestHelper;
using TestBucket.Domain.Requirements.Models;

namespace TestBucket.Domain.UnitTests.Requirements.Validation;

/// <summary>
/// Unit tests for <see cref="RequirementSpecificationFolderValidator"/>.
/// Validates the rules for the <c>Name</c> property of <see cref="RequirementSpecificationFolder"/>.
/// </summary>
[EnrichedTest]
[UnitTest]
[FunctionalTest]
public class RequirementSpecificationFolderValidatorTests
{
    private readonly RequirementSpecificationFolderValidator _validator = new();

    /// <summary>
    /// Verifies that a validation error is produced when <c>Name</c> is an empty string.
    /// </summary>
    [Fact]
    public void Should_Have_Error_When_Name_Is_Empty()
    {
        var model = new RequirementSpecificationFolder { Name = "" };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Name)
            .WithErrorMessage("validation-folder-empty");
    }

    /// <summary>
    /// Verifies that a validation error is produced when <c>Name</c> contains a slash ('/').
    /// </summary>
    [Fact]
    public void Should_Have_Error_When_Name_Contains_Slash()
    {
        var model = new RequirementSpecificationFolder { Name = "Folder/Name" };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Name)
            .WithErrorMessage("validation-folder-cannot-contain-slash");
    }

    /// <summary>
    /// Verifies that no validation error is produced when <c>Name</c> is a valid non-empty string without slashes.
    /// </summary>
    [Fact]
    public void Should_Not_Have_Error_When_Name_Is_Valid()
    {
        var model = new RequirementSpecificationFolder { Name = "ValidFolderName" };
        var result = _validator.TestValidate(model);
        result.ShouldNotHaveValidationErrorFor(x => x.Name);
    }
}