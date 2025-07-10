using Microsoft.Extensions.Localization;

namespace TestBucket.Contracts.Localization;
public interface IAppLocalization
{
    IStringLocalizer Code { get; init; }
    IStringLocalizer Errors { get; init; }
    IStringLocalizer Validation { get; init; }
    IStringLocalizer Http { get; init; }
    IStringLocalizer Insights { get; init; }
    IStringLocalizer Project { get; init; }
    IStringLocalizer Settings { get; init; }
    IStringLocalizer Shared { get; init; }
    IStringLocalizer Account { get; init; }
    IStringLocalizer Requirements { get; init; }
}