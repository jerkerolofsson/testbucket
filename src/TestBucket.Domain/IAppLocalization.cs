using Microsoft.Extensions.Localization;

namespace TestBucket.Domain;
public interface IAppLocalization
{
    IStringLocalizer Code { get; init; }
    IStringLocalizer Errors { get; init; }
    IStringLocalizer Http { get; init; }
    IStringLocalizer Insights { get; init; }
    IStringLocalizer Project { get; init; }
    IStringLocalizer Settings { get; init; }
    IStringLocalizer Shared { get; init; }
    IStringLocalizer Requirements { get; init; }
}