namespace TestBucket.Domain.IntegrationTests.Search
{
    /// <summary>
    /// Tests related to searching for tests
    /// </summary>
    /// <param name="Fixture"></param>
    [IntegrationTest]
    [EnrichedTest]
    [Component("Testing")]
    [FunctionalTest]
    public class SearchTests(ProjectFixture Fixture) : IClassFixture<ProjectFixture>
    {
        /// <summary>
        /// Verfies that a test can be found by searching for a milestone using field-syntax:
        /// field:value
        /// 
        /// Example
        /// milestone:1.0
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task SearchTests_WithMilestoneFilter()
        {
            var suite = await Fixture.Tests.AddSuiteAsync();
            var test1 = await Fixture.Tests.AddAsync(suite);
            var test2 = await Fixture.Tests.AddAsync(suite);
            var test3 = await Fixture.Tests.AddAsync(suite);
            await Fixture.Tests.SetMilestoneAsync(test1, "10.0");
            await Fixture.Tests.SetMilestoneAsync(test2, "20.0");
            await Fixture.Tests.SetMilestoneAsync(test3, "30.0");

            // Act
            var ids = await Fixture.Tests.SearchTestIdsAsync("milestone:20.0");

            // Assert 
            Assert.Single(ids);
            Assert.Contains(test2.Id, ids);
        }

        /// <summary>
        /// Verifies that it is possible to find tests by filtering on test suite id
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task SearchTests_WithSuiteIdFilter()
        {
            var suiteA = await Fixture.Tests.AddSuiteAsync();
            await Fixture.Tests.AddAsync(suiteA);
            await Fixture.Tests.AddAsync(suiteA);


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

        /// <summary>
        /// Verifies that it is possible to find a test by searching for its name
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task SearchTests_WithNameFilter()
        {
            var suite = await Fixture.Tests.AddSuiteAsync();
            var test1 = await Fixture.Tests.AddAsync(suite);
            await Fixture.Tests.AddAsync(suite);
            await Fixture.Tests.AddAsync(suite);

            // Act
            var ids = await Fixture.Tests.SearchTestIdsAsync(test1.Name);

            // Assert 
            Assert.Single(ids);
            Assert.Contains(test1.Id, ids);
        }

        /// <summary>
        /// Verifies that it is possible to filter using the sunce-syntax
        /// </summary>
        /// <returns></returns>
        [Fact]
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

        /// <summary>
        /// Verifies that it is possible to filter on a test suite and dates
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task SearchTests_WithFromUntilFilter()
        {
            Fixture.App.TimeProvider.SetTime(new DateTimeOffset(2025, 5, 21, 0, 0, 0, TimeSpan.Zero));
            var suite = await Fixture.Tests.AddSuiteAsync();
            var test1 = await Fixture.Tests.AddAsync(suite);

            Fixture.App.TimeProvider.SetTime(new DateTimeOffset(2025, 5, 23, 5, 0, 0, TimeSpan.Zero));
            await Fixture.Tests.AddAsync(suite);

            // Act
            var ids = await Fixture.Tests.SearchTestIdsAsync($"testsuite-id:{suite.Id} from:2025-05-21 until:2025-05-22");

            // Assert 
            Assert.Single(ids);
            Assert.Contains(test1.Id, ids);
        }
    }
}
