using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestBucket.Formats.Csv;
using TestBucket.Traits.Xunit;

namespace TestBucket.Formats.UnitTests.Csv
{
    /// <summary>
    /// Contains unit tests for <see cref="CsvRepoSerializer"/> focusing on Qcase CSV data.
    /// </summary>
    [UnitTest]
    [EnrichedTest]
    [Component("Test Formats")]
    [Feature("Import Tests")]
    [FunctionalTest]
    public class CsvSerializerTestRailTests
    {
        /// <summary>
        /// Verifies that the deserialized tests are found
        /// </summary>
        [Fact]
        public async Task Deserialize_TestRailValidCsv_TestsFound()
        {
            // Arrange
            using var source = File.OpenRead("./Csv/TestData/TestRail.csv");
            var reader = new CsvRepoSerializer();

            // Act
            var repo = await reader.DeserializeAsync(source);

            // Assert
            Assert.Contains(repo.TestCases, x => x.TestCaseName == "Check user should Register by filling all the required fields");
            Assert.Contains(repo.TestCases, x => x.TestCaseName == "Verify if the password required rules are not satisfied in the password");
        }

        /// <summary>
        /// Verifies that the section is mapped to path
        /// </summary>
        [Fact]
        public async Task Deserialize_TestRailValidCsv_SectionIsMappedToPath()
        {
            // Arrange
            using var source = File.OpenRead("./Csv/TestData/TestRail.csv");
            var reader = new CsvRepoSerializer();

            // Act
            var repo = await reader.DeserializeAsync(source);

            // Assert
            var test = repo.TestCases.FirstOrDefault(x => x.TestCaseName == "Check user should Register by filling all the required fields");
            Assert.NotNull(test);
            Assert.Equal("registration", test.Path);
        }
    }
}