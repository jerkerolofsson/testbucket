using System;
using TestBucket.Domain.Issues.Models;
using TestBucket.Domain.Requirements.Models;
using TestBucket.Domain.Shared;

namespace TestBucket.Domain.UnitTests.Shared
{
    /// <summary>
    /// Unit tests for the <see cref="NavigationState"/> class.
    /// </summary>
    [UnitTest]
    [FunctionalTest]
    [Component("User Interface")]
    public class AppNavigationStateTests
    {
        private readonly NavigationState _navState;

        /// <summary>
        /// Initializes a new instance of the <see cref="AppNavigationStateTests"/> class.
        /// </summary>
        public AppNavigationStateTests()
        {
            _navState = new NavigationState();
        }

        /// <summary>
        /// Tests that NavigationState.SetSelectedTestCase correctly updates the selected test case.
        /// </summary>
        [Fact]
        public void SetSelectedTestCase_ShouldUpdateSelectedTestCase()
        {
            // Arrange
            var testCase = new TestCase { Id = 1, Name = "Test Case 1" };

            // Act
            _navState.SetSelectedTestCase(testCase);

            // Assert
            Assert.Equal(testCase, _navState.SelectedTestCase);
        }

        /// <summary>
        /// Tests that <see cref="NavigationState.ClearSelection"/> resets all selected entities to null.
        /// </summary>
        [Fact]
        public void ClearSelection_ShouldResetSelectedEntities()
        {
            // Arrange
            _navState.SetSelectedTestCase(new TestCase { Id = 1, Name = "123" });
            _navState.SetSelectedTestLabFolder(new TestLabFolder { Id = 2, Name = "123" });

            // Act
            _navState.ClearSelection();

            // Assert
            Assert.Null(_navState.SelectedTestCase);
            Assert.Null(_navState.SelectedTestLabFolder);
        }

        /// <summary>
        /// Tests that <see cref="NavigationState.SelectTestLab"/> sets <see cref="NavigationState.IsTestLabSelected"/> to true.
        /// </summary>
        [Fact]
        public void SelectTestLab_ShouldSetIsTestLabSelected()
        {
            // Act
            _navState.SelectTestLab();

            // Assert
            Assert.True(_navState.IsTestLabSelected);
        }

        /// <summary>
        /// Tests that <see cref="NavigationState.SelectTestRepository"/> sets <see cref="NavigationState.IsTestRepositorySelected"/> to true.
        /// </summary>
        [Fact]
        public void SelectTestRepository_ShouldSetIsTestRepositorySelected()
        {
            // Act
            _navState.SelectTestRepository();

            // Assert
            Assert.True(_navState.IsTestRepositorySelected);
        }

        /// <summary>
        /// Tests that NavigationState.SetSelectedRequirement> correctly updates the selected requirement.
        /// </summary>
        [Fact]
        public void SetSelectedRequirement_ShouldUpdateSelectedRequirement()
        {
            // Arrange
            var requirement = new Requirement { Id = 1, Name = "Requirement 1" };

            // Act
            _navState.SetSelectedRequirement(requirement);

            // Assert
            Assert.Equal(requirement, _navState.SelectedRequirement);
        }

        /// <summary>
        /// Tests that <see cref="NavigationState.SetSelectedTestSuite"/> correctly updates the selected test suite.
        /// </summary>
        [Fact]
        public void SetSelectedTestSuite_ShouldUpdateSelectedTestSuite()
        {
            // Arrange
            var testSuite = new TestSuite { Id = 1, Name = "Test Suite 1" };

            // Act
            _navState.SetSelectedTestSuite(testSuite);

            // Assert
            Assert.Equal(testSuite, _navState.SelectedTestSuite);
        }

        /// <summary>
        /// Tests that <see cref="NavigationState.SetMultiSelectedTestCases"/> correctly updates the list of multi-selected test cases.
        /// </summary>
        [Fact]
        public void SetMultiSelectedTestCases_ShouldUpdateMultiSelectedTestCases()
        {
            // Arrange
            var testCases = new List<TestCase>
            {
                new TestCase { Id = 1, Name = "Test Case 1" },
                new TestCase { Id = 2, Name = "Test Case 2" }
            };

            // Act
            _navState.SetMultiSelectedTestCases(testCases);

            // Assert
            Assert.Equal(testCases, _navState.MultiSelectedTestCases);
        }

        /// <summary>
        /// Tests that setting <see cref="NavigationState.SelectedIssue"/> updates the selected issue.
        /// </summary>
        [Fact]
        public void SetSelectedIssue_ShouldUpdateSelectedIssue()
        {
            // Arrange
            var issue = new LocalIssue { Id = 1, Title = "Issue 1" };

            // Act
            _navState.SelectedIssue = issue;

            // Assert
            Assert.Equal(issue, _navState.SelectedIssue);
        }

        /// <summary>
        /// Tests that setting <see cref="NavigationState.SelectedLinkedIssue"/> updates the selected linked issue.
        /// </summary>
        [Fact]
        public void SetSelectedLinkedIssue_ShouldUpdateSelectedLinkedIssue()
        {
            // Arrange
            var linkedIssue = new LinkedIssue { Id = 1, Title = "Linked Issue 1" };

            // Act
            _navState.SelectedLinkedIssue = linkedIssue;

            // Assert
            Assert.Equal(linkedIssue, _navState.SelectedLinkedIssue);
        }
    }
}