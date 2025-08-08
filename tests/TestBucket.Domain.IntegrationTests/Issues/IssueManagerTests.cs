using TestBucket.Contracts.Issues.States;

namespace TestBucket.Domain.IntegrationTests.Issues
{
    /// <summary>
    /// Testsa for issue manager
    /// </summary>
    /// <param name="Fixture"></param>
    [IntegrationTest]
    [FunctionalTest]
    [Feature("Issues")]
    [Component("Issues")]
    public class IssueManagerTests(ProjectFixture Fixture) : IClassFixture<ProjectFixture>
    {
        /// <summary>
        /// Verifies that when adding a new issue the default state is Open
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task AddIssue_InitialStateIsOpen()
        {
            var issue = await Fixture.Issues.AddIssueAsync();

            var createdIssue = await Fixture.Issues.GetIssueByIdAsync(issue.Id);
            Assert.NotNull(createdIssue);
            Assert.Equal(IssueStates.Open, createdIssue.State);
            Assert.Equal(MappedIssueState.Open, createdIssue.MappedState);
        }

        /// <summary>
        /// Verifies that when adding a new issue the created and modified timestamp is set
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task AddIssue_ModifiedSet()
        {
            var issue = await Fixture.Issues.AddIssueAsync();

            var createdIssue = await Fixture.Issues.GetIssueByIdAsync(issue.Id);
            Assert.NotNull(createdIssue);
            Assert.NotEqual(DateTimeOffset.MinValue, createdIssue.Created);
            Assert.NotEqual(DateTimeOffset.MinValue, createdIssue.Modified);
        }

        /// <summary>
        /// Verifies when adding a new **issue** the Closed timestamp is null.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task AddIssue_ClosedTimestampIsNull()
        {
            var issue = await Fixture.Issues.AddIssueAsync();

            var createdIssue = await Fixture.Issues.GetIssueByIdAsync(issue.Id);
            Assert.NotNull(createdIssue);
            Assert.Null(createdIssue.Closed);
        }

        /// <summary>
        /// Verifies that seaching for an issue with "text based" search, we can filter by state
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task SearchForIssues()
        {
            await Fixture.Issues.AddIssueAsync();
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
