using TestBucket.Contracts.Issues.States;
using TestBucket.Domain.Issues.Models;
using TestBucket.Domain.Issues.Specifications;
using TestBucket.Domain.Testing.Specifications.TestCases;

namespace TestBucket.Domain.UnitTests.Issues
{
    /// <summary>
    /// Contains unit tests for searching issues by various criteria using different specifications.
    /// </summary>
    [FunctionalTest]
    [EnrichedTest]
    [UnitTest]
    [Feature("Search")]
    [Component("Issues")]
    public class IssueFilterSpecificationTests
    {
        /// <summary>
        /// Verifies that <see cref="FindLocalIssueByExternalId"/> returns true when the external ID matches.
        /// </summary>
        [Fact]
        public void IsMatch_ReturnsTrue_WhenExternalIdMatches()
        {
            // Arrange
            var expectedId = "EXT-123";
            var issue = new LocalIssue { ExternalId = expectedId };
            var spec = new FindLocalIssueByExternalId(expectedId);

            // Act
            var result = spec.IsMatch(issue);

            // Assert
            Assert.True(result);
        }

        /// <summary>
        /// Verifies that <see cref="FindLocalIssueByExternalId"/> returns false when the external ID does not match.
        /// </summary>
        [Fact]
        public void IsMatch_ReturnsFalse_WhenExternalIdDoesNotMatch()
        {
            // Arrange
            var expectedId = "EXT-123";
            var issue = new LocalIssue { ExternalId = "DIFFERENT-ID" };
            var spec = new FindLocalIssueByExternalId(expectedId);

            // Act
            var result = spec.IsMatch(issue);

            // Assert
            Assert.False(result);
        }

        /// <summary>
        /// Verifies that <see cref="FindLocalIssueByExternalId"/> returns false when the issue's external ID is null.
        /// </summary>
        [Fact]
        public void IsMatch_ReturnsFalse_WhenExternalIdIsNull()
        {
            // Arrange
            var expectedId = "EXT-123";
            var issue = new LocalIssue { ExternalId = null };
            var spec = new FindLocalIssueByExternalId(expectedId);

            // Act
            var result = spec.IsMatch(issue);

            // Assert
            Assert.False(result);
        }

        /// <summary>
        /// Verifies that <see cref="FindLocalIssueByType"/> returns true when the type matches, case-insensitively.
        /// </summary>
        [Fact]
        public void IsMatch_ReturnsTrue_WhenTypeMatches_CaseInsensitive()
        {
            var issue = new LocalIssue { IssueType = "Bug" };
            var spec = new FindLocalIssueByType("bug");
            Assert.True(spec.IsMatch(issue));
        }

        /// <summary>
        /// Verifies that <see cref="FindLocalIssueByType"/> returns false when the type does not match.
        /// </summary>
        [Fact]
        public void IsMatch_ReturnsFalse_WhenTypeDoesNotMatch()
        {
            var issue = new LocalIssue { IssueType = "Feature" };
            var spec = new FindLocalIssueByType("bug");
            Assert.False(spec.IsMatch(issue));
        }

        /// <summary>
        /// Verifies that <see cref="FindLocalIssueByType"/> returns false when the issue's type is null.
        /// </summary>
        [Fact]
        public void IsMatch_ReturnsFalse_WhenTypeIsNull()
        {
            var issue = new LocalIssue { IssueType = null };
            var spec = new FindLocalIssueByType("bug");
            Assert.False(spec.IsMatch(issue));
        }

        /// <summary>
        /// Verifies that <see cref="FindLocalIssueById"/> returns true when the ID matches.
        /// </summary>
        [Fact]
        public void IsMatch_ReturnsTrue_WhenIdMatches()
        {
            var issue = new LocalIssue { Id = 42 };
            var spec = new FindLocalIssueById(42);
            Assert.True(spec.IsMatch(issue));
        }

        /// <summary>
        /// Verifies that <see cref="FindLocalIssueById"/> returns false when the ID does not match.
        /// </summary>
        [Fact]
        public void IsMatch_ReturnsFalse_WhenIdDoesNotMatch()
        {
            var issue = new LocalIssue { Id = 99 };
            var spec = new FindLocalIssueById(42);
            Assert.False(spec.IsMatch(issue));
        }

        /// <summary>
        /// Verifies that <see cref="FilterIssuesThatRequireClassification"/> returns true when classification is required.
        /// </summary>
        [Fact]
        public void IsMatch_ReturnsTrue_WhenClassificationRequired()
        {
            var issue = new LocalIssue { ClassificationRequired = true };
            var spec = new FilterIssuesThatRequireClassification();
            Assert.True(spec.IsMatch(issue));
        }

        /// <summary>
        /// Verifies that <see cref="FilterIssuesThatRequireClassification"/> returns false when classification is not required.
        /// </summary>
        [Fact]
        public void IsMatch_ReturnsFalse_WhenClassificationNotRequired()
        {
            var issue = new LocalIssue { ClassificationRequired = false };
            var spec = new FilterIssuesThatRequireClassification();
            Assert.False(spec.IsMatch(issue));
        }

        /// <summary>
        /// Verifies that <see cref="FilterLocalIssueByStringField"/> returns true when the string field matches.
        /// </summary>
        [Fact]
        public void FilterRequirementByStringField_ReturnsTrue_WhenFieldMatches()
        {
            var issue = new LocalIssue()
            {
                IssueFields = new List<IssueField>
                {
                    new IssueField { FieldDefinitionId = 1, StringValue = "expected", LocalIssueId = 1 }
                }
            };
            var spec = new FilterLocalIssueByStringField(1, "expected");
            Assert.True(spec.IsMatch(issue));
        }

        /// <summary>
        /// Verifies that <see cref="FilterLocalIssueByStringField"/> returns false when the string field does not match.
        /// </summary>
        [Fact]
        public void FilterRequirementByStringField_ReturnsFalse_WhenFieldDoesNotMatch()
        {
            var issue = new LocalIssue()
            {
                IssueFields = new List<IssueField>
                {
                    new IssueField { FieldDefinitionId = 1, StringValue = "expected", LocalIssueId = 1 }
                }
            }; var spec = new FilterLocalIssueByStringField(1, "other");
            Assert.False(spec.IsMatch(issue));
        }

        /// <summary>
        /// Verifies that <see cref="FindLocalIssueByExternalDisplayId"/> returns true when the display ID matches.
        /// </summary>
        [Fact]
        public void FindLocalIssueByExternalDisplayId_ReturnsTrue_WhenDisplayIdMatches()
        {
            var issue = new LocalIssue { ExternalDisplayId = "EXT-001" };
            var spec = new FindLocalIssueByExternalDisplayId("EXT-001");
            Assert.True(spec.IsMatch(issue));
        }

        /// <summary>
        /// Verifies that <see cref="FindLocalIssueByExternalDisplayId"/> returns false when the display ID does not match.
        /// </summary>
        [Fact]
        public void FindLocalIssueByExternalDisplayId_ReturnsFalse_WhenDisplayIdDoesNotMatch()
        {
            var issue = new LocalIssue { ExternalDisplayId = "EXT-002" };
            var spec = new FindLocalIssueByExternalDisplayId("EXT-001");
            Assert.False(spec.IsMatch(issue));
        }

        /// <summary>
        /// Verifies that <see cref="FindLocalIssueByExternalSystemId"/> returns true when the system ID matches.
        /// </summary>
        [Fact]
        public void FindLocalIssueByExternalSystemId_ReturnsTrue_WhenSystemIdMatches()
        {
            var issue = new LocalIssue { ExternalSystemId = 42 };
            var spec = new FindLocalIssueByExternalSystemId(42);
            Assert.True(spec.IsMatch(issue));
        }

        /// <summary>
        /// Verifies that <see cref="FindLocalIssueByExternalSystemId"/> returns false when the system ID does not match.
        /// </summary>
        [Fact]
        public void FindLocalIssueByExternalSystemId_ReturnsFalse_WhenSystemIdDoesNotMatch()
        {
            var issue = new LocalIssue { ExternalSystemId = 99 };
            var spec = new FindLocalIssueByExternalSystemId(42);
            Assert.False(spec.IsMatch(issue));
        }

        /// <summary>
        /// Verifies that <see cref="FindLocalIssueByExternalSystemName"/> returns true when the system name matches.
        /// </summary>
        [Fact]
        public void FindLocalIssueByExternalSystemName_ReturnsTrue_WhenSystemNameMatches()
        {
            var issue = new LocalIssue { ExternalSystemName = "Jira" };
            var spec = new FindLocalIssueByExternalSystemName("Jira");
            Assert.True(spec.IsMatch(issue));
        }

        /// <summary>
        /// Verifies that <see cref="FindLocalIssueByExternalSystemName"/> returns false when the system name does not match.
        /// </summary>
        [Fact]
        public void FindLocalIssueByExternalSystemName_ReturnsFalse_WhenSystemNameDoesNotMatch()
        {
            var issue = new LocalIssue { ExternalSystemName = "Azure" };
            var spec = new FindLocalIssueByExternalSystemName("Jira");
            Assert.False(spec.IsMatch(issue));
        }

        /// <summary>
        /// Verifies that <see cref="FindLocalIssueByMappedState"/> returns true when the mapped state matches.
        /// </summary>
        [Fact]
        public void FindLocalIssueByMappedState_ReturnsTrue_WhenMappedStateMatches()
        {
            var issue = new LocalIssue { MappedState = MappedIssueState.Open };
            var spec = new FindLocalIssueByMappedState(MappedIssueState.Open);
            Assert.True(spec.IsMatch(issue));
        }

        /// <summary>
        /// Verifies that <see cref="FindLocalIssueByMappedState"/> returns false when the mapped state does not match.
        /// </summary>
        [Fact]
        public void FindLocalIssueByMappedState_ReturnsFalse_WhenMappedStateDoesNotMatch()
        {
            var issue = new LocalIssue { MappedState = MappedIssueState.Closed };
            var spec = new FindLocalIssueByMappedState(MappedIssueState.Open);
            Assert.False(spec.IsMatch(issue));
        }

        /// <summary>
        /// Verifies that <see cref="FindLocalIssueByState"/> returns true when the state matches.
        /// </summary>
        [Fact]
        public void FindLocalIssueByState_ReturnsTrue_WhenStateMatches()
        {
            var issue = new LocalIssue { State = "Active" };
            var spec = new FindLocalIssueByState("Active");
            Assert.True(spec.IsMatch(issue));
        }

        /// <summary>
        /// Verifies that <see cref="FindLocalIssueByState"/> returns false when the state does not match.
        /// </summary>
        [Fact]
        public void FindLocalIssueByState_ReturnsFalse_WhenStateDoesNotMatch()
        {
            var issue = new LocalIssue { State = "Closed" };
            var spec = new FindLocalIssueByState("Active");
            Assert.False(spec.IsMatch(issue));
        }

        /// <summary>
        /// Verifies that <see cref="FindLocalIssueByText"/> returns true when the text is contained in the issue's title.
        /// </summary>
        [Fact]
        public void FindLocalIssueByText_ReturnsTrue_WhenTextIsContained()
        {
            var issue = new LocalIssue { Title = "Critical bug found" };
            var spec = new FindLocalIssueByText("bug");
            Assert.True(spec.IsMatch(issue));
        }

        /// <summary>
        /// Verifies that <see cref="FindLocalIssueByText"/> returns false when the text is not contained in the issue's title.
        /// </summary>
        [Fact]
        public void FindLocalIssueByText_ReturnsFalse_WhenTextIsNotContained()
        {
            var issue = new LocalIssue { Title = "Feature request" };
            var spec = new FindLocalIssueByText("bug");
            Assert.False(spec.IsMatch(issue));
        }
    }

}