﻿using System;
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
    public class IssueStateTests(ProjectFixture Fixture) : IClassFixture<ProjectFixture>
    {
        [Fact]
        [TestDescription("""
            Verifies that when an issue is changed from one state to Closed, the Closed timestamp is updated

            # Steps
            1. Add an issue
            2. Change the state to "Closed"
            3. ClosedTimestamp is set
            """)]
        public async Task ChangeState_ToClosed_ClosedTimestampIsUpdated()
        {
            var issue = await Fixture.Issues.AddIssueAsync();

            // Act
            issue.State = "Closed";
            issue.MappedState = MappedIssueState.Closed;
            await Fixture.Issues.UpdateIssueAsync(issue);
            
            var result = await Fixture.Issues.GetIssueByIdAsync(issue.Id);
            Assert.NotNull(result);
            Assert.NotNull(result.Closed);
        }
    }
}
