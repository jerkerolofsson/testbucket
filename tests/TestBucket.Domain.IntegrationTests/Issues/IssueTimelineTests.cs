using TestBucket.Contracts.Issues.States;

namespace TestBucket.Domain.IntegrationTests.Issues
{
    [IntegrationTest]
    [FunctionalTest]
    [Feature("Issues")]
    [Component("Issues")]
    public class IssueTimelineTests(ProjectFixture Fixture) : IClassFixture<ProjectFixture>
    {
        /// <summary>
        /// Verifies that a comment is added to the issue timeline when an assignee is added
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task AddIssue_InitialStateIsOpen()
        {
            var issue = await Fixture.Issues.AddIssueAsync();

            // Act
            issue.AssignedTo = "bob.marley@google.com";
            await Fixture.Issues.UpdateIssueAsync(issue);

            // Assert
            var issueWithComments = await Fixture.Issues.GetIssueByIdAsync(issue.Id);
            Assert.NotNull(issueWithComments?.Comments);
            Assert.NotEmpty(issueWithComments.Comments);
            var timelineEvent = issueWithComments.Comments.FirstOrDefault(x=>x.LoggedAction == "assignedto");
            Assert.NotNull(timelineEvent);
            Assert.Equal(issue.AssignedTo, timelineEvent.LoggedActionArgument);
        }
    }
}
