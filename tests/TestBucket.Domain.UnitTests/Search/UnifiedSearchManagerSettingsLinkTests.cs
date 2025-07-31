using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TestBucket.Contracts.Localization;
using TestBucket.Domain.Fields;
using TestBucket.Domain.Identity;
using TestBucket.Domain.Projects.Models;
using TestBucket.Domain.Requirements;
using TestBucket.Domain.Search;
using TestBucket.Domain.Search.Models;
using TestBucket.Domain.Settings;
using TestBucket.Domain.Settings.Models;
using TestBucket.Domain.Testing.Models;
using TestBucket.Domain.Testing.TestCases.Search;
using Xunit;

namespace TestBucket.Domain.UnitTests.Search
{
    /// <summary>
    /// Contains unit tests for the <see cref="UnifiedSearchManager"/> class, verifying the behavior of unified search functionality.
    /// </summary>
    [Feature("Search")]
    [UnitTest]
    [Component("Search")]
    [EnrichedTest]
    [FunctionalTest]
    public class UnifiedSearchManagerSettingsLinkTests
    {
        private readonly IAppLocalization _loc = Substitute.For<IAppLocalization>();

        private ClaimsPrincipal CreateValidUser(string tenantId, long? projectId = null)
        {
            return Impersonation.Impersonate(tenantId);
        }

        /// <summary>
        /// Verifies that the UnifiedSearchManager.SearchAsync method finds matches for link keywords for partial matches
        /// </summary>
        [Fact]
        public async Task SearchAsync_WithLinkMatchingKeywordPartialMatch_IsMatch()
        {
            // Arrange
            var mockTestCaseRepo = Substitute.For<ITestCaseRepository>();
            var mockFieldDefinitionManager = Substitute.For<IFieldDefinitionManager>();
            var mockRequirementRepo = Substitute.For<IRequirementRepository>();
            var settingsManager = new SettingsManager([], [new SettingsLink() { RelativeUrl = "/", Title = "Link1", Keywords = "appearance" }], _loc);

            var manager = new UnifiedSearchManager(mockTestCaseRepo, mockFieldDefinitionManager, mockRequirementRepo, settingsManager);

            var principal = CreateValidUser("tenant1");
            var testProject = new TestProject() { Name = "project", Slug = "project", ShortName = "pr" };
            var searchText = "ear";
            var cancellationToken = CancellationToken.None;

            // Act
            var results = await manager.SearchAsync(principal, testProject, searchText, cancellationToken);

            // Assert
            Assert.NotNull(results);
            Assert.Single(results);
            Assert.Equal("Link1", results[0].Text);
        }

        /// <summary>
        /// Verifies that the UnifiedSearchManager.SearchAsync method finds matches for link description for partial matches
        /// </summary>
        [Fact]
        public async Task SearchAsync_WithLinkMatchingDescriptionPartialMatch_IsMatch()
        {
            // Arrange
            var mockTestCaseRepo = Substitute.For<ITestCaseRepository>();
            var mockFieldDefinitionManager = Substitute.For<IFieldDefinitionManager>();
            var mockRequirementRepo = Substitute.For<IRequirementRepository>();
            var settingsManager = new SettingsManager([], [new SettingsLink() { RelativeUrl = "/", Title = "Link1", Keywords = "appearance", Description="The weather is 30 degrees" }], _loc);

            var manager = new UnifiedSearchManager(mockTestCaseRepo, mockFieldDefinitionManager, mockRequirementRepo, settingsManager);

            var principal = CreateValidUser("tenant1");
            var testProject = new TestProject() { Name = "project", Slug = "project", ShortName = "pr" };
            var searchText = "weather";
            var cancellationToken = CancellationToken.None;

            // Act
            var results = await manager.SearchAsync(principal, testProject, searchText, cancellationToken);

            // Assert
            Assert.NotNull(results);
            Assert.Single(results);
            Assert.Equal("Link1", results[0].Text);
        }


        /// <summary>
        /// Verifies that the UnifiedSearchManager.SearchAsync method finds matches for link title for partial matches
        /// </summary>
        [Fact]
        public async Task SearchAsync_WithLinkMatchingTitlePartialMatch_IsMatch()
        {
            // Arrange
            var mockTestCaseRepo = Substitute.For<ITestCaseRepository>();
            var mockFieldDefinitionManager = Substitute.For<IFieldDefinitionManager>();
            var mockRequirementRepo = Substitute.For<IRequirementRepository>();
            var settingsManager = new SettingsManager([], [new SettingsLink() { RelativeUrl = "/", Title = "Link1", Keywords = "appearance" }], _loc);

            var manager = new UnifiedSearchManager(mockTestCaseRepo, mockFieldDefinitionManager, mockRequirementRepo, settingsManager);

            var principal = CreateValidUser("tenant1");
            var testProject = new TestProject() { Name = "project", Slug = "project", ShortName = "pr" };
            var searchText = "ink";
            var cancellationToken = CancellationToken.None;

            // Act
            var results = await manager.SearchAsync(principal, testProject, searchText, cancellationToken);

            // Assert
            Assert.NotNull(results);
            Assert.Single(results);
            Assert.Equal("Link1", results[0].Text);
        }

        /// <summary>
        /// Verifies that the UnifiedSearchManager.SearchAsync method finds matches for link keywords
        /// </summary>
        [Fact]
        public async Task SearchAsync_WithLinkMatchingKeywordExactMatch_IsMatch()
        {
            // Arrange
            var mockTestCaseRepo = Substitute.For<ITestCaseRepository>();
            var mockFieldDefinitionManager = Substitute.For<IFieldDefinitionManager>();
            var mockRequirementRepo = Substitute.For<IRequirementRepository>();
            var settingsManager = new SettingsManager([], [new SettingsLink() { RelativeUrl = "/", Title = "Link1", Keywords = "appearance" }], _loc);

            var manager = new UnifiedSearchManager(mockTestCaseRepo, mockFieldDefinitionManager, mockRequirementRepo, settingsManager);

            var principal = CreateValidUser("tenant1");
            var testProject = new TestProject() { Name = "project", Slug = "project", ShortName = "pr" };
            var searchText = "appearance";
            var cancellationToken = CancellationToken.None;

            // Act
            var results = await manager.SearchAsync(principal, testProject, searchText, cancellationToken);

            // Assert
            Assert.NotNull(results);
            Assert.Single(results);
            Assert.Equal("Link1", results[0].Text);
        }

        /// <summary>
        /// Verifies that the UnifiedSearchManager.SearchAsync method returns an empty list when there are no matches in the links
        /// </summary>
        [Fact]
        public async Task SearchAsync_WithNoLinkMatches_NothingMatches()
        {
            // Arrange
            var mockTestCaseRepo = Substitute.For<ITestCaseRepository>();
            var mockFieldDefinitionManager = Substitute.For<IFieldDefinitionManager>();
            var mockRequirementRepo = Substitute.For<IRequirementRepository>();
            var settingsManager = new SettingsManager([], [new SettingsLink() { RelativeUrl = "/", Title = "Link1", Keywords = "appearance" }], _loc);

            var manager = new UnifiedSearchManager(mockTestCaseRepo, mockFieldDefinitionManager, mockRequirementRepo, settingsManager);

            var principal = CreateValidUser("tenant1");
            var testProject = new TestProject() { Name = "project", Slug = "project", ShortName = "pr" };
            var searchText = "color";
            var cancellationToken = CancellationToken.None;

            // Act
            var results = await manager.SearchAsync(principal, testProject, searchText, cancellationToken);

            // Assert
            Assert.NotNull(results);
            Assert.Empty(results);
        }
    }
}
