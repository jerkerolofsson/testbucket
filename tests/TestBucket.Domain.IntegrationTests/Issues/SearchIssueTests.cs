using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using TestBucket.Contracts.Issues.States;
using TestBucket.Domain.Fields;
using TestBucket.Domain.Issues.Models;
using TestBucket.Domain.Issues.Search;
using TestBucket.Domain.Milestones;
using TestBucket.Domain.Projects.Models;
using TestBucket.Traits.Core;

namespace TestBucket.Domain.IntegrationTests.Issues
{
    /// <summary>
    /// Search for issue tests
    /// </summary>
    /// <param name="Fixture"></param>
    [IntegrationTest]
    [FunctionalTest]
    [Feature("Search")]
    [Component("Issues")]
    public class SearchIssueTests(ProjectFixture Fixture) : IClassFixture<ProjectFixture>
    {
        /// <summary>
        /// Verifies that an issue can be found when searching for the state Closed after an issue has been Closed
        ///
        /// # Steps
        /// 1. Add two issues with the same title 
        /// 2. Close the first issue
        /// 3. Search for the issue using State="Closed" and title : One issue found.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task SearchIssue_WithStateClosed_AfterClosed_OneIssueFound()
        {
            var issue1 = await Fixture.Issues.AddIssueAsync();
            issue1.Title = Guid.NewGuid().ToString();
            await Fixture.Issues.UpdateIssueAsync(issue1);

            await Fixture.Issues.CloseIssueAsync(issue1);

            var issue2 = await Fixture.Issues.AddIssueAsync();
            issue2.Title = issue1.Title;
            await Fixture.Issues.UpdateIssueAsync(issue2);

            // Act
            var result = await Fixture.Issues.SearchAsync(new SearchIssueQuery() { State = "Closed", ProjectId = Fixture.ProjectId, Text = issue1.Title }, 0, 10);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result.Items);
            Assert.Equal(issue1.Title, result.Items[0].Title);
        }

        /// <summary>
        /// Verifies that an issue can be found when searching for the state Closed
        ///
        /// # Steps
        /// 1. Add two issues with title "SearchIssue_WithState"
        /// 2. Change the state to "Closed" for one of them
        /// 3. Search for the issue using State="Closed" and title="SearchIssue_WithState": One issue found.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task SearchIssue_WithState()
        {
            var issue1 = await Fixture.Issues.AddIssueAsync();
            issue1.Title = "SearchIssue_WithState";
            issue1.State = "Closed";
            issue1.MappedState = MappedIssueState.Closed;
            await Fixture.Issues.UpdateIssueAsync(issue1);

            var issue2 = await Fixture.Issues.AddIssueAsync();
            issue2.Title = "SearchIssue_WithState";
            issue2.State = "Open";
            issue2.MappedState = MappedIssueState.Open;
            await Fixture.Issues.UpdateIssueAsync(issue2);

            // Act
            var result = await Fixture.Issues.SearchAsync(new SearchIssueQuery() { State = "Closed", ProjectId = Fixture.ProjectId, Text = issue1.Title }, 0, 10);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result.Items);
            Assert.Equal(issue1.Title, result.Items[0].Title);
        }

        /// <summary>
        /// Verifies that an issue can be found when searching for the state Closed
        ///
        /// # Steps
        /// 1. Add two issues with title "SearchIssue_WithMappedState"
        /// 2. Change the state to "Closed" for one of them
        /// 3. Search for the issue using State="Closed" and title="SearchIssue_WithMappedState": One issue found.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task SearchIssue_WithMappedState()
        {
            var issue1 = await Fixture.Issues.AddIssueAsync();
            issue1.Title = "SearchIssue_WithMappedState";
            issue1.State = "Closed";
            issue1.MappedState = MappedIssueState.Closed;
            await Fixture.Issues.UpdateIssueAsync(issue1);

            var issue2 = await Fixture.Issues.AddIssueAsync();
            issue2.Title = "SearchIssue_WithMappedState";
            issue2.State = "Open";
            issue2.MappedState = MappedIssueState.Open;
            await Fixture.Issues.UpdateIssueAsync(issue2);

            // Act
            var result = await Fixture.Issues.SearchAsync(new SearchIssueQuery() { MappedState = MappedIssueState.Closed, ProjectId = Fixture.ProjectId, Text = issue1.Title }, 0, 10);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result.Items);
            Assert.Equal(issue1.Title, result.Items[0].Title);
        }

        /// <summary>
        /// Verifies that an issue can be found when searching for the state Closed
        ///
        /// # Steps
        /// 1. Add an external system
        /// 2. Add two issues with ExternalId 1 and 2 respectively
        /// 3. Search for the issue using ExternalId="1" and ExternalSystem matching the external system added: One issue found.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task FindLocalIssueFromExternalAsync_WithExternalId()
        {
            var externalSystem = new ExternalSystem() { Name = "external-issue-system" };
            await Fixture.AddIntegrationAsync(externalSystem);

            var issue1 = await Fixture.Issues.AddIssueAsync();
            issue1.Title = Guid.NewGuid().ToString();
            issue1.ExternalId = "1";
            issue1.ExternalSystemId = externalSystem.Id;
            await Fixture.Issues.UpdateIssueAsync(issue1);

            var issue2 = await Fixture.Issues.AddIssueAsync();
            issue2.ExternalId = "2";
            issue2.ExternalSystemId = externalSystem.Id;
            await Fixture.Issues.UpdateIssueAsync(issue2);

            // Act
            var result = await Fixture.Issues.FindLocalIssueFromExternalAsync(Fixture.ProjectId, externalSystem.Id, issue1.ExternalId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(issue1.Title, result.Title);
        }
    }
}
