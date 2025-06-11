using FluentValidation.TestHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestBucket.Domain.Metrics.Models;
using TestBucket.Domain.Metrics.Validation;

namespace TestBucket.Domain.UnitTests.Metrics
{
    /// <summary>
    /// Contains unit tests for the <see cref="MetricValidator"/> class, verifying validation rules for <see cref="Metric"/> models.
    /// </summary>
    [Component("Metrics")]
    [UnitTest]
    [EnrichedTest]
    public class MetricsValidatorTests
    {
        /// <summary>
        /// The validator instance under test.
        /// </summary>
        private readonly MetricValidator _validator = new();

        /// <summary>
        /// Verifies that a validation error is produced when the <c>Name</c> property is <c>null</c>.
        /// </summary>
        [Fact]
        public void Should_Have_Error_When_Name_Is_Null()
        {
            var model = new Metric { Name = null!, MeterName = "meter", Value = 1.0, Unit = "ms" };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Name)
                .WithErrorMessage("validation-name-empty");
        }

        /// <summary>
        /// Verifies that a validation error is produced when the <c>Name</c> property is an empty string.
        /// </summary>
        [Fact]
        public void Should_Have_Error_When_Name_Is_Empty()
        {
            var model = new Metric { Name = "", MeterName = "meter", Value = 1.0, Unit = "ms" };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Name)
                .WithErrorMessage("validation-name-empty");
        }

        /// <summary>
        /// Verifies that a validation error is produced when the <c>Name</c> property has an invalid format.
        /// </summary>
        [Fact]
        public void Should_Have_Error_When_Name_Is_Invalid_Format()
        {
            var model = new Metric { Name = "Invalid Name!", MeterName = "meter", Value = 1.0, Unit = "ms" };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Name)
                .WithErrorMessage("validation-invalid-opentelemetry");
        }

        /// <summary>
        /// Verifies that a validation error is produced when the <c>MeterName</c> property is <c>null</c>.
        /// </summary>
        [Fact]
        public void Should_Have_Error_When_MeterName_Is_Null()
        {
            var model = new Metric { Name = "valid_name", MeterName = null!, Value = 1.0, Unit = "ms" };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.MeterName)
                .WithErrorMessage("validation-name-empty");
        }

        /// <summary>
        /// Verifies that a validation error is produced when the <c>MeterName</c> property is an empty string.
        /// </summary>
        [Fact]
        public void Should_Have_Error_When_MeterName_Is_Empty()
        {
            var model = new Metric { Name = "valid_name", MeterName = "", Value = 1.0, Unit = "ms" };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.MeterName)
                .WithErrorMessage("validation-name-empty");
        }

        /// <summary>
        /// Verifies that a validation error is produced when the <c>Unit</c> property is an empty string.
        /// </summary>
        [Fact]
        public void Should_Have_Error_When_Unit_Is_Empty()
        {
            var model = new Metric { Name = "valid_name", MeterName = "meter", Value = 1.0, Unit = "" };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Unit)
                .WithErrorMessage("validation-unit-empty");
        }

        /// <summary>
        /// Verifies that no validation errors are produced when all properties are valid.
        /// </summary>
        [Fact]
        public void Should_Not_Have_Error_When_All_Fields_Are_Valid()
        {
            var model = new Metric { Name = "valid_name", MeterName = "meter", Value = 1.0, Unit = "ms" };
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}