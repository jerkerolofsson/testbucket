using FluentValidation;

using TestBucket.Domain.Metrics.Models;

namespace TestBucket.Domain.Metrics.Validation;
public class MetricValidator : AbstractValidator<Metric>
{
    public MetricValidator()
    {
        RuleFor(x => x.Name).NotNull().WithMessage(x => "validation-name-empty");
        RuleFor(x => x.Name).NotEmpty().WithMessage(x => "validation-name-empty");

        RuleFor(x => x.MeterName).NotNull().WithMessage(x => "validation-name-empty");
        RuleFor(x => x.MeterName).NotEmpty().WithMessage(x => "validation-name-empty");

        // Only allow lower case alphanumerical characters and hyphens
        RuleFor(x => x.Name)
            .Matches("^[a-z0-9_\\.]+$")
            .WithMessage(x => "validation-invalid-opentelemetry");

        //RuleFor(x => x.MeterName)
        //  .Matches("^[a-z0-9_\\.]+$")
        //  .WithMessage(x => "validation-invalid-opentelemetry");

        RuleFor(x => x.Unit).NotEmpty().WithMessage(x => "validation-unit-empty");
        RuleFor(x => x.Unit).NotEmpty().WithMessage(x => "validation-unit-empty");
    }
}
