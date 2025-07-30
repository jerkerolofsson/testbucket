using TestBucket.Domain.Requirements.Models;
using TestBucket.Domain.Requirements.Specifications;
using TestBucket.Domain.Requirements.Specifications.Folders;
using TestBucket.Domain.Requirements.Specifications.Links;
using TestBucket.Domain.Requirements.Specifications.Requirements;

namespace TestBucket.Domain.UnitTests.Requirements.Search
{
    /// <summary>
    /// Contains unit tests for various requirement filter specifications.
    /// </summary>
    [FunctionalTest]
    [EnrichedTest]
    [UnitTest]
    [Feature("Search")]
    [Component("Requirements")]
    public class RequirementFilterSpecificationTests
    {
        /// <summary>
        /// Tests that <see cref="FilterRequirementSpecificationById"/> matches the correct requirement by ID.
        /// </summary>
        [Fact]
        public void FilterRequirementSpecificationById_ShouldMatchCorrectId()
        {
            // Arrange
            var specification = new FilterRequirementSpecificationById(1);
            var requirementSpecification = new RequirementSpecification { Id = 1, Name = "Test" };

            // Act
            var compiledExpression = specification.Expression.Compile();
            var result = compiledExpression(requirementSpecification);

            // Assert
            Assert.True(result);
        }

        /// <summary>
        /// Tests that <see cref="FilterRequirementSpecificationById"/> does not match the incorrect requirement by ID.
        /// </summary>
        [Fact]
        public void FilterRequirementSpecificationById_ShouldNotMatchIncorrectId()
        {
            // Arrange
            var specification = new FilterRequirementSpecificationById(2);
            var requirementSpecification = new RequirementSpecification { Id = 1, Name = "Test" };

            // Act
            var compiledExpression = specification.Expression.Compile();
            var result = compiledExpression(requirementSpecification);

            // Assert
            Assert.False(result);
        }

        /// <summary>
        /// Tests that <see cref="FilterRequirementSpecificationById"/> handles zero ID correctly.
        /// </summary>
        [Fact]
        public void FilterRequirementSpecificationById_ShouldHandleZeroId()
        {
            // Arrange
            var specification = new FilterRequirementSpecificationById(0);
            var requirementSpecification = new RequirementSpecification { Id = 0, Name = "Test" };

            // Act
            var compiledExpression = specification.Expression.Compile();
            var result = compiledExpression(requirementSpecification);

            // Assert
            Assert.True(result);
        }


        /// <summary>
        /// Tests that <see cref="FilterRequirementSpecificationByText"/> matches the correct requirement by text.
        /// </summary>
        [Fact]
        public void FilterRequirementSpecificationByText_ShouldMatchCorrectText()
        {
            // Arrange
            var specification = new FilterRequirementSpecificationByText("Test");
            var requirementSpecification = new RequirementSpecification { Name = "Test Specification" };

            // Act
            var compiledExpression = specification.Expression.Compile();
            var result = compiledExpression(requirementSpecification);

            // Assert
            Assert.True(result);
        }

        /// <summary>
        /// Tests that <see cref="FilterRequirementSpecificationByText"/> does not match the incorrect requirement by text.
        /// </summary>
        [Fact]
        public void FilterRequirementSpecificationByText_ShouldNotMatchIncorrectText()
        {
            // Arrange
            var specification = new FilterRequirementSpecificationByText("Different");
            var requirementSpecification = new RequirementSpecification { Name = "Test Specification" };

            // Act
            var compiledExpression = specification.Expression.Compile();
            var result = compiledExpression(requirementSpecification);

            // Assert
            Assert.False(result);
        }

        /// <summary>
        /// Tests that <see cref="FilterRequirementSpecificationByText"/> performs case-insensitive matching.
        /// </summary>
        [Fact]
        public void FilterRequirementSpecificationByText_ShouldMatchCaseInsensitive()
        {
            // Arrange
            var specification = new FilterRequirementSpecificationByText("TEST");
            var requirementSpecification = new RequirementSpecification { Name = "test specification" };

            // Act
            var compiledExpression = specification.Expression.Compile();
            var result = compiledExpression(requirementSpecification);

            // Assert
            Assert.True(result);
        }

        /// <summary>
        /// Tests that <see cref="FilterRequirementSpecificationByText"/> handles empty search text.
        /// </summary>
        [Fact]
        public void FilterRequirementSpecificationByText_ShouldMatchEmptyText()
        {
            // Arrange
            var specification = new FilterRequirementSpecificationByText("");
            var requirementSpecification = new RequirementSpecification { Name = "Any Specification" };

            // Act
            var compiledExpression = specification.Expression.Compile();
            var result = compiledExpression(requirementSpecification);

            // Assert
            Assert.True(result);
        }


        /// <summary>
        /// Tests that <see cref="FilterRequirementSpecificationByText"/> matches partial text in the middle.
        /// </summary>
        [Fact]
        public void FilterRequirementSpecificationByText_ShouldMatchPartialText()
        {
            // Arrange
            var specification = new FilterRequirementSpecificationByText("mid");
            var requirementSpecification = new RequirementSpecification { Name = "Start Middle End" };

            // Act
            var compiledExpression = specification.Expression.Compile();
            var result = compiledExpression(requirementSpecification);

            // Assert
            Assert.True(result);
        }

        /// <summary>
        /// Tests that <see cref="FilterRequirementSpecificationByExternalId"/> matches the correct requirement by external ID.
        /// </summary>
        [Fact]
        public void FilterRequirementSpecificationByExternalId_ShouldMatchCorrectExternalId()
        {
            // Arrange
            var specification = new FilterRequirementSpecificationByExternalId("ProviderA", "12345");
            var requirementSpecification = new RequirementSpecification { ExternalProvider = "ProviderA", ExternalId = "12345", Name = "Test" };

            // Act
            var compiledExpression = specification.Expression.Compile();
            var result = compiledExpression(requirementSpecification);

            // Assert
            Assert.True(result);
        }

        /// <summary>
        /// Tests that <see cref="FilterRequirementSpecificationByExternalId"/> does not match with an incorrect external ID.
        /// </summary>
        [Fact]
        public void FilterRequirementSpecificationByExternalId_ShouldNotMatchIncorrectExternalId()
        {
            // Arrange
            var specification = new FilterRequirementSpecificationByExternalId("ProviderA", "67890");
            var requirementSpecification = new RequirementSpecification { ExternalProvider = "ProviderA", ExternalId = "12345", Name = "Test" };

            // Act
            var compiledExpression = specification.Expression.Compile();
            var result = compiledExpression(requirementSpecification);

            // Assert
            Assert.False(result);
        }

        /// <summary>
        /// Tests that <see cref="FilterRequirementSpecificationByExternalId"/> does not match with an incorrect provider.
        /// </summary>
        [Fact]
        public void FilterRequirementSpecificationByExternalId_ShouldNotMatchIncorrectProvider()
        {
            // Arrange
            var specification = new FilterRequirementSpecificationByExternalId("ProviderB", "12345");
            var requirementSpecification = new RequirementSpecification { ExternalProvider = "ProviderA", ExternalId = "12345", Name = "Test" };

            // Act
            var compiledExpression = specification.Expression.Compile();
            var result = compiledExpression(requirementSpecification);

            // Assert
            Assert.False(result);
        }

        /// <summary>
        /// Tests that <see cref="FilterRequirementSpecificationByExternalId"/> handles null external provider.
        /// </summary>
        [Fact]
        public void FilterRequirementSpecificationByExternalId_ShouldHandleNullExternalProvider()
        {
            // Arrange
            var specification = new FilterRequirementSpecificationByExternalId("ProviderA", "12345");
            var requirementSpecification = new RequirementSpecification { ExternalProvider = null, ExternalId = "12345", Name = "Test" };

            // Act
            var compiledExpression = specification.Expression.Compile();
            var result = compiledExpression(requirementSpecification);

            // Assert
            Assert.False(result);
        }

        /// <summary>
        /// Tests that <see cref="FilterRequirementSpecificationByExternalId"/> handles null external ID.
        /// </summary>
        [Fact]
        public void FilterRequirementSpecificationByExternalId_ShouldHandleNullExternalId()
        {
            // Arrange
            var specification = new FilterRequirementSpecificationByExternalId("ProviderA", "12345");
            var requirementSpecification = new RequirementSpecification { ExternalProvider = "ProviderA", ExternalId = null, Name = "Test" };

            // Act
            var compiledExpression = specification.Expression.Compile();
            var result = compiledExpression(requirementSpecification);

            // Assert
            Assert.False(result);
        }

        /// <summary>
        /// Tests that <see cref="FilterRequirementSpecificationByExternalId"/> handles empty strings.
        /// </summary>
        [Fact]
        public void FilterRequirementSpecificationByExternalId_ShouldMatchEmptyStrings()
        {
            // Arrange
            var specification = new FilterRequirementSpecificationByExternalId("", "");
            var requirementSpecification = new RequirementSpecification { ExternalProvider = "", ExternalId = "", Name = "Test" };

            // Act
            var compiledExpression = specification.Expression.Compile();
            var result = compiledExpression(requirementSpecification);

            // Assert
            Assert.True(result);
        }

        /// <summary>
        /// Tests that <see cref="FilterRequirementSpecificationBySlug"/> matches the correct requirement by slug.
        /// </summary>
        [Fact]
        public void FilterRequirementSpecificationBySlug_ShouldMatchCorrectSlug()
        {
            // Arrange
            var specification = new FilterRequirementSpecificationBySlug("test-slug");
            var requirementSpecification = new RequirementSpecification { Slug = "test-slug", Name = "Test" };

            // Act
            var compiledExpression = specification.Expression.Compile();
            var result = compiledExpression(requirementSpecification);

            // Assert
            Assert.True(result);
        }

        /// <summary>
        /// Tests that <see cref="FilterRequirementSpecificationBySlug"/> does not match an incorrect slug.
        /// </summary>
        [Fact]
        public void FilterRequirementSpecificationBySlug_ShouldNotMatchIncorrectSlug()
        {
            // Arrange
            var specification = new FilterRequirementSpecificationBySlug("another-slug");
            var requirementSpecification = new RequirementSpecification { Slug = "test-slug", Name = "Test" };

            // Act
            var compiledExpression = specification.Expression.Compile();
            var result = compiledExpression(requirementSpecification);

            // Assert
            Assert.False(result);
        }

        /// <summary>
        /// Tests that <see cref="FilterRequirementSpecificationBySlug"/> handles null slug.
        /// </summary>
        [Fact]
        public void FilterRequirementSpecificationBySlug_ShouldHandleNullSlug()
        {
            // Arrange
            var specification = new FilterRequirementSpecificationBySlug("test-slug");
            var requirementSpecification = new RequirementSpecification { Slug = null, Name = "Test" };

            // Act
            var compiledExpression = specification.Expression.Compile();
            var result = compiledExpression(requirementSpecification);

            // Assert
            Assert.False(result);
        }

        /// <summary>
        /// Tests that <see cref="FilterRequirementSpecificationBySlug"/> handles empty slug.
        /// </summary>
        [Fact]
        public void FilterRequirementSpecificationBySlug_ShouldMatchEmptySlug()
        {
            // Arrange
            var specification = new FilterRequirementSpecificationBySlug("");
            var requirementSpecification = new RequirementSpecification { Slug = "", Name = "Test" };

            // Act
            var compiledExpression = specification.Expression.Compile();
            var result = compiledExpression(requirementSpecification);

            // Assert
            Assert.True(result);
        }

        /// <summary>
        /// Tests that <see cref="FilterRequirementByIsOpen"/> returns true for open requirements.
        /// </summary>
        [Fact]
        public void FilterRequirementByIsOpen_ShouldReturnTrueForOpenRequirements()
        {
            // Arrange
            var specification = new FilterRequirementByIsOpen();
            var openRequirement = new Requirement { MappedState = Contracts.Requirements.States.MappedRequirementState.InProgress, Name = "Open Requirement" };

            // Act
            var compiledExpression = specification.Expression.Compile();
            var result = compiledExpression(openRequirement);

            // Assert
            Assert.True(result);
        }

        /// <summary>
        /// Tests that <see cref="FilterRequirementByIsOpen"/> returns false for closed requirements.
        /// </summary>
        [Fact]
        public void FilterRequirementByIsOpen_ShouldReturnFalseForClosedRequirements()
        {
            // Arrange
            var specification = new FilterRequirementByIsOpen();
            var closedRequirement = new Requirement { MappedState = Contracts.Requirements.States.MappedRequirementState.Completed, Name = "Closed Requirement" };

            // Act
            var compiledExpression = specification.Expression.Compile();
            var result = compiledExpression(closedRequirement);

            // Assert
            Assert.False(result);
        }

        /// <summary>
        /// Tests that <see cref="FilterRequirementByIsOpen"/> returns false for canceled requirements.
        /// </summary>
        [Fact]
        public void FilterRequirementByIsOpen_ShouldReturnFalseForCanceledRequirements()
        {
            // Arrange
            var specification = new FilterRequirementByIsOpen();
            var canceledRequirement = new Requirement { MappedState = Contracts.Requirements.States.MappedRequirementState.Canceled, Name = "Canceled Requirement" };

            // Act
            var compiledExpression = specification.Expression.Compile();
            var result = compiledExpression(canceledRequirement);

            // Assert
            Assert.False(result);
        }

        /// <summary>
        /// Tests that <see cref="FilterRequirementByIsOpen"/> returns true for draft requirements.
        /// </summary>
        [Fact]
        public void FilterRequirementByIsOpen_ShouldReturnTrueForDraftRequirements()
        {
            // Arrange
            var specification = new FilterRequirementByIsOpen();
            var draftRequirement = new Requirement { MappedState = Contracts.Requirements.States.MappedRequirementState.Draft, Name = "Draft Requirement" };

            // Act
            var compiledExpression = specification.Expression.Compile();
            var result = compiledExpression(draftRequirement);

            // Assert
            Assert.True(result);
        }

        /// <summary>
        /// Tests that <see cref="FilterRequirementByIsOpen"/> returns true for accepted requirements.
        /// </summary>
        [Fact]
        public void FilterRequirementByIsOpen_ShouldReturnTrueForAcceptedRequirements()
        {
            // Arrange
            var specification = new FilterRequirementByIsOpen();
            var acceptedRequirement = new Requirement { MappedState = Contracts.Requirements.States.MappedRequirementState.Accepted, Name = "Accepted Requirement" };

            // Act
            var compiledExpression = specification.Expression.Compile();
            var result = compiledExpression(acceptedRequirement);

            // Assert
            Assert.True(result);
        }

        /// <summary>
        /// Tests that <see cref="FilterRequirementByIsOpen"/> returns true for assigned requirements.
        /// </summary>
        [Fact]
        public void FilterRequirementByIsOpen_ShouldReturnTrueForAssignedRequirements()
        {
            // Arrange
            var specification = new FilterRequirementByIsOpen();
            var assignedRequirement = new Requirement { MappedState = Contracts.Requirements.States.MappedRequirementState.Assigned, Name = "Assigned Requirement" };

            // Act
            var compiledExpression = specification.Expression.Compile();
            var result = compiledExpression(assignedRequirement);

            // Assert
            Assert.True(result);
        }

        /// <summary>
        /// Tests that <see cref="FilterRequirementByIsOpen"/> returns true for reviewed requirements.
        /// </summary>
        [Fact]
        public void FilterRequirementByIsOpen_ShouldReturnTrueForReviewedRequirements()
        {
            // Arrange
            var specification = new FilterRequirementByIsOpen();
            var reviewedRequirement = new Requirement { MappedState = Contracts.Requirements.States.MappedRequirementState.Reviewed, Name = "Reviewed Requirement" };

            // Act
            var compiledExpression = specification.Expression.Compile();
            var result = compiledExpression(reviewedRequirement);

            // Assert
            Assert.True(result);
        }

        /// <summary>
        /// Tests that <see cref="FilterRequirementByIsOpen"/> returns true for delivered requirements.
        /// </summary>
        [Fact]
        public void FilterRequirementByIsOpen_ShouldReturnTrueForDeliveredRequirements()
        {
            // Arrange
            var specification = new FilterRequirementByIsOpen();
            var deliveredRequirement = new Requirement { MappedState = Contracts.Requirements.States.MappedRequirementState.Delivered, Name = "Delivered Requirement" };

            // Act
            var compiledExpression = specification.Expression.Compile();
            var result = compiledExpression(deliveredRequirement);

            // Assert
            Assert.True(result);
        }

        /// <summary>
        /// Tests that <see cref="FilterRequirementByIsOpen"/> returns true for other requirements.
        /// </summary>
        [Fact]
        public void FilterRequirementByIsOpen_ShouldReturnTrueForOtherRequirements()
        {
            // Arrange
            var specification = new FilterRequirementByIsOpen();
            var otherRequirement = new Requirement { MappedState = Contracts.Requirements.States.MappedRequirementState.Other, Name = "Other Requirement" };

            // Act
            var compiledExpression = specification.Expression.Compile();
            var result = compiledExpression(otherRequirement);

            // Assert
            Assert.True(result);
        }

        /// <summary>
        /// Tests that <see cref="FilterRequirementTestLinkByTest"/> returns true for matching test case IDs.
        /// </summary>
        [Fact]
        public void FilterRequirementTestLinkByTest_ShouldReturnTrueForMatchingTestCaseId()
        {
            // Arrange
            var specification = new FilterRequirementTestLinkByTest(123);
            var testLinkMatching = new RequirementTestLink { TestCaseId = 123, RequirementId = 1 };

            // Act
            var compiledExpression = specification.Expression.Compile();
            var result = compiledExpression(testLinkMatching);

            // Assert
            Assert.True(result);
        }

        /// <summary>
        /// Tests that <see cref="FilterRequirementTestLinkByTest"/> returns false for non-matching test case IDs.
        /// </summary>
        [Fact]
        public void FilterRequirementTestLinkByTest_ShouldReturnFalseForNonMatchingTestCaseId()
        {
            // Arrange
            var specification = new FilterRequirementTestLinkByTest(123);
            var testLinkNonMatching = new RequirementTestLink { TestCaseId = 456, RequirementId = 2 };

            // Act
            var compiledExpression = specification.Expression.Compile();
            var result = compiledExpression(testLinkNonMatching);

            // Assert
            Assert.False(result);
        }

        /// <summary>
        /// Tests that <see cref="FilterRequirementTestLinkByTest"/> handles zero test case ID.
        /// </summary>
        [Fact]
        public void FilterRequirementTestLinkByTest_ShouldHandleZeroTestCaseId()
        {
            // Arrange
            var specification = new FilterRequirementTestLinkByTest(0);
            var testLink = new RequirementTestLink { TestCaseId = 0, RequirementId = 1 };

            // Act
            var compiledExpression = specification.Expression.Compile();
            var result = compiledExpression(testLink);

            // Assert
            Assert.True(result);
        }

        /// <summary>
        /// Tests that <see cref="FilterRequirementTestLinkByTest"/> handles negative test case ID.
        /// </summary>
        [Fact]
        public void FilterRequirementTestLinkByTest_ShouldHandleNegativeTestCaseId()
        {
            // Arrange
            var specification = new FilterRequirementTestLinkByTest(-1);
            var testLink = new RequirementTestLink { TestCaseId = -1, RequirementId = 1 };

            // Act
            var compiledExpression = specification.Expression.Compile();
            var result = compiledExpression(testLink);

            // Assert
            Assert.True(result);
        }

        /// <summary>
        /// Tests that <see cref="FilterRequirementFoldersByText"/> returns true for matching folder names.
        /// </summary>
        [Fact]
        public void FilterRequirementFoldersByText_ShouldReturnTrueForMatchingText()
        {
            // Arrange
            var specification = new FilterRequirementFoldersByText("test");
            var folder = new RequirementSpecificationFolder { Name = "Test Folder" };

            // Act
            var compiledExpression = specification.Expression.Compile();
            var result = compiledExpression(folder);

            // Assert
            Assert.True(result);
        }

        /// <summary>
        /// Tests that <see cref="FilterRequirementFoldersByText"/> returns false for non-matching folder names.
        /// </summary>
        [Fact]
        public void FilterRequirementFoldersByText_ShouldReturnFalseForNonMatchingText()
        {
            // Arrange
            var specification = new FilterRequirementFoldersByText("test");
            var folder = new RequirementSpecificationFolder { Name = "Unrelated Folder" };

            // Act
            var compiledExpression = specification.Expression.Compile();
            var result = compiledExpression(folder);

            // Assert
            Assert.False(result);
        }

        /// <summary>
        /// Tests that <see cref="FilterRequirementFoldersByText"/> performs case-insensitive matching.
        /// </summary>
        [Fact]
        public void FilterRequirementFoldersByText_ShouldMatchCaseInsensitive()
        {
            // Arrange
            var specification = new FilterRequirementFoldersByText("TEST");
            var folder = new RequirementSpecificationFolder { Name = "test folder" };

            // Act
            var compiledExpression = specification.Expression.Compile();
            var result = compiledExpression(folder);

            // Assert
            Assert.True(result);
        }

        /// <summary>
        /// Tests that <see cref="FilterRequirementFoldersByText"/> handles empty search text.
        /// </summary>
        [Fact]
        public void FilterRequirementFoldersByText_ShouldMatchEmptyText()
        {
            // Arrange
            var specification = new FilterRequirementFoldersByText("");
            var folder = new RequirementSpecificationFolder { Name = "Any Folder" };

            // Act
            var compiledExpression = specification.Expression.Compile();
            var result = compiledExpression(folder);

            // Assert
            Assert.True(result);
        }


        /// <summary>
        /// Tests that <see cref="FilterRequirementFoldersByText"/> matches partial text.
        /// </summary>
        [Fact]
        public void FilterRequirementFoldersByText_ShouldMatchPartialText()
        {
            // Arrange
            var specification = new FilterRequirementFoldersByText("port");
            var folder = new RequirementSpecificationFolder { Name = "Important Folder" };

            // Act
            var compiledExpression = specification.Expression.Compile();
            var result = compiledExpression(folder);

            // Assert
            Assert.True(result);
        }
    }
}