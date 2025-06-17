using TestBucket.CodeCoverage.Models;
using TestBucket.CodeCoverage.Tests.Utils;

namespace TestBucket.CodeCoverage.Tests
{
    /// <summary>
    /// Contains unit tests for the <see cref="CoverageFormatDetector"/> class.
    /// </summary>
    [Feature("Code Coverage")]
    [Component("Code Coverage")]
    [UnitTest]
    [EnrichedTest]
    [FunctionalTest]
    public class CoverageFormatDetectorTests
    {
        /// <summary>
        /// Verifies that <see cref="CoverageFormatDetector.Detect(byte[])"/> returns <see cref="CodeCoverageFormat.UnknownFormat"/>
        /// when provided with an empty string.
        /// </summary>
        [Fact]
        public void DetectFormat_WithEmptyString_ReturnsUnknown()
        {
            // Arrange
            var xml = "";

            // Act
            var result = CoverageFormatDetector.Detect(Encoding.UTF8.GetBytes(xml));

            // Assert
            Assert.Equal(CodeCoverageFormat.UnknownFormat, result);
        }

        /// <summary>
        /// Verifies that <see cref="CoverageFormatDetector.DetectFromFileAsync(string)"/> returns <see cref="CodeCoverageFormat.Cobertura"/>
        /// when provided with a valid Cobertura XML file.
        /// </summary>
        [Fact]
        public async Task DetectFromFileAsync_WithCoberturaXml_ReturnsCobertura()
        {
            // Arrange
            string xml = GetValidCoberturaXml();
            using var tempDir = new TempDir();
            var path = Path.Combine(tempDir.TempPath, "coverage.xml");
            File.WriteAllText(path, xml);

            // Act
            var result = await CoverageFormatDetector.DetectFromFileAsync(path);

            // Assert
            Assert.Equal(CodeCoverageFormat.Cobertura, result);
        }

        /// <summary>
        /// Verifies that <see cref="CoverageFormatDetector.Detect(byte[])"/> returns <see cref="CodeCoverageFormat.Cobertura"/>
        /// when provided with a valid Cobertura XML string.
        /// </summary>
        [Fact]
        public void DetectFormat_WithCoberturaXml_ReturnsCobertura()
        {
            // Arrange
            string xml = GetValidCoberturaXml();

            // Act
            var result = CoverageFormatDetector.Detect(Encoding.UTF8.GetBytes(xml));

            // Assert
            Assert.Equal(CodeCoverageFormat.Cobertura, result);
        }

        /// <summary>
        /// Returns a valid Cobertura XML string for testing purposes.
        /// </summary>
        /// <returns>A valid Cobertura XML string.</returns>
        private static string GetValidCoberturaXml()
        {
            return """
                <?xml version="1.0" encoding="utf-8" standalone="yes"?>
                <coverage line-rate="0.9851851851851852" branch-rate="0.9166666666666666" complexity="136" version="1.9" timestamp="1747026661" lines-covered="266" lines-valid="270" branches-covered="66" branches-valid="72">
                  <packages>
                    <package line-rate="0.9851851851851852" branch-rate="0.9166666666666666" complexity="136" name="TestBucket.CodeCoverage">
                      <classes>
                        <class line-rate="1" branch-rate="1" complexity="7" name="TestBucket.CodeCoverage.CodeCoverageReport" filename="D:\local\code\testbucket\src\Package\Reporting\TestBucket.CodeCoverage\CodeCoverageReport.cs">
                          <methods>
                            <method line-rate="1" branch-rate="1" complexity="1" name="get_Packages" signature="()">
                              <lines>
                                <line number="13" hits="1" branch="False" />
                              </lines>
                            </method>
                          </class>                
                      </classes>
                    </package>
                  </packages>
                </coverage>
                """;
        }
    }
}