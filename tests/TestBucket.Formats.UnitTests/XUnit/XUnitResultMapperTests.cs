using TestBucket.Formats.XUnit;
using TestBucket.Traits.Xunit;

namespace TestBucket.Formats.UnitTests.XUnit
{
    /// <summary>
    /// Unit tests for <see cref="XUnitResultMapper"/> to verify correct mapping between <see cref="TestResult"/> values and xUnit result strings.
    /// </summary>
    [UnitTest]
    [FunctionalTest]
    [Component("Test Formats")]
    [Feature("Import Test Results")]
    [EnrichedTest]
    public class XUnitResultMapperTests
    {
        /// <summary>
        /// Verifies that XUnitResultMapper.Map returns the correct xUnit result string for each TestResult value.
        /// 
        /// # Steps
        /// - Passes a TestResult value (or null) to XUnitResultMapper.Map.
        /// - Checks that the returned string matches the expected xUnit result string ("Pass", "Fail", or "Skip").
        /// - Ensures that only TestResult.Passed and TestResult.Failed map to "Pass" and "Fail" respectively; all other values (including null) map to "Skip".
        /// </summary>
        /// <param name="input">The TestResult value to map, or null.</param>
        /// <param name="expected">The expected xUnit result string.</param>
        [Theory]
        [InlineData(TestResult.Passed, "Pass")]
        [InlineData(TestResult.Failed, "Fail")]
        [InlineData(TestResult.Skipped, "Skip")]
        [InlineData(TestResult.Blocked, "Skip")]
        [InlineData(TestResult.Error, "Skip")]
        [InlineData(TestResult.Crashed, "Skip")]
        [InlineData(TestResult.Assert, "Skip")]
        [InlineData(TestResult.Hang, "Skip")]
        [InlineData(TestResult.Inconclusive, "Skip")]
        [InlineData(TestResult.NoRun, "Skip")]
        [InlineData(TestResult.Other, "Skip")]
        [InlineData(null, "Skip")]
        public void Map_TestResult_ToString_ReturnsExpectedString(TestResult? input, string expected)
        {
            var result = XUnitResultMapper.Map(input);
            Assert.Equal(expected, result);
        }

        /// <summary>
        /// Verifies that XUnitResultMapper.Map returns the correct "TestResult" value for each xUnit result string.
        /// </summary>
        /// <param name="input">The xUnit result string to map.</param>
        /// <param name="expected">The expected <see cref="TestResult"/> value.</param>
        [Theory]
        [InlineData("Pass", TestResult.Passed)]
        [InlineData("Fail", TestResult.Failed)]
        [InlineData("Skip", TestResult.Skipped)]
        [InlineData("Unknown", TestResult.Skipped)]
        [InlineData("", TestResult.Skipped)]
        public void Map_String_ToTestResult_ReturnsExpectedEnum(string input, TestResult expected)
        {
            var result = XUnitResultMapper.Map(input);
            Assert.Equal(expected, result);
        }
    }
}