using TestBucket.CodeCoverage.Models;
using TestBucket.CodeCoverage.Parsers;

namespace TestBucket.CodeCoverage.Tests.Parsing
{
    /// <summary>
    /// Contains unit tests for the <see cref="CoberturaParser"/> class, verifying correct parsing and merging of Cobertura code coverage reports.
    /// </summary>
    [Feature("Code Coverage")]
    [UnitTest]
    [EnrichedTest]
    [FunctionalTest]
    public class CoberturaParsingTests
    {
        /// <summary>
        /// Verifies that when loading the same coverage report twice and merging it, the number of packages are not duplicated.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        [Fact]
        public async Task ParseCobertura_Twice_PackagesCorrect()
        {
            var parser = new CoberturaParser();
            var report = new CodeCoverageReport();
            report = await parser.ParseFileAsync(report, "./TestData/coverage.cobertura.xml", TestContext.Current.CancellationToken);
            report = await parser.ParseFileAsync(report, "./TestData/coverage.cobertura.xml", TestContext.Current.CancellationToken);

            Assert.NotEmpty(report.Packages);
            Assert.Equal(3, report.Packages.Count);
            Assert.Equal("TestBucket.Domain", report.Packages[0].Name);
            Assert.Equal("TestBucket.Domain", report.Packages[0].GetName());
            Assert.Equal("TestBucket.Data", report.Packages[1].Name);
            Assert.Equal("TestBucket.Formats", report.Packages[2].Name);
        }

        /// <summary>
        /// Verifies that when loading the same coverage report twice and merging it, the number of hits are added together.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        [Fact]
        public async Task ParseCobertura_Twice_LineHitsAdded()
        {
            var parser = new CoberturaParser();
            var report = new CodeCoverageReport();
            report = await parser.ParseFileAsync(report, "./TestData/coverage.cobertura.xml", TestContext.Current.CancellationToken);
            report = await parser.ParseFileAsync(report, "./TestData/coverage.cobertura.xml", TestContext.Current.CancellationToken);

            Assert.NotEmpty(report.Packages);
            Assert.Equal(3, report.Packages.Count);
            var formatsPackage = report.Packages.Where(x => x.Name == "TestBucket.Formats").First();
            var importDefaultsClass = formatsPackage.FindClassByName("TestBucket.Formats.ImportDefaults");
            Assert.NotNull(importDefaultsClass);

            var getExternalIdMethod = importDefaultsClass.FindMethodBySignature("GetExternalId", "(TestBucket.Formats.Dtos.TestRunDto, TestBucket.Formats.Dtos.TestSuiteRunDto, TestBucket.Formats.Dtos.TestCaseRunDto)");
            Assert.NotNull(getExternalIdMethod);

            var line5 = getExternalIdMethod.FindLineByNumber(5);
            Assert.NotNull(line5);
            Assert.Equal(2, line5.Hits);
        }

        /// <summary>
        /// Verifies that the parsed package names are correct when no methods are present.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        [Fact]
        public async Task ParseCobertura_WithNoMethods_CoveragePercentCorrect()
        {
            var parser = new CoberturaParser();
            CodeCoverageReport report = await parser.ParseFileAsync("./TestData/nomethods.cobertura.xml", TestContext.Current.CancellationToken);

            Assert.Single(report.Packages);
            var percent = report.Packages[0].CoveragePercent.Value;
            Assert.Equal(85.7, Math.Round(percent, 1));
        }

        /// <summary>
        /// Verifies calling <c>GetChildren</c> on a package returns all classes.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        [Fact]
        public async Task GetChildrenOnPackage_After_ParseCobertura_WithClasses_ReturnsClasses()
        {
            var parser = new CoberturaParser();
            CodeCoverageReport report = await parser.ParseFileAsync("./TestData/nomethods.cobertura.xml", TestContext.Current.CancellationToken);

            Assert.Single(report.Packages);
            var classes = report.Packages[0].GetChildren();
            Assert.NotEmpty(classes);
            Assert.Single(classes);
            Assert.Equal("example_cpp", classes.First().GetName());
        }

        /// <summary>
        /// Verifies <c>GetChildren</c> on a class returns methods.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        [Fact]
        public async Task GetChildrenOnClass_After_ParseCobertura_WithClassesAndMethods_ReturnsMethods()
        {
            // Arrange
            var parser = new CoberturaParser();
            CodeCoverageReport report = await parser.ParseFileAsync("./TestData/coverage.cobertura.xml", TestContext.Current.CancellationToken);

            // Act
            Assert.NotEmpty(report.Packages);
            var classes = report.Packages[0].GetChildren();

            // Assert
            Assert.NotEmpty(classes);
            var domainServiceExtensions = classes.First(x => x.GetName() == "Microsoft.Extensions.DependencyInjection.DomainServiceExtensions");
            var methods = domainServiceExtensions.GetChildren();
            Assert.Single(methods);
            Assert.Equal("AddDomainServices", methods.First().GetName());
        }

        /// <summary>
        /// Verifies <c>GetChildren</c> on a method returns lines.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        [Fact]
        public async Task GetChildrenOnMethods_After_ParseCobertura_WithClassesAndMethods_ReturnsLines()
        {
            // Arrange
            var parser = new CoberturaParser();
            CodeCoverageReport report = await parser.ParseFileAsync("./TestData/coverage.cobertura.xml", TestContext.Current.CancellationToken);

            // Act
            Assert.NotEmpty(report.Packages);
            var classes = report.Packages[0].GetChildren();

            // Assert
            Assert.NotEmpty(classes);
            var domainServiceExtensions = classes.First(x => x.GetName() == "Microsoft.Extensions.DependencyInjection.DomainServiceExtensions");
            var methods = domainServiceExtensions.GetChildren();
            Assert.Single(methods);
            var method = methods.First();
            var lines = method.GetChildren();
            Assert.NotEmpty(lines);
            Assert.True(lines[0] is CodeCoverageLine);
        }

        /// <summary>
        /// Verifies that the <c>CoveragePercent</c> is correct for the whole file.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        [Fact]
        public async Task ParseCobertura_WithMethods_CoveragePercentIsCorrect()
        {
            var parser = new CoberturaParser();
            CodeCoverageReport report = await parser.ParseFileAsync("./TestData/coverage.cobertura.xml", TestContext.Current.CancellationToken);

            Assert.NotEmpty(report.Packages);
            var percent = report.CoveragePercent.Value;
            Assert.Equal(22.34, percent);
        }

        /// <summary>
        /// Verifies that the number of lines calculated is correct in a file with methods.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        [Fact]
        public async Task ParseCobertura_WithMethods_LineCountCorrect()
        {
            var parser = new CoberturaParser();
            CodeCoverageReport report = await parser.ParseFileAsync("./TestData/coverage.cobertura.xml", TestContext.Current.CancellationToken);

            Assert.NotEmpty(report.Packages);
            var domainLineCount = report.LineCount.Value;
            var percent = report.CoveragePercent.Value;
            Assert.Equal(16014, domainLineCount);
        }

        /// <summary>
        /// Verifies that the number of lines calculated is correct in a file without methods.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        [Fact]
        public async Task ParseCobertura_WithNoMethods_PackageLineCountCorrect()
        {
            var parser = new CoberturaParser();
            CodeCoverageReport report = await parser.ParseFileAsync("./TestData/nomethods.cobertura.xml", TestContext.Current.CancellationToken);

            Assert.Single(report.Packages);
            var domainLineCount = report.Packages[0].LineCount.Value;
            Assert.Equal(7, domainLineCount);
        }

        /// <summary>
        /// Verifies that generated classes for the async state machine are merged into a single class, as per the code
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        [Theory]
        [InlineData("TestBucket.Domain.Traceability.DiscoverRequirementRelationshipsHandler")]
        [InlineData("TestBucket.Domain.TestResources.TestCaseDependencyMerger")]
        public async Task ParseCobertura_WithAsyncClass_MergedWithRealClass(string className)
        {
            var parser = new CoberturaParser();
            CodeCoverageReport report = await parser.ParseFileAsync("./TestData/coverage.cobertura.xml", TestContext.Current.CancellationToken);

            Assert.NotEmpty(report.Packages);
            int count = 0;
            foreach(var @class in report.GetClasses(_ => true))
            {
                if (@class.Name == className)
                {
                    count++;
                }
                else if (@class.Name.StartsWith(className))
                {
                    Assert.Fail($"Expected class {@class.Name} to be trimmed and merged");
                }
            }
            Assert.Equal(1, count);
        }

        /// <summary>
        /// Verifies that the parsed package names are correct.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        [Fact]
        public async Task ParseCobertura_PackagesCorrect()
        {
            var parser = new CoberturaParser();
            CodeCoverageReport report = await parser.ParseFileAsync("./TestData/coverage.cobertura.xml", TestContext.Current.CancellationToken);

            Assert.NotEmpty(report.Packages);
            Assert.Equal(3, report.Packages.Count);
            Assert.Equal("TestBucket.Domain", report.Packages[0].Name);
            Assert.Equal("TestBucket.Data", report.Packages[1].Name);
            Assert.Equal("TestBucket.Formats", report.Packages[2].Name);
        }

        /// <summary>
        /// Verifies that the class filename is parsed correctly.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        [Fact]
        public async Task ParseCobertura_ClassFilenameCorrect()
        {
            var parser = new CoberturaParser();

            // Act
            CodeCoverageReport report = await parser.ParseFileAsync("./TestData/coverage.cobertura.xml", TestContext.Current.CancellationToken);

            // Assert
            Assert.NotEmpty(report.Packages);
            var formatsPackage = report.Packages.Where(x => x.Name == "TestBucket.Formats").First();
            var importDefaultsClass = formatsPackage.FindClassByName("TestBucket.Formats.ImportDefaults");
            Assert.NotNull(importDefaultsClass);
            Assert.Equal("D:\\local\\code\\testbucket\\src\\Package\\Reporting\\TestBucket.Formats\\ImportDefaults.cs", importDefaultsClass.FileName);
        }

        /// <summary>
        /// Verifies that the class is found in the parsed report.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        [Fact]
        public async Task ParseCobertura_ClassFound()
        {
            var parser = new CoberturaParser();

            // Act
            CodeCoverageReport report = await parser.ParseFileAsync("./TestData/coverage.cobertura.xml", TestContext.Current.CancellationToken);

            // Assert
            Assert.NotEmpty(report.Packages);
            var formatsPackage = report.Packages.Where(x => x.Name == "TestBucket.Formats").First();
            var importDefaultsClass = formatsPackage.FindClassByName("TestBucket.Formats.ImportDefaults");
            Assert.NotNull(importDefaultsClass);
        }

        /// <summary>
        /// Verifies that the method is found in the parsed class.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        [Fact]
        public async Task ParseCobertura_MethodFound()
        {
            var parser = new CoberturaParser();

            // Act
            CodeCoverageReport report = await parser.ParseFileAsync("./TestData/coverage.cobertura.xml", TestContext.Current.CancellationToken);

            // Assert
            Assert.NotEmpty(report.Packages);
            var formatsPackage = report.Packages.Where(x => x.Name == "TestBucket.Formats").First();
            var importDefaultsClass = formatsPackage.FindClassByName("TestBucket.Formats.ImportDefaults");
            Assert.NotNull(importDefaultsClass);
            Assert.Equal("TestBucket.Formats.ImportDefaults", importDefaultsClass.Name);
            Assert.Equal("TestBucket.Formats.ImportDefaults", importDefaultsClass.GetName());

            var getExternalIdMethod = importDefaultsClass.FindMethodBySignature("GetExternalId", "(TestBucket.Formats.Dtos.TestRunDto, TestBucket.Formats.Dtos.TestSuiteRunDto, TestBucket.Formats.Dtos.TestCaseRunDto)");
            Assert.NotNull(getExternalIdMethod);
            Assert.Equal("GetExternalId", getExternalIdMethod.Name);
            Assert.Equal("GetExternalId", getExternalIdMethod.GetName());
        }

        /// <summary>
        /// Verifies that the line is found in the parsed method.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        [Fact]
        public async Task ParseCobertura_LineFound()
        {
            var parser = new CoberturaParser();

            // Act
            CodeCoverageReport report = await parser.ParseFileAsync("./TestData/coverage.cobertura.xml", TestContext.Current.CancellationToken);

            // Assert
            Assert.NotEmpty(report.Packages);
            var formatsPackage = report.Packages.Where(x => x.Name == "TestBucket.Formats").First();
            var importDefaultsClass = formatsPackage.FindClassByName("TestBucket.Formats.ImportDefaults");
            Assert.NotNull(importDefaultsClass);

            var getExternalIdMethod = importDefaultsClass.FindMethodBySignature("GetExternalId", "(TestBucket.Formats.Dtos.TestRunDto, TestBucket.Formats.Dtos.TestSuiteRunDto, TestBucket.Formats.Dtos.TestCaseRunDto)");
            Assert.NotNull(getExternalIdMethod);
        }

        /// <summary>
        /// Verifies that the condition type is parsed correctly for each line.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        [Fact]
        public async Task ParseCobertura_ConditionTypeCorrect()
        {
            var parser = new CoberturaParser();

            // Act
            CodeCoverageReport report = await parser.ParseFileAsync("./TestData/coverage.cobertura.xml", TestContext.Current.CancellationToken);

            // Assert
            Assert.NotEmpty(report.Packages);
            var formatsPackage = report.Packages.Where(x => x.Name == "TestBucket.Formats").First();
            var importDefaultsClass = formatsPackage.FindClassByName("TestBucket.Formats.ImportDefaults");
            Assert.NotNull(importDefaultsClass);

            var getExternalIdMethod = importDefaultsClass.FindMethodBySignature("GetExternalId", "(TestBucket.Formats.Dtos.TestRunDto, TestBucket.Formats.Dtos.TestSuiteRunDto, TestBucket.Formats.Dtos.TestCaseRunDto)");
            Assert.NotNull(getExternalIdMethod);

            Assert.Equal(4, getExternalIdMethod.Lines.Count);

            var line6 = getExternalIdMethod.FindLineByNumber(6);
            Assert.NotNull(line6);
            Assert.Equal(5, line6.Conditions.Count);
            Assert.Equal("jump", line6.Conditions[0].Type);
            Assert.Equal("jump", line6.Conditions[1].Type);
            Assert.Equal("jump", line6.Conditions[2].Type);
            Assert.Equal("jump", line6.Conditions[3].Type);
            Assert.Equal("jump", line6.Conditions[4].Type);
        }

        /// <summary>
        /// Verifies that the condition coverage is parsed correctly for each line.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        [Fact]
        public async Task ParseCobertura_ConditionCoverageCorrect()
        {
            var parser = new CoberturaParser();

            // Act
            CodeCoverageReport report = await parser.ParseFileAsync("./TestData/coverage.cobertura.xml", TestContext.Current.CancellationToken);

            // Assert
            Assert.NotEmpty(report.Packages);
            var formatsPackage = report.Packages.Where(x => x.Name == "TestBucket.Formats").First();
            var importDefaultsClass = formatsPackage.FindClassByName("TestBucket.Formats.ImportDefaults");
            Assert.NotNull(importDefaultsClass);

            var getExternalIdMethod = importDefaultsClass.FindMethodBySignature("GetExternalId", "(TestBucket.Formats.Dtos.TestRunDto, TestBucket.Formats.Dtos.TestSuiteRunDto, TestBucket.Formats.Dtos.TestCaseRunDto)");
            Assert.NotNull(getExternalIdMethod);

            Assert.Equal(4, getExternalIdMethod.Lines.Count);

            var line6 = getExternalIdMethod.FindLineByNumber(6);
            Assert.NotNull(line6);
            Assert.Equal(5, line6.Conditions.Count);
            Assert.Equal(0.5, line6.Conditions[0].Coverage);
            Assert.Equal(0.5, line6.Conditions[1].Coverage);
            Assert.Equal(0.5, line6.Conditions[2].Coverage);
            Assert.Equal(0.5, line6.Conditions[3].Coverage);
            Assert.Equal(1, line6.Conditions[4].Coverage);
        }

        /// <summary>
        /// Verifies that the branch attribute on a line is parsed correctly when it is false.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        [Fact]
        [TestDescription("""
            Verifies that the branch attribute on a line is parsed correctly when it is false
            """)]
        public async Task ParseCobertura_WithBranchFalse_ParsedCorrectly()
        {
            var xml = """
                <?xml version="1.0" encoding="utf-8" standalone="yes"?>
                <coverage line-rate="0" branch-rate="0" complexity="135" version="1.9">
                  <packages>
                    <package name="package1">
                      <classes>
                        <class line-rate="1" branch-rate="1" complexity="7" name="class1">
                          <methods>
                            <method name="method1" signature="()">
                              <lines>
                                <line number="1" hits="1" branch="False" />
                              </lines>
                            </method>
                          </methods>
                        </class>
                      </classes>
                    </package>
                  </packages>
                </coverage>
                """;
            var parser = new CoberturaParser();
            var report = await parser.ParseTextAsync(xml, TestContext.Current.CancellationToken);

            Assert.NotEmpty(report.Packages);
            Assert.Single(report.Packages);
            Assert.Equal("package1", report.Packages[0].Name);
            Assert.Single(report.Packages[0].Classes);
            Assert.Equal("class1", report.Packages[0].Classes[0].Name);
            Assert.Single(report.Packages[0].Classes[0].Methods);
            Assert.Single(report.Packages[0].Classes[0].Methods[0].Lines);
            Assert.False(report.Packages[0].Classes[0].Methods[0].Lines[0].IsBranch);
        }

        /// <summary>
        /// Verifies that the branch attribute on a line is parsed correctly when it is true.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        [Fact]
        [TestDescription("""
            Verifies that the branch attribute on a line is parsed correctly when it is true
            """)]
        public async Task ParseCobertura_WithBranchTrue_ParsedCorrectly()
        {
            var xml = """
                <?xml version="1.0" encoding="utf-8" standalone="yes"?>
                <coverage line-rate="0" branch-rate="0" complexity="135" version="1.9">
                  <packages>
                    <package name="package1">
                      <classes>
                        <class line-rate="1" branch-rate="1" complexity="7" name="class1">
                          <methods>
                            <method name="method1" signature="()">
                              <lines>
                                <line number="1" hits="1" branch="True" />
                              </lines>
                            </method>
                          </methods>
                        </class>
                      </classes>
                    </package>
                  </packages>
                </coverage>
                """;
            var parser = new CoberturaParser();
            var report = await parser.ParseTextAsync(xml, TestContext.Current.CancellationToken);

            Assert.NotEmpty(report.Packages);
            Assert.Single(report.Packages);
            Assert.Equal("package1", report.Packages[0].Name);
            Assert.Single(report.Packages[0].Classes);
            Assert.Equal("class1", report.Packages[0].Classes[0].Name);
            Assert.Single(report.Packages[0].Classes[0].Methods);
            Assert.Single(report.Packages[0].Classes[0].Methods[0].Lines);
            Assert.True(report.Packages[0].Classes[0].Methods[0].Lines[0].IsBranch);
        }
    }
}