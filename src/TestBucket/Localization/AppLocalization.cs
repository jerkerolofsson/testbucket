using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Extensions.Localization;

using TestBucket.Contracts.Localization;

namespace TestBucket.Localization;
public class AppLocalization : IAppLocalization
{
    public AppLocalization(
        IStringLocalizer<SharedStrings> shared,
        IStringLocalizer<ProjectStrings> project,
        IStringLocalizer<CodeStrings> code,
        IStringLocalizer<SettingStrings> settings,
        IStringLocalizer<InsightStrings> insight,
        IStringLocalizer<HttpStrings> http,
        IStringLocalizer<AccountStrings> account,
        IStringLocalizer<IssueStrings> issues,
        IStringLocalizer<ErrorStrings> error,
        IStringLocalizer<ValidationStrings> validation,
        IStringLocalizer<SecurityStrings> security,
        IStringLocalizer<RequirementStrings> requirement,
        IStringLocalizer<TestEnvironmentStrings> testEnv,
        IStringLocalizer<StateStrings> states,
        IStringLocalizer<FieldStrings> field)
    {
        Account = account;
        Shared = shared;
        Project = project;
        Code = code;
        Http = http;
        Errors = error;
        Insights = insight;
        Requirements = requirement;
        TestEnvironment = testEnv;
        Security = security;
        States = states;
        Fields = field;
        Settings = settings;
        Issues = issues;
        Validation = validation;
    }

    public required IStringLocalizer Account { get; init; }
    public required IStringLocalizer Validation { get; init; }
    public required IStringLocalizer Security { get; init; }
    public required IStringLocalizer States { get; init; }
    public required IStringLocalizer TestEnvironment { get; init; }
    public required IStringLocalizer Issues { get; init; }
    public required IStringLocalizer Requirements { get; init; }
    public required IStringLocalizer Fields { get; init; }
    public required IStringLocalizer Shared { get; init; }
    public required IStringLocalizer Project { get; init; }
    public required IStringLocalizer Settings { get; init; }
    public required IStringLocalizer Insights { get; init; }
    public required IStringLocalizer Code { get; init; }
    public required IStringLocalizer Errors { get; init; }
    public required IStringLocalizer Http { get; init; }
}
