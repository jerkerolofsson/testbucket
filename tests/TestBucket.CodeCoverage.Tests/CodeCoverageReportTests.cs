using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestBucket.CodeCoverage.Models;
using TestBucket.CodeCoverage.Parsers;

namespace TestBucket.CodeCoverage.Tests
{
    /// <summary>
    /// Contains unit tests for the <see cref="CodeCoverageReport"/> class, verifying code coverage calculations and package lookup functionality.
    /// </summary>
    [Feature("Code Coverage")]
    [Component("Code Coverage")]
    [UnitTest]
    [EnrichedTest]
    [FunctionalTest]
    public class CodeCoverageReportTests
    {
        /// <summary>
        /// Verifies that <see cref="CodeCoverageReport.CoveragePercent"/> returns zero when there are no lines to cover.
        /// </summary>
        [Fact]
        public void CoveragePercent_WithNoLines_IsZero()
        {
            // Arrange
            var report = new CodeCoverageReport();

            // Act
            var coveragePercent = report.CoveragePercent.Value;

            Assert.Equal(0, coveragePercent);
        }

        /// <summary>
        /// Verifies that <see cref="CodeCoverageReport.FindPackageByName(string)"/> successfully finds an existing package by name.
        /// </summary>
        [Fact]
        public void FindPackageByName_WithExistingPackage_Success()
        {
            var report = new CodeCoverageReport();
            report.GetOrCreatePackage("package1");

            var package = report.FindPackageByName("package1");

            Assert.NotNull(package);
            Assert.Equal("package1", package.Name);
        }

        /// <summary>
        /// Verifies that <see cref="CodeCoverageReport.FindPackageByName(string)"/> can find a package by name when multiple packages exist.
        /// </summary>
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

        /// <summary>
        /// Verifies that <see cref="CodeCoverageReport.AddPackage(CodeCoveragePackage)"/> adds a package to the report.
        /// </summary>
        [Fact]
        public void AddPackage_AddsPackageToReport()
        {
            var report = new CodeCoverageReport();
            var package = new CodeCoveragePackage { Name = "MyPackage" };

            report.AddPackage(package);

            Assert.Contains(package, report.Packages);
        }

        /// <summary>
        /// Verifies that <see cref="CodeCoverageReport.GetOrCreatePackage(string)"/> creates a new package if it does not exist.
        /// </summary>
        [Fact]
        public void GetOrCreatePackage_CreatesNewPackage_WhenNotExists()
        {
            var report = new CodeCoverageReport();

            var package = report.GetOrCreatePackage("NewPackage");

            Assert.NotNull(package);
            Assert.Equal("NewPackage", package.Name);
            Assert.Contains(package, report.Packages);
        }

        /// <summary>
        /// Verifies that <see cref="CodeCoverageReport.FindPackage(Predicate{CodeCoveragePackage})"/> finds a package by predicate.
        /// </summary>
        [Fact]
        public void FindPackage_ByPredicate_FindsCorrectPackage()
        {
            var report = new CodeCoverageReport();
            var package = new CodeCoveragePackage { Name = "Target" };
            report.AddPackage(package);

            var found = report.FindPackage(p => p.Name == "Target");

            Assert.Same(package, found);
        }

        /// <summary>
        /// Verifies that <see cref="CodeCoverageReport.CoveragePercent"/> calculates correct coverage for multiple packages and classes.
        /// </summary>
        [Fact]
        public void CoveragePercent_CalculatesCorrectly_WithMultiplePackagesAndClasses()
        {
            var report = new CodeCoverageReport();

            var package1 = report.GetOrCreatePackage("P1");
            var class1 = package1.GetOrCreateClass("C1", "C1.cs");
            class1.AddLine(new CodeCoverageLine { LineNumber = 1, Hits = 1 });
            class1.AddLine(new CodeCoverageLine { LineNumber = 2, Hits = 0 });

            var package2 = report.GetOrCreatePackage("P2");
            var class2 = package2.GetOrCreateClass("C2", "C2.cs");
            class2.AddLine(new CodeCoverageLine { LineNumber = 1, Hits = 1 });
            class2.AddLine(new CodeCoverageLine { LineNumber = 2, Hits = 1 });

            // Total: 4 lines, 3 covered
            Assert.Equal(75.0, report.CoveragePercent.Value);
        }
    }
}