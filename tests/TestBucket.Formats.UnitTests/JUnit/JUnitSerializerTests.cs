using TestBucket.Formats.JUnit;
using TestBucket.Formats.UnitTests.Utilities;
using TestBucket.Metrics.Xunit;
using TestBucket.Traits.Xunit;

namespace TestBucket.Formats.UnitTests.JUnit
{
    /// <summary>
    /// Contains unit tests for the <see cref="JUnitSerializer"/> class, verifying correct deserialization of JUnit XML files into test run and suite models.
    /// </summary>
    [UnitTest]
    [FunctionalTest]
    [Component("Test Formats")]
    [Feature("Import Test Results")]
    [EnrichedTest]
    [IncludeDiagnostics]
    public class JUnitSerializerTests
    {
        /// <summary>
        /// Verifies that a JUnit XML without a name attribute on the testsuites node gets the name from the first testsuite node.
        /// </summary>
        [Fact]
        [TestId("JUNIT-001")]
        public void Deserialize_WithoutTestSuitesName_RunNameFromFirstSuite()
        {
            var xml = TestDataUtils.GetResourceXml("TestBucket.Formats.UnitTests.JUnit.TestData.junit-basic.xml");
            var serializer = new JUnitSerializer();
            var run = serializer.Deserialize(xml);

            Assert.NotNull(run.Suites);
            Assert.NotEmpty(run.Suites);
            Assert.Equal(run.Name, run.Suites[0].Name);
        }

        /// <summary>
        /// Verifies that a JUnit XML contains the correct number of test suites and their names are deserialized correctly.
        /// </summary>
        [Fact]
        [TestId("JUNIT-002")]
        public void Deserialize_WithTwoTestSuites_TwoRunsDeserializedWithCorrectNames()
        {
            var xml = TestDataUtils.GetResourceXml("TestBucket.Formats.UnitTests.JUnit.TestData.junit-basic.xml");
            var serializer = new JUnitSerializer();
            var run = serializer.Deserialize(xml);

            Assert.NotNull(run.Suites);
            Assert.Equal(2, run.Suites.Count);
            Assert.Equal("Tests.Registration", run.Suites[0].Name);
            Assert.Equal("Tests.Authentication", run.Suites[1].Name);
        }

        /// <summary>
        /// Verifies that properties on the testcase element in a JUnit XML are extracted as traits.
        /// </summary>
        [Fact]
        [TestId("JUNIT-003")]
        public void Deserialize_WithPropertiesOnTest_TwoTestsDeserializedWithCorrectTraits()
        {
            var xml = TestDataUtils.GetResourceXml("TestBucket.Formats.UnitTests.JUnit.TestData.junit-properties.xml");
            var serializer = new JUnitSerializer();
            var run = serializer.Deserialize(xml);

            Assert.NotNull(run.Suites);
            Assert.Single(run.Suites);
            Assert.NotNull(run.Suites[0].Tests);
            var tests = run.Suites[0].Tests;
            Assert.NotNull(tests);
            Assert.Equal(2, tests.Count);

            foreach (var test in tests)
            {
                Assert.NotNull(test.Traits);

                var trait = test.Traits.Where(x => x.Name == "Product Component").FirstOrDefault();
                Assert.NotNull(trait);
                Assert.Equal("TestBucket.Formats", trait.Value);
            }
        }

        /// <summary>
        /// Verifies that the assembly name is read from the testsuite name if no assembly trait is defined in the JUnit XML.
        /// </summary>
        [Fact]
        [TestId("JUNIT-004")]
        public void Deserialize_WithoutAssemblyName_AssemblyNameSuiteName()
        {
            var xml = TestDataUtils.GetResourceXml("TestBucket.Formats.UnitTests.JUnit.TestData.junit-basic.xml");
            var serializer = new JUnitSerializer();
            var run = serializer.Deserialize(xml);

            Assert.NotNull(run.Suites);
            Assert.NotEmpty(run.Suites);
            Assert.Equal(2, run.Suites.Count);
            Assert.NotNull(run.Suites[0]);
            Assert.NotNull(run.Suites[1]);
            Assert.NotNull(run.Suites[0].Tests);
            Assert.NotNull(run.Suites[1].Tests);

            foreach (var test in run.Suites[0].Tests!)
            {
                Assert.Equal("Tests.Registration", test.Assembly);
            }

            foreach (var test in run.Suites[1].Tests!)
            {
                Assert.Equal("Tests.Authentication", test.Assembly);
            }
        }

        /// <summary>
        /// Verifies that tests from nested testsuites in a JUnit XML are flattened into the parent container.
        /// </summary>
        [Fact]
        [TestId("JUNIT-005")]
        public void Deserialize_WithNestedTestSuites_AllTestsInParentContainer()
        {
            var xml = TestDataUtils.GetResourceXml("TestBucket.Formats.UnitTests.JUnit.TestData.junit-basic.xml");
            var serializer = new JUnitSerializer();
            var run = serializer.Deserialize(xml);

            Assert.NotNull(run.Suites);
            Assert.NotEmpty(run.Suites);
            Assert.Equal(2, run.Suites.Count);
            Assert.NotNull(run.Suites[0]);
            Assert.NotNull(run.Suites[1]);
            Assert.NotNull(run.Suites[0].Tests);
            Assert.NotNull(run.Suites[1].Tests);

            Assert.Equal(3, run.Suites[0].Tests!.Count);
            Assert.Equal(6, run.Suites[1].Tests!.Count);
        }

        /// <summary>
        /// Verifies that tests from nested testsuites have a folder extracted from the nested testsuite name.
        /// </summary>
        [Fact]
        [TestId("JUNIT-006")]
        public void Deserialize_WithNestedTestSuites_FolderExtractedForNestedTests()
        {
            var xml = TestDataUtils.GetResourceXml("TestBucket.Formats.UnitTests.JUnit.TestData.junit-basic.xml");
            var serializer = new JUnitSerializer();
            var run = serializer.Deserialize(xml);

            Assert.NotNull(run.Suites);
            Assert.NotEmpty(run.Suites);
            Assert.Equal(2, run.Suites.Count);
            Assert.NotNull(run.Suites[0]);
            Assert.NotNull(run.Suites[1]);
            Assert.NotNull(run.Suites[0].Tests);
            Assert.NotNull(run.Suites[1].Tests);

            Assert.Equal(3, run.Suites[0].Tests!.Count);
            Assert.Equal(6, run.Suites[1].Tests!.Count);

            // Tests.Authentication
            // - testCase7
            // - testCase8
            // - testCase9
            // - Tests.Authentication.Login
            //   - testCase4
            //   - testCase5
            //   - testCase6

            var tests = run.Suites[1].Tests!;
            for (int i = 3; i < 6; i++)
            {
                Assert.Single(tests[i].Folders);
                Assert.Equal("Login", tests[i].Folders[0]);
            }
        }

        /// <summary>
        /// Verifies that if a TestId trait exists in the JUnit XML, it will be used for the ExternalId property.
        /// </summary>
        [Fact]
        [TestId("JUNIT-007")]
        public void Deserialize_WithTestIdTrait_TestIdTraitUsedAsExternalId()
        {
            var xml = TestDataUtils.GetResourceXml("TestBucket.Formats.UnitTests.JUnit.TestData.junit-testid-trait.xml");
            var serializer = new JUnitSerializer();
            var run = serializer.Deserialize(xml);

            Assert.NotNull(run.Suites);
            Assert.NotEmpty(run.Suites);
            Assert.NotEmpty(run.Suites[0].Tests);
            var test = run.Suites.First().Tests.First();

            Assert.Equal("MY-EXTERNAL-ID", test.ExternalId);
            Assert.Equal("MY-EXTERNAL-ID", test.TestId);
        }

        /// <summary>
        /// Verifies that a JUnit XML with a test suite root is parsed successfully.
        /// </summary>
        [Fact]
        [TestId("JUNIT-008")]
        public void Deserialize_WithTestSuiteRoot_ParsedSuccess()
        {
            var xml = TestDataUtils.GetResourceXml("TestBucket.Formats.UnitTests.JUnit.TestData.junit-testid-trait.xml");
            var serializer = new JUnitSerializer();
            var run = serializer.Deserialize(xml);

            Assert.NotNull(run.Suites);
            Assert.NotEmpty(run.Suites);
            Assert.NotEmpty(run.Suites[0].Tests);
            var test = run.Suites.First().Tests.First();
        }
    }
}