using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestBucket.Formats.Ctrf;
using TestBucket.Traits.Xunit;

namespace TestBucket.Formats.UnitTests.Ctrf
{
    [UnitTest]
    [Trait("Format", "CTRF")]
    [EnrichedTest]
    public class CtrfResultMappingTests
    {
        [InlineData(TestResult.Passed, "passed")]
        [InlineData(TestResult.Passed, "failed")]
        [InlineData(TestResult.NoRun, "pending")]
        [InlineData(TestResult.Skipped, "skipped")]
        [Theory]
        public void GetCtrfStatusFromTestResult(TestResult result, string expectedStatus)
        {
            var status = CtrfResultMapping.GetCtrfStatusFromTestResult(result);
            Assert.Equal(expectedStatus, status);
        }

        [InlineData(TestResult.Passed, "passed")]
        [InlineData(TestResult.Passed, "failed")]
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
