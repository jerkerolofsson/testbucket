using TestBucket.Formats.Ctrf;
using TestBucket.Traits.Xunit;

namespace TestBucket.Formats.UnitTests.Ctrf
{
    /// <summary>
    /// Contains unit tests for mapping between <see cref="TestResult"/> values and CTRF status strings.
    /// </summary>
    [UnitTest]
    [FunctionalTest]
    [Component("Test Result Formats")]
    [Feature("Import Test Results")]
    [EnrichedTest]
    public class CtrfResultMappingTests
    {
        /// <summary>
        /// Verifies that <see cref="CtrfResultMapping.GetCtrfStatusFromTestResult(TestResult)"/> returns the correct CTRF status string for a given <see cref="TestResult"/>.
        /// </summary>
        /// <param name="result">The <see cref="TestResult"/> value to map.</param>
        /// <param name="expectedStatus">The expected CTRF status string.</param>
        [InlineData(TestResult.Passed, "passed")]
        [InlineData(TestResult.Failed, "failed")]
        [InlineData(TestResult.NoRun, "pending")]
        [InlineData(TestResult.Skipped, "skipped")]
        [Theory]
        public void GetCtrfStatusFromTestResult(TestResult result, string expectedStatus)
        {
            var status = CtrfResultMapping.GetCtrfStatusFromTestResult(result);
            Assert.Equal(expectedStatus, status);
        }

        /// <summary>
        /// Verifies that <see cref="CtrfResultMapping.GetTestResultFromCtrfStatus(string)"/> returns the correct <see cref="TestResult"/> for a given CTRF status string.
        /// </summary>
        /// <param name="expectedResult">The expected <see cref="TestResult"/> value.</param>
        /// <param name="status">The CTRF status string to map.</param>
        [InlineData(TestResult.Passed, "passed")]
        [InlineData(TestResult.Failed, "failed")]
        [InlineData(TestResult.NoRun, "pending")]
        [InlineData(TestResult.Skipped, "skipped")]
        [Theory]
        public void GetTestResultFromCtrfStatus(TestResult expectedResult, string status)
        {
            var result = CtrfResultMapping.GetTestResultFromCtrfStatus(status);
            Assert.Equal(expectedResult, result);
        }
    }
}