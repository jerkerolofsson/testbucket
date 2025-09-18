using TestBucket.Contracts.Issues.States;
using TestBucket.Contracts.Requirements.States;
using TestBucket.Contracts.Testing.States;
using TestBucket.Domain.States.Models;

namespace TestBucket.Domain.States.Caching;

/// <summary>
/// Contains the default states for entities
/// </summary>
public class DefaultStates
{
    /// <summary>
    /// Returns default states if no states has been configured for the project
    /// </summary>
    /// <returns></returns>
    internal static IReadOnlyList<TestState> GetDefaultTestCaseStates()
    {
        return [
                new()
                {
                    Name = TestCaseStates.Draft,
                    MappedState = MappedTestState.Draft,
                    IsInitial = true,
                    Color = "#ADFF2F",
                    AllowedStates = [TestCaseStates.Ongoing, TestCaseStates.Completed]
                },
                new()
                {
                    Name = TestCaseStates.Ongoing, 
                    MappedState = MappedTestState.Ongoing, 
                    IsInitial = true, 
                    AllowedStates = [TestCaseStates.Review, TestCaseStates.Completed]
                },
                new() 
                { 
                    Name = TestCaseStates.Review, 
                    MappedState = MappedTestState.Review, 
                    AllowedStates = [TestCaseStates.Draft, TestCaseStates.Completed] 
                },
                new() 
                {
                    Color = "#6200ee",
                    Name = TestCaseStates.Completed, 
                    MappedState = MappedTestState.Completed, 
                    IsFinal = true, 
                    AllowedStates = [TestCaseStates.Review, TestCaseStates.Draft] 
                },
            ];
    }

    /// <summary>
    /// Returns default states if no states has been configured for the project
    /// </summary>
    /// <returns></returns>
    internal static IReadOnlyList<TestState> GetDefaultTestCaseRunStates()
    {
        return [
                new() 
                { 
                    Name = TestCaseRunStates.NotStarted,
                    MappedState = MappedTestState.NotStarted,
                    IsInitial = true,
                    Color = "#ADFF2F",
                    AllowedStates = [TestCaseRunStates.Assigned, TestCaseRunStates.Ongoing, TestCaseRunStates.Completed]
                },
                new() 
                { 
                    Name = TestCaseRunStates.Assigned, 
                    MappedState = MappedTestState.Assigned,
                    AllowedStates = [TestCaseRunStates.Ongoing] 
                },
                new() 
                { 
                    Name = TestCaseRunStates.Ongoing, 
                    MappedState = MappedTestState.Ongoing, 
                    AllowedStates = [TestCaseRunStates.Assigned, TestCaseRunStates.Completed]
                },
                new() 
                {
                    Color = "purple",
                    Name = TestCaseRunStates.Completed, 
                    MappedState = MappedTestState.Completed, 
                    IsFinal = true,  
                    AllowedStates = [TestCaseRunStates.Assigned, TestCaseRunStates.Ongoing] 
                }
            ];
    }

    /// <summary>
    /// Returns the default final state for test case runs.
    /// </summary>
    internal static TestState GetDefaultTestCaseRunFinalState()
    {
        return GetDefaultTestCaseRunStates().FirstOrDefault(x => x.IsFinal)
            ?? throw new InvalidOperationException("No final state defined in default test case run states.");
    }

    /// <summary>
    /// Returns the default initial state for test case runs.
    /// </summary>
    internal static TestState GetDefaultTestCaseRunInitialState()
    {
        return GetDefaultTestCaseRunStates().FirstOrDefault(x => x.IsInitial)
            ?? throw new InvalidOperationException("No initial state defined in default test case run states.");
    }

    /// <summary>
    /// Returns default requirement states
    /// </summary>
    /// <returns></returns>
    internal static IReadOnlyList<RequirementState> GetDefaultRequirementStates()
    {
        return [
                new() 
                {
                    Name = RequirementStates.Draft, 
                    MappedState = MappedRequirementState.Draft, 
                    IsInitial = true,
                    Color = "#ADFF2F",
                    AllowedStates =
                    [
                        RequirementStates.Accepted,
                        RequirementStates.Assigned,
                        RequirementStates.InProgress,
                        RequirementStates.Completed,
                        RequirementStates.Delivered,
                        RequirementStates.Canceled
                    ] ,
                    Aliases = ["New", "To Do", "Design"]
                },
                new() 
                { 
                    Name = RequirementStates.Accepted, 
                    MappedState = MappedRequirementState.Accepted, 
                    AllowedStates =
                    [
                        RequirementStates.Assigned,
                        RequirementStates.InProgress,
                        RequirementStates.Completed,
                        RequirementStates.Delivered,
                        RequirementStates.Canceled
                    ] 
                },
                new() 
                { 
                    Name = RequirementStates.Assigned, 
                    MappedState = MappedRequirementState.Assigned, 
                    AllowedStates =
                    [
                        RequirementStates.Accepted,

                        RequirementStates.InProgress,
                        RequirementStates.Completed,
                        RequirementStates.Delivered,
                        RequirementStates.Canceled
                    ] 
                },
                new() 
                { 
                    Name = RequirementStates.InProgress, 
                    MappedState = MappedRequirementState.InProgress, 
                    AllowedStates =
                    [
                        RequirementStates.Assigned,

                        RequirementStates.Completed,
                        RequirementStates.Delivered,
                        RequirementStates.Canceled
                    ],
                    Aliases = ["In-Progress", "In Progress"]
                },
                new() 
                { 
                    Name = RequirementStates.Delivered, 
                    MappedState = MappedRequirementState.Delivered, 
                    AllowedStates =
                    [
                        RequirementStates.Assigned,
                        RequirementStates.InProgress,

                        RequirementStates.Completed,
                        RequirementStates.Canceled
                    ]  
                },
                new() 
                { 
                    Name = RequirementStates.Completed,
                    Color = "#6200ee",
                    MappedState = MappedRequirementState.Completed, 
                    IsFinal = true, 
                    AllowedStates =
                    [
                        RequirementStates.Assigned,
                        RequirementStates.InProgress,
                        RequirementStates.Canceled
                    ],
                    Aliases = ["Done", "Closed"]
                },
                new() 
                { 
                    Name = RequirementStates.Canceled, 
                    MappedState = MappedRequirementState.Canceled, 
                    AllowedStates = [],
                    Aliases = ["Rejected"],
                    Color = "#EF476F"
                },
            ];
    }


    /// <summary>
    /// Returns default issue states
    /// </summary>
    /// <returns></returns>
    internal static IReadOnlyList<IssueState> GetDefaultIssueStates()
    {
        return [
                new() { 
                    Name = IssueStates.Open, 
                    MappedState = MappedIssueState.Open, 
                    IsInitial = true,
                    Color = "#ADFF2F",
                    AllowedStates =
                    [
                        IssueStates.Triage,
                        IssueStates.Closed
                    ],
                    Aliases = ["New", "To do"]
                },
                new() 
                { 
                    Name = IssueStates.Triage, 
                    MappedState = MappedIssueState.Triage ,
                    AllowedStates =
                    [
                        IssueStates.Accepted,
                        IssueStates.Closed
                    ]
                },
                new() 
                { 
                    Name = IssueStates.Accepted, 
                    MappedState = MappedIssueState.Accepted,
                    AllowedStates =
                    [
                        IssueStates.Assigned,
                        IssueStates.InProgress,
                        IssueStates.Closed
                    ] },
                new() 
                { 
                    Name = IssueStates.Assigned, 
                    MappedState = MappedIssueState.Assigned,
                    AllowedStates =
                    [
                        IssueStates.InProgress,
                        IssueStates.Closed
                    ]  
                },
                new() 
                { 
                    Name = IssueStates.InProgress, 
                    MappedState = MappedIssueState.InProgress,
                    AllowedStates =
                    [
                        IssueStates.Assigned,
                        IssueStates.InProgress,
                        IssueStates.Closed
                    ],
                    Aliases = ["In Progress", "In-Progress", "Ongoing"]
                },
                new() 
                { 
                    Name = IssueStates.InReview, 
                    MappedState = MappedIssueState.InReview,
                    AllowedStates =
                    [
                        IssueStates.Assigned,
                        IssueStates.Reviewed,
                        IssueStates.Closed
                    ],
                    Aliases = ["Reviewing"]
                },
                new() 
                { 
                    Name = IssueStates.Reviewed, 
                    MappedState = MappedIssueState.Reviewed ,
                    AllowedStates =
                    [
                        IssueStates.Assigned,
                        IssueStates.InReview,
                        IssueStates.Closed
                    ]
                },
                new() 
                { 
                    Name = IssueStates.Closed,
                    Color = "#6200ee",
                    MappedState = MappedIssueState.Closed, 
                    IsFinal = true,
                    AllowedStates = [],
                    Aliases = ["Done", "Completed"]
                },
            ];
    }

    public static StateDefinition CreateDefaultProjectDefinition(string tenantId, long projectId)
    {
        return new StateDefinition
        {
            TenantId = tenantId,
            TeamId = null,
            TestProjectId = projectId,
            IssueStates = [],
            RequirementStates = [],
            TestCaseRunStates = [],
            TestCaseStates = []
        };
    }
    public static StateDefinition CreateDefaultTeamDefinition(string tenantId, long teamId)
    {
        return new StateDefinition
        {
            TenantId = tenantId,
            TeamId = teamId,
            TestProjectId = null,
            IssueStates = [],
            RequirementStates = [],
            TestCaseRunStates = [],
            TestCaseStates = []
        };
    }
    public static StateDefinition CreateDefaultTenantDefinition(string tenantId)
    {
        return new StateDefinition
        {
            TenantId = tenantId,
            TeamId = null,
            TestProjectId = null,
            IssueStates = DefaultStates.GetDefaultIssueStates().ToList(),
            RequirementStates = DefaultStates.GetDefaultRequirementStates().ToList(),
            TestCaseRunStates = DefaultStates.GetDefaultTestCaseRunStates().ToList(),
            TestCaseStates = DefaultStates.GetDefaultTestCaseStates().ToList()
        };
    }
}
