using TestBucket.CodeCoverage.Parsers;

namespace TestBucket.CodeCoverage.Tests.Parsing
{
    [Feature("Code Coverage")]
    [UnitTest]
    [EnrichedTest]
    [FunctionalTest]
    public class CoberturaParsingTests
    {
        [Fact]
        [TestDescription("""
            Verifies that when loading the same coverage report twice and merging it, the number of 
            packages are not duplicated
            """)]
        public async Task ParseCobertura_Twice_PackagesCorrect()
        {
            var parser = new CoberturaParser();
            var report = new CodeCoverageReport();
            report = await parser.ParseFileAsync(report, "./TestData/coverage.cobertura.xml", TestContext.Current.CancellationToken);
            report = await parser.ParseFileAsync(report, "./TestData/coverage.cobertura.xml", TestContext.Current.CancellationToken);

            Assert.NotEmpty(report.Packages);
            Assert.Equal(3, report.Packages.Count);
            Assert.Equal("TestBucket.Domain", report.Packages[0].Name);
            Assert.Equal("TestBucket.Data", report.Packages[1].Name);
            Assert.Equal("TestBucket.Formats", report.Packages[2].Name);
        }

        [Fact]
        [TestDescription("""
            Verifies that when loading the same coverage report twice and merging it, the number of 
            hits are added together
            """)]
        public async Task ParseCobertura_Twice_LineHitsAddedd()
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

            var getExternalIdMethod = importDefaultsClass.FindMethodBySignature("GetExternalId", "(TestBucket.Formats.Dtos.TestRunDto, TestBucket.Formats.Dtos.TestSuiteRunDto, TestBucket.Formats.Dtos.TestCaseRunDto)");
            Assert.NotNull(getExternalIdMethod);
        }

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
