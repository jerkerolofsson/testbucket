using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestBucket.Domain.IntegrationTests.Search
{
    [IntegrationTest]
    [EnrichedTest]
    [Component("Testing")]
    [FunctionalTest]
    public class SearchTests(ProjectFixture Fixture) : IClassFixture<ProjectFixture>
    {
        [Fact]
        [TestDescription("""
            Verifies that it is possible to filter on the milestone field when searching for test cases

            # Steps
            1. Add a test suite
            2. Add 3 test cases
            3. Assign 1.0, 2.0 and 3.0 milestones to each test respectively
            4. Search for test IDs with the query: "milestone:2.0"
            5. The test ID for the second test is returned
            """)]
        public async Task SearchTests_WithMilestoneFilter()
        {
            var suite = await Fixture.Tests.AddSuiteAsync();
            var test1 = await Fixture.Tests.AddAsync(suite);
            var test2 = await Fixture.Tests.AddAsync(suite);
            var test3 = await Fixture.Tests.AddAsync(suite);
            await Fixture.Tests.SetMilestoneAsync(test1, "1.0");
            await Fixture.Tests.SetMilestoneAsync(test2, "2.0");
            await Fixture.Tests.SetMilestoneAsync(test3, "3.0");

            // Act
            var ids = await Fixture.Tests.SearchTestIdsAsync("milestone:2.0");

            // Assert 
            Assert.Single(ids);
            Assert.Contains(test2.Id, ids);
        }

        [Fact]
        [TestDescription("""
            Verifies that it is possible to filter on the milestone field when searching for test cases

            # Steps
            1. Add a test suite
            2. Add two tests to the first suite
            3. Add a second test suite
            4. Add 3 test cases to the second suite
            5. Search for test IDs with the query: "testsuite-id:{secondSuite.Id}"
            6. The test ID for the all 3 tests returned
            """)]
        public async Task SearchTests_WithSuiteIdFilter()
        {
            var suiteA = await Fixture.Tests.AddSuiteAsync();
            var test1A = await Fixture.Tests.AddAsync(suiteA);
            var test2A = await Fixture.Tests.AddAsync(suiteA);


            var secondSuite = await Fixture.Tests.AddSuiteAsync();
            var test1 = await Fixture.Tests.AddAsync(secondSuite);
            var test2 = await Fixture.Tests.AddAsync(secondSuite);
            var test3 = await Fixture.Tests.AddAsync(secondSuite);
            await Fixture.Tests.SetMilestoneAsync(test1, "1.0");
            await Fixture.Tests.SetMilestoneAsync(test2, "2.0");
            await Fixture.Tests.SetMilestoneAsync(test3, "3.0");

            // Act
            var ids = await Fixture.Tests.SearchTestIdsAsync($"testsuite-id:{secondSuite.Id}");

            // Assert 
            Assert.Equal(3, ids.Count);
            Assert.Contains(test1.Id, ids);
            Assert.Contains(test2.Id, ids);
            Assert.Contains(test3.Id, ids);
        }

        [Fact]
        [TestDescription("""
            Verifies that it is possible to filter on the milestone field when searching for test cases

            # Steps
            1. Add a test suite
            2. Add a test case with a specific name
            3. Add 2 other test cases with other names
            4. Search for test IDs with name for the first test case
            5. The ID for the first test returned
            
            """)]
        public async Task SearchTests_WithNameFilter()
        {
            var suite = await Fixture.Tests.AddSuiteAsync();
            var test1 = await Fixture.Tests.AddAsync(suite);
            var test2 = await Fixture.Tests.AddAsync(suite);
            var test3 = await Fixture.Tests.AddAsync(suite);

            // Act
            var ids = await Fixture.Tests.SearchTestIdsAsync(test1.Name);

            // Assert 
            Assert.Single(ids);
            Assert.Contains(test1.Id, ids);
        }

        [Fact]
        [TestDescription("""
            Verifies that it is possible to filter on the milestone field when searching for test cases

            # Steps
            1. Set the time to 2025-05-21 00:00:00
            2. Add a test suite
            3. Add one test
            4. Set the time to 2025-05-21 05:00:00
            5. Search with query "testsuite-id:{suite.Id} since:1h"
            6. Only the ID of the second test was returned
            """)]
        public async Task SearchTests_WithSince1hFilter()
        {
            var time1 = new DateTimeOffset(2025, 5, 21, 0, 0, 0, TimeSpan.Zero);
            var time2 = new DateTimeOffset(2025, 5, 21, 5, 0, 0, TimeSpan.Zero);

            Fixture.App.TimeProvider.SetTime(time1);
            var suite = await Fixture.Tests.AddSuiteAsync();
            var test1 = await Fixture.Tests.AddAsync(suite);

            Fixture.App.TimeProvider.SetTime(time2);
            var test2 = await Fixture.Tests.AddAsync(suite);

            // Act
            var ids = await Fixture.Tests.SearchTestIdsAsync($"testsuite-id:{suite.Id} since:1h");

            // Assert 
            Assert.Equal(time1, test1.Created);
            Assert.Equal(time2, test2.Created);
            Assert.Single(ids);
            Assert.Contains(test2.Id, ids);
        }

        [Fact]
        [TestDescription("""
            Verifies that it is possible to filter on the milestone field when searching for test cases

            # Steps
            1. Set the time to 2025-05-21 00:00:00
            2. Add a test suite
            3. Add one test
            4. Set the time to 2025-05-23 05:00:00
            5. Search with query "testsuite-id:{suite.Id} from:2025-05-21 until:2025-05-22"
            6. Only the ID of the first test was returned
            """)]
        public async Task SearchTests_WithFromUntilFilter()
        {
            Fixture.App.TimeProvider.SetTime(new DateTimeOffset(2025, 5, 21, 0, 0, 0, TimeSpan.Zero));
            var suite = await Fixture.Tests.AddSuiteAsync();
            var test1 = await Fixture.Tests.AddAsync(suite);

            Fixture.App.TimeProvider.SetTime(new DateTimeOffset(2025, 5, 23, 5, 0, 0, TimeSpan.Zero));
            var test2 = await Fixture.Tests.AddAsync(suite);

            // Act
            var ids = await Fixture.Tests.SearchTestIdsAsync($"testsuite-id:{suite.Id} from:2025-05-21 until:2025-05-22");

            // Assert 
            Assert.Single(ids);
            Assert.Contains(test1.Id, ids);
        }
    }
}
