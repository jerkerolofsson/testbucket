using Microsoft.Extensions.Logging;

using TestBucket.Domain.Export.Handlers.Requirements;
using TestBucket.Domain.Export.Zip;
using TestBucket.Formats;

namespace TestBucket.Domain.Requirements.Import
{
    internal class TestZipImporter : ITestRepositorySerializer
    {
        public string MediaType => "application/zip";

        public ValueTask<TestRepositoryDto> DeserializeAsync(Stream stream)
        {
            var result = new TestRepositoryDto();

            var source = new ZipImporter(stream);

            foreach (var exportedEntity in source.ReadAll())
            {
                if(exportedEntity.Type == "test-suite")
                {
                    var suite = TestSerialization.DeserializeTestSuiteAsync(exportedEntity).Result;
                    if (suite != null)
                    {
                        result.TestSuites.Add(suite);
                    }
                }
                else if (exportedEntity.Type == "test-case")
                {
                    var testCase = TestSerialization.DeserializeTestCaseAsync(exportedEntity).Result;
                    if (testCase != null)
                    {
                        result.TestCases.Add(testCase);
                    }
                }
            }
            return ValueTask.FromResult(result);
        }

        public ValueTask SerializeAsync(TestRepositoryDto source, Stream destination)
        {
            throw new NotImplementedException();
        }
    }
}
