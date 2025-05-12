using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestBucket.CodeCoverage.Parsers;

namespace TestBucket.CodeCoverage.Tests
{
    [Feature("Code Coverage")]
    [UnitTest]
    [EnrichedTest]
    [FunctionalTest]
    public class CodeCoverageReportTests
    {
        [Fact]
        public void FindPackageByName_WithExistingPackage_Success()
        {
            var report = new CodeCoverageReport();
            report.GetOrCreatePackage("package1");

            var package = report.FindPackageByName("package1");

            Assert.NotNull(package);
            Assert.Equal("package1", package.Name);
        }

        [Fact]
        public void FindPackageByName_WithManyPackages_Success()
        {
            var report = new CodeCoverageReport();
            report.GetOrCreatePackage("package4");
            report.GetOrCreatePackage("package3");
            report.GetOrCreatePackage("package2");
            report.GetOrCreatePackage("package1");

            var package = report.FindPackageByName("package1");

            Assert.NotNull(package);
            Assert.Equal("package1", package.Name);
        }
    }
}
