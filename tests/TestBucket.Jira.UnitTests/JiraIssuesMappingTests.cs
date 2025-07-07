using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestBucket.Contracts.Issues.States;
using TestBucket.Jira.Issues;
using TestBucket.Jira.Models;
using TestBucket.Traits.Xunit;

namespace TestBucket.Jira.UnitTests
{
    /// <summary>
    /// Unit tests for Jira issue status mapping functionality.
    /// Tests the mapping of Jira status strings to internal issue states.
    /// </summary>
    [EnrichedTest]
    [FunctionalTest]
    [UnitTest]
    [Component("Jira")]
    [Feature("Jira")]
    public class JiraIssuesMappingTests
    {
        /// <summary>
        /// Tests that a null Jira status returns the default "Other" state.
        /// </summary>
        [Fact]
        public void MapJiraStatusToMappedState_WithNullStatus_ReturnsOtherState()
        {
            // Arrange
            var stateMapping = new DefaultStateMap();

            // Act
            var result = JiraIssues.MapJiraStatusToMappedState(null, stateMapping);

            // Assert
            Assert.Equal(IssueStates.Other, result.Name);
            Assert.Equal(MappedIssueState.Other, result.MappedState);
        }

        /// <summary>
        /// Tests that an empty string Jira status returns the default "Other" state.
        /// </summary>
        [Fact]
        public void MapJiraStatusToMappedState_WithEmptyStatus_ReturnsOtherState()
        {
            // Arrange
            var stateMapping = new DefaultStateMap();

            // Act
            var result = JiraIssues.MapJiraStatusToMappedState("", stateMapping);

            // Assert
            Assert.Equal(IssueStates.Other, result.Name);
            Assert.Equal(MappedIssueState.Other, result.MappedState);
        }

        /// <summary>
        /// Tests that known Jira statuses are correctly mapped to their corresponding internal states.
        /// </summary>
        /// <param name="jiraStatus">The Jira status string to test.</param>
        /// <param name="expectedName">The expected mapped state name.</param>
        /// <param name="expectedMappedState">The expected mapped state enum value.</param>
        [Theory]
        [InlineData("To Do", IssueStates.Open, MappedIssueState.Open)]
        [InlineData("In Progress", IssueStates.InProgress, MappedIssueState.InProgress)]
        [InlineData("Done", IssueStates.Closed, MappedIssueState.Closed)]
        [InlineData("In Review", IssueStates.InReview, MappedIssueState.InReview)]
        public void MapJiraStatusToMappedState_WithKnownStatus_ReturnsMappedState(string jiraStatus, string expectedName, MappedIssueState expectedMappedState)
        {
            // Arrange
            var stateMapping = new DefaultStateMap();

            // Act
            var result = JiraIssues.MapJiraStatusToMappedState(jiraStatus, stateMapping);

            // Assert
            Assert.Equal(expectedName, result.Name);
            Assert.Equal(expectedMappedState, result.MappedState);
        }

        /// <summary>
        /// Tests that unknown Jira statuses are preserved as custom states with "Other" mapping.
        /// </summary>
        [Fact]
        public void MapJiraStatusToMappedState_WithUnknownStatus_ReturnsCustomStateWithOtherMapping()
        {
            // Arrange
            var stateMapping = new DefaultStateMap();
            var unknownStatus = "Custom Unknown Status";

            // Act
            var result = JiraIssues.MapJiraStatusToMappedState(unknownStatus, stateMapping);

            // Assert
            Assert.Equal(unknownStatus, result.Name);
            Assert.Equal(MappedIssueState.Other, result.MappedState);
        }

        /// <summary>
        /// Tests that case-sensitive status mapping falls back to custom state when no exact match is found.
        /// </summary>
        [Fact]
        public void MapJiraStatusToMappedState_WithCaseSensitiveStatus_ReturnsCustomStateWhenNotMatched()
        {
            // Arrange
            var stateMapping = new DefaultStateMap();
            var statusWithDifferentCase = "to do"; // lowercase

            // Act
            var result = JiraIssues.MapJiraStatusToMappedState(statusWithDifferentCase, stateMapping);

            // Assert
            Assert.Equal(statusWithDifferentCase, result.Name);
            Assert.Equal(MappedIssueState.Other, result.MappedState);
        }

        /// <summary>
        /// Tests that custom state mappings override default mappings when configured.
        /// </summary>
        [Fact]
        public void MapJiraStatusToMappedState_WithCustomMapping_ReturnsCustomMappedState()
        {
            // Arrange
            var stateMapping = new IssueStateMapping();
            var customStatus = "Pending Review";
            var customState = new IssueState(IssueStates.Reviewed, MappedIssueState.Reviewed);
            stateMapping.AddMapping(customStatus, customState);

            // Act
            var result = JiraIssues.MapJiraStatusToMappedState(customStatus, stateMapping);

            // Assert
            Assert.Equal(IssueStates.Reviewed, result.Name);
            Assert.Equal(MappedIssueState.Reviewed, result.MappedState);
        }

        /// <summary>
        /// Tests that an empty state mapping configuration returns custom states for any input.
        /// </summary>
        [Fact]
        public void MapJiraStatusToMappedState_WithEmptyStateMapping_ReturnsCustomStateForAnyInput()
        {
            // Arrange
            var stateMapping = new IssueStateMapping();
            var jiraStatus = "Any Status";

            // Act
            var result = JiraIssues.MapJiraStatusToMappedState(jiraStatus, stateMapping);

            // Assert
            Assert.Equal(jiraStatus, result.Name);
            Assert.Equal(MappedIssueState.Other, result.MappedState);
        }

        /// <summary>
        /// Tests that custom mappings can override default state mappings for standard Jira statuses.
        /// </summary>
        [Fact]
        public void MapJiraStatusToMappedState_WithOverriddenDefaultMapping_ReturnsOverriddenState()
        {
            // Arrange
            var stateMapping = new DefaultStateMap();
            var customState = new IssueState(IssueStates.Triaged, MappedIssueState.Triaged);
            stateMapping.AddMapping("To Do", customState); // Override default "To Do" mapping

            // Act
            var result = JiraIssues.MapJiraStatusToMappedState("To Do", stateMapping);

            // Assert
            Assert.Equal(IssueStates.Triaged, result.Name);
            Assert.Equal(MappedIssueState.Triaged, result.MappedState);
        }
    }
}