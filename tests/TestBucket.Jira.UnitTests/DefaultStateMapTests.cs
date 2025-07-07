using TestBucket.Contracts.Issues.States;
using TestBucket.Jira.Models;
using TestBucket.Traits.Xunit;

namespace TestBucket.Jira.UnitTests
{
    /// <summary>
    /// Unit tests for the <see cref="DefaultStateMap"/> class that provides default mappings
    /// between Jira status names and internal issue states.
    /// </summary>
    [EnrichedTest]
    [FunctionalTest]
    [UnitTest]
    [Component("Jira")]
    [Feature("Jira")]
    public class DefaultStateMapTests
    {
        /// <summary>
        /// Tests that the DefaultStateMap can accept additional custom mappings
        /// beyond the default ones without affecting existing mappings.
        /// </summary>
        [Fact]
        public void DefaultStateMap_CanAddAdditionalMappings()
        {
            // Arrange
            var stateMap = new DefaultStateMap();
            var customState = new IssueState(IssueStates.Triaged, MappedIssueState.Triaged);

            // Act
            stateMap.AddMapping("Custom Status", customState);

            // Assert
            Assert.Equal(5, stateMap.Count);
            Assert.True(stateMap.ContainsKey("Custom Status"));
            Assert.Equal(customState, stateMap["Custom Status"]);
        }

        /// <summary>
        /// Verifies that the DefaultStateMap contains the correct default mappings
        /// for standard Jira status names to their corresponding issue states.
        /// </summary>
        /// <param name="jiraStatus">The Jira status name to test.</param>
        /// <param name="expectedName">The expected internal issue state name.</param>
        /// <param name="expectedMappedState">The expected mapped issue state enum value.</param>
        [Theory]    
        [InlineData("To Do", IssueStates.Open, MappedIssueState.Open)]
        [InlineData("In Progress", IssueStates.InProgress, MappedIssueState.InProgress)]
        [InlineData("Done", IssueStates.Closed, MappedIssueState.Closed)]
        [InlineData("In Review", IssueStates.InReview, MappedIssueState.InReview)]
        public void DefaultStateMap_VerifySpecificMapping(string jiraStatus, string expectedName, MappedIssueState expectedMappedState)
        {
            // Arrange
            var stateMap = new DefaultStateMap();

            // Act
            var state = stateMap[jiraStatus];

            // Assert
            Assert.Equal(expectedName, state.Name);
            Assert.Equal(expectedMappedState, state.MappedState);
        }
    }
}
