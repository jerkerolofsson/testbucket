using TestBucket.CodeCoverage.Parsers;

namespace TestBucket.CodeCoverage.Tests.Parsing
{
    [Feature("Code Coverage")]
    [UnitTest]
    [EnrichedTest]
    [FunctionalTest]
    public class CoberturaParsingCorruptFileTests
    {
        [Fact]
        [TestDescription("""
            Verifies that hits is zero when the hits attribute is missing on a line
            """)]
        public async Task ParseCobertura_WithMissingHitsAttribute_HitsIsZero()
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
                                <line number="1" branch="False" />
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
            Assert.Equal(0, report.Packages[0].Classes[0].Methods[0].Lines[0].Hits);
        }

        [Fact]
        [TestDescription("""
            Verifies that when loading a file with missing filename on method, there is no crash or side-effect
            """)]
        public async Task ParseCobertura_WithMissingFilenameOnClass_ClassIsNotIngored()
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
        }

        [Fact]
        [TestDescription("""
            Verifies that when loading a file with missing method name, there is no crash and the method is not added
            """)]
        public async Task ParseCobertura_WithMissingMethodName_MethodIsIgnored()
        {
            var xml = """
                <?xml version="1.0" encoding="utf-8" standalone="yes"?>
                <coverage line-rate="0" branch-rate="0" complexity="135" version="1.9">
                  <packages>
                    <package name="package1">
                      <classes>
                        <class line-rate="1" branch-rate="1" complexity="7" name="class1" filename="/tmp/file1.cs">
                          <methods>
                            <method signature="()">
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
            Assert.Empty(report.Packages[0].Classes[0].Methods);
        }
    }
}
