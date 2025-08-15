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
    public class CsvSerializerQcaseTests
    {
        /// <summary>
        /// Verifies that the deserialized description contains the Markdown header "## Description".
        /// </summary>
        [Fact]
        public async Task Deserialize_QcaseValidCsv_DescriptionContainsMarkdownHeader()
        {
            // Arrange
            using var source = File.OpenRead("./Csv/TestData/qase.csv");
            var reader = new CsvRepoSerializer();

            // Act
            var repo = await reader.DeserializeAsync(source);

            // Assert
            Assert.Contains("## Description", repo.TestCases[0].Description);
        }

        /// <summary>
        /// Verifies that the deserialized description contains the Markdown header "## Pre-condition".
        /// </summary>
        [Fact]
        public async Task Deserialize_QcaseValidCsv_DescriptionContainsMarkdownPreconditionHeader()
        {
            // Arrange
            using var source = File.OpenRead("./Csv/TestData/qase.csv");
            var reader = new CsvRepoSerializer();

            // Act
            var repo = await reader.DeserializeAsync(source);

            // Assert
            Assert.Contains("## Pre-condition", repo.TestCases[0].Description);
        }

        /// <summary>
        /// Verifies that the deserialized description does not contain the Markdown header "## Pre-condition" when the precondition is empty.
        /// </summary>
        [Fact]
        public async Task Deserialize_QcaseValidCsvAndEmptyPrecondition_DescriptionDoesNotContainsMarkdownPreconditionHeader()
        {
            // Arrange
            using var source = File.OpenRead("./Csv/TestData/qase.csv");
            var reader = new CsvRepoSerializer();

            // Act
            var repo = await reader.DeserializeAsync(source);

            // Assert
            Assert.True(repo.TestCases.Count >= 1);
            Assert.DoesNotContain("## Pre-condition", repo.TestCases[1].Description);
        }

        /// <summary>
        /// Verifies that the deserialized description contains the expected steps result text.
        /// </summary>
        [Fact]
        public async Task Deserialize_QcaseValidCsv_DescriptionContainsStepsResult()
        {
            // Arrange
            using var source = File.OpenRead("./Csv/TestData/qase.csv");
            var reader = new CsvRepoSerializer();

            // Act
            var repo = await reader.DeserializeAsync(source);

            // Assert
            Assert.Contains("UI should be perfect", repo.TestCases[0].Description);
        }

        /// <summary>
        /// Verifies that the deserialized suite and case counts are correct and that the first and last test case names match expectations.
        /// </summary>
        [Fact]
        public async Task Deserialize_QcaseValidCsv_SuiteAndCaseCountCorrect()
        {
            // Arrange
            using var source = File.OpenRead("./Csv/TestData/qase.csv");
            var reader = new CsvRepoSerializer();

            // Act
            var repo = await reader.DeserializeAsync(source);

            // Assert
            Assert.Equal(3, repo.TestSuites.Count);
            Assert.Equal(15, repo.TestCases.Count);
            Assert.Equal("Check all the text boxes, radio buttons, buttons, etc", repo.TestCases[0].TestCaseName);
            Assert.Equal("Verify Forgot Password sends a forgot password link", repo.TestCases[14].TestCaseName);
        }
    }
}