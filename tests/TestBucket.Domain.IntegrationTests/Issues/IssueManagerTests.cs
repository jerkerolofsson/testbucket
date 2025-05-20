using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using TestBucket.Contracts.Issues.States;
using TestBucket.Domain.Fields;
using TestBucket.Domain.Issues.Models;
using TestBucket.Domain.Milestones;
using TestBucket.Traits.Core;

namespace TestBucket.Domain.IntegrationTests.Issues
{
    [IntegrationTest]
    [FunctionalTest]
    [Feature("Issues")]
    [Component("Issues")]
    public class IssueManagerTests(ProjectFixture Fixture) : IClassFixture<ProjectFixture>
    {
        [Fact]
        [TestDescription("""
            Verifies that when adding a new issue the default state is Open

            # Steps
            1. Add an issue
            2. State is "Open"
            """)]
        public async Task AddIssue_InitialStateIsOpen()
        {
            var issue = await Fixture.Issues.AddIssueAsync();

            var createdIssue = await Fixture.Issues.GetIssueByIdAsync(issue.Id);
            Assert.NotNull(createdIssue);
            Assert.Equal(IssueStates.Open, createdIssue.State);
            Assert.Equal(MappedIssueState.Open, createdIssue.MappedState);
        }


        [Fact]
        [TestDescription("""
            Verifies that when adding a new issue the created and modified timestamp is set

            # Steps
            1. Add an issue
            2. Created timestamp set
            3. Modified timestamp set
            """)]
        public async Task AddIssue_ModifiedSet()
        {
            var issue = await Fixture.Issues.AddIssueAsync();

            var createdIssue = await Fixture.Issues.GetIssueByIdAsync(issue.Id);
            Assert.NotNull(createdIssue);
            Assert.NotEqual(DateTimeOffset.MinValue, createdIssue.Created);
            Assert.NotEqual(DateTimeOffset.MinValue, createdIssue.Modified);
        }

        [Fact]
        [TestDescription("""
            Verifies that when adding a new issue the closed timestamp is not set

            # Steps
            1. Add an issue
            2. Closed timestamp is null
            """)]
        public async Task AddIssue_ClosedTimestampIsNull()
        {
            var issue = await Fixture.Issues.AddIssueAsync();

            var createdIssue = await Fixture.Issues.GetIssueByIdAsync(issue.Id);
            Assert.NotNull(createdIssue);
            Assert.Null(createdIssue.Closed);
        }

        [Fact]
        [TestDescription("""
            Verifies that seaching for an issue with "text based" search, we can filter by state

            # Steps
            1. Add an issue with state Open
            2. Add another issue with state Accepted
            3. Search for issues in state Accepted: One issue is returned.
            """)]
        public async Task SearchForIssues()
        {
            var issue1 = await Fixture.Issues.AddIssueAsync();
            var issue2 = await Fixture.Issues.AddIssueAsync();
            issue2.Title = "MyTitle";
            issue2.State = IssueStates.Accepted;
            issue2.MappedState = MappedIssueState.Accepted;

            await Fixture.Issues.UpdateIssueAsync(issue2);

            // Act
            var result = await Fixture.Issues.SearchAsync("state:Accepted", 0, 10);
            Assert.Equal(1, result.TotalCount);
            Assert.Single(result.Items);
            Assert.Equal("MyTitle", result.Items[0].Title);
        }
    }
}
