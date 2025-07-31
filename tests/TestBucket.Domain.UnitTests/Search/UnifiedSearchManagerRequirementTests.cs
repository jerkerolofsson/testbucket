using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using NSubstitute;

using TestBucket.Contracts.Localization;
using TestBucket.Domain.Fields;
using TestBucket.Domain.Identity;
using TestBucket.Domain.Projects.Models;
using TestBucket.Domain.Requirements;
using TestBucket.Domain.Requirements.Models;
using TestBucket.Domain.Requirements.Specifications.Requirements;
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
    /// Contains unit tests for the <see cref="UnifiedSearchManager"/> class, verifying the behavior of unified search functionality related
    /// to requirements.
    /// </summary>
    [Feature("Search")]
    [UnitTest]
    [Component("Search")]
    [EnrichedTest]
    public class UnifiedSearchManagerRequirementTests
    {
        private readonly IAppLocalization _loc = Substitute.For<IAppLocalization>();

        private ClaimsPrincipal CreateValidUser(string tenantId, long? projectId = null)
        {
            return Impersonation.Impersonate(tenantId);
        }

        /// <summary>
        /// Verifies that the UnifiedSearchManager.SearchAsync uses the tenantid from the ClaimsPrincipal 
        /// when searching for requirements
        /// </summary>
        [Fact]
        [SecurityTest]
        public async Task SearchAsync_WithRequirements_TenantIdFromPrincipalUsedForRequirementSearch()
        {
            // Arrange
            var mockTestCaseRepo = Substitute.For<ITestCaseRepository>();
            var mockFieldDefinitionManager = Substitute.For<IFieldDefinitionManager>();
            var mockRequirementRepo = Substitute.For<IRequirementRepository>();

            var settingsManager = new SettingsManager([], [], _loc);

            var manager = new UnifiedSearchManager(mockTestCaseRepo, mockFieldDefinitionManager, mockRequirementRepo, settingsManager);

            var principal = CreateValidUser("tenant1");
            var testProject = new TestProject() { Name = "project", Slug = "project", ShortName = "pr" };
            var searchText = "TheSearchString";
            var cancellationToken = CancellationToken.None;

            // Act
            await manager.SearchAsync(principal, testProject, searchText, cancellationToken);

            // Assert
            await mockRequirementRepo.Received(1).SearchRequirementsAsync(
                  Arg.Is<IEnumerable<FilterSpecification<Requirement>>>(filters =>
                      filters.OfType<FilterByTenant<Requirement>>().Any(filter => filter.TenantId == "tenant1")), Arg.Any<int>(), Arg.Any<int>());
        }

        /// <summary>
        /// Verifies that the UnifiedSearchManager.SearchAsync uses the tenantid from the ClaimsPrincipal 
        /// when searching for requirements
        /// </summary>
        [Fact]
        [FunctionalTest]
        public async Task SearchAsync_WithRequirements_SearchTextUsedForRequirementSearch()
        {
            // Arrange
            var mockTestCaseRepo = Substitute.For<ITestCaseRepository>();
            var mockFieldDefinitionManager = Substitute.For<IFieldDefinitionManager>();
            var mockRequirementRepo = Substitute.For<IRequirementRepository>();

            var settingsManager = new SettingsManager([], [], _loc);

            var manager = new UnifiedSearchManager(mockTestCaseRepo, mockFieldDefinitionManager, mockRequirementRepo, settingsManager);

            var principal = CreateValidUser("tenant1");
            var testProject = new TestProject() { Name = "project", Slug = "project", ShortName = "pr" };
            var searchText = "TheSearchString";
            var cancellationToken = CancellationToken.None;

            // Act
            await manager.SearchAsync(principal, testProject, searchText, cancellationToken);

            // Assert
            await mockRequirementRepo.Received(1).SearchRequirementsAsync(
                  Arg.Is<IEnumerable<FilterSpecification<Requirement>>>(filters =>
                      filters.OfType<FilterRequirementByText>().Any(filter => filter.Text == "thesearchstring")), Arg.Any<int>(), Arg.Any<int>());
        }


        /// <summary>
        /// Verifies that the UnifiedSearchManager.SearchAsync adds a filter for the specified project
        /// when searching for requirements
        /// </summary>
        [Fact]
        [FunctionalTest]
        public async Task SearchAsync_WithRequirements_SearchQueryFilteredByProject()
        {
            // Arrange
            var mockTestCaseRepo = Substitute.For<ITestCaseRepository>();
            var mockFieldDefinitionManager = Substitute.For<IFieldDefinitionManager>();
            var mockRequirementRepo = Substitute.For<IRequirementRepository>();

            var settingsManager = new SettingsManager([], [], _loc);

            var manager = new UnifiedSearchManager(mockTestCaseRepo, mockFieldDefinitionManager, mockRequirementRepo, settingsManager);

            var principal = CreateValidUser("tenant1");
            var testProject = new TestProject() { Name = "project", Slug = "project", ShortName = "pr", Id = 12 };
            var searchText = "TheSearchString";
            var cancellationToken = CancellationToken.None;

            // Act
            await manager.SearchAsync(principal, testProject, searchText, cancellationToken);

            // Assert
            await mockRequirementRepo.Received(1).SearchRequirementsAsync(
                  Arg.Is<IEnumerable<FilterSpecification<Requirement>>>(filters =>
                      filters.OfType<FilterByProject<Requirement>>().Any(filter => filter.Id == 12)), Arg.Any<int>(), Arg.Any<int>());
        }
    }
}
