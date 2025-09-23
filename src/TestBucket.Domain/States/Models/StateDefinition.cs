
using Microsoft.EntityFrameworkCore.Metadata.Internal;

using TestBucket.Contracts.Issues.States;
using TestBucket.Contracts.Requirements.States;
using TestBucket.Contracts.States;
using TestBucket.Contracts.Testing.States;
using TestBucket.Domain.Requirements.Models;

namespace TestBucket.Domain.States.Models;
public class StateDefinition : ProjectEntity
{
    public long Id { get; set; }

    /// <summary>
    /// States for issues
    /// </summary>
    [Column(TypeName = "jsonb")]
    public required List<IssueState> IssueStates { get; set; }

    /// <summary>
    /// States for requirements
    /// </summary>
    [Column(TypeName = "jsonb")]
    public required List<RequirementState> RequirementStates { get; set; }


    /// <summary>
    /// States for test cases
    /// </summary>
    [Column(TypeName = "jsonb")]
    public required List<TestState> TestCaseStates { get; set; }

    /// <summary>
    /// States for test case runs
    /// </summary>
    [Column(TypeName = "jsonb")]
    public required List<TestState> TestCaseRunStates { get; set; }

    /// <summary>
    /// Creates a typed state entity based on the specified entityType
    /// </summary>
    /// <param name="entityType"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public static BaseState Create(StateEntityType entityType)
    {
        switch (entityType)
        {
            case StateEntityType.Issue:
                return new IssueState();
            case StateEntityType.TestCase:
                return new TestState();
            case StateEntityType.TestCaseRun:
                return new TestState();
            case StateEntityType.Requirement:
                return new RequirementState();
        }
        throw new NotImplementedException();
    }

    /// <summary>
    /// Adds a state to the correct entity collection
    /// </summary>
    /// <param name="entityType"></param>
    /// <param name="state"></param>
    /// <exception cref="ArgumentException"></exception>
    public void Add(StateEntityType entityType, BaseState state)
    {
        switch (entityType)
        {
            case StateEntityType.Issue:
                if (state is IssueState issueState)
                {
                    IssueStates.Add(issueState);
                }
                else
                {
                    throw new ArgumentException("Expected state to be of the type IssueState");
                }
                break;
            case StateEntityType.TestCase:
                if (state is TestState testCaseState)
                {
                    TestCaseStates.Add(testCaseState);
                }
                else
                {
                    throw new ArgumentException("Expected state to be of the type TestState");
                }
                break;
            case StateEntityType.TestCaseRun:
                if (state is TestState testCaseRunState)
                {
                    TestCaseRunStates.Add(testCaseRunState);
                }
                else
                {
                    throw new ArgumentException("Expected state to be of the type TestState");
                }
                break;
            case StateEntityType.Requirement:
                if (state is RequirementState requirementState)
                {
                    RequirementStates.Add(requirementState);
                }
                else
                {
                    throw new ArgumentException("Expected state to be of the type RequirementState");
                }
                break;
        }
        SortStates();
    }

    /// <summary>
    /// Removes a state
    /// </summary>
    /// <param name="entityType"></param>
    /// <param name="state"></param>
    public void Remove(StateEntityType entityType, BaseState state)
    {
        switch(entityType)
        {
            case StateEntityType.Issue:
                IssueStates.RemoveAll(x => x.Equals(state));
                break;
            case StateEntityType.TestCase:
                TestCaseStates.RemoveAll(x => x.Equals(state));
                break;
            case StateEntityType.TestCaseRun:
                TestCaseRunStates.RemoveAll(x => x.Equals(state));
                break;
            case StateEntityType.Requirement:
                RequirementStates.RemoveAll(x => x.Equals(state));
                break;
        }
        SortStates();
    }

    private void SortStates()
    {
        TestCaseStates.Sort((a, b) => a.SortOrder.CompareTo(b.SortOrder));
        IssueStates.Sort((a, b) => a.SortOrder.CompareTo(b.SortOrder));
        TestCaseRunStates.Sort((a, b) => a.SortOrder.CompareTo(b.SortOrder));
        RequirementStates.Sort((a, b) => a.SortOrder.CompareTo(b.SortOrder));
    }

    public IEnumerable<BaseState> GetStates(StateEntityType entityType)
    {
        switch (entityType)
        {
            case StateEntityType.Issue:
                return IssueStates.Cast<BaseState>();
            case StateEntityType.TestCase:
                return TestCaseStates.Cast<BaseState>();
            case StateEntityType.TestCaseRun:
                return TestCaseRunStates.Cast<BaseState>();
            case StateEntityType.Requirement:
                return RequirementStates.Cast<BaseState>();
        }

        return [];
    }

    internal void Clear()
    {
        TestCaseRunStates = [];
        TestCaseStates = [];
        IssueStates = [];
        RequirementStates = [];
    }
}
