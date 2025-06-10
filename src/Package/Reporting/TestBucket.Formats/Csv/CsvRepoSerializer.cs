using CsvHelper.Configuration;

using TestBucket.Formats.Shared;

namespace TestBucket.Formats.Csv;
public class CsvRepoSerializer : ITestRepositorySerializer
{
    public string MediaType => "text/csv";

    public async ValueTask<TestRepositoryDto> DeserializeAsync(Stream source)
    {
        var result = new TestRepositoryDto();
        var context = new RepositoryDeserializationContext();

        using var textReader = new StreamReader(source, Encoding.UTF8, leaveOpen: true);
        var reader = new CsvHelper.CsvReader(textReader, new CsvConfiguration(CultureInfo.InvariantCulture) { DetectDelimiter = true });

        await reader.ReadAsync();
        if (reader.ReadHeader())
        {
            context.Headers = reader.HeaderRecord ?? [];

            while(await reader.ReadAsync())
            {
                string[] row = reader.Parser.Record ?? [];
                await ProcessRowAsync(context, result, row);
            }
        }

        return result;
    }

    private ValueTask ProcessRowAsync(RepositoryDeserializationContext context, TestRepositoryDto result, string[] row)
    {
        HashSet<string> suiteNames = ["suite"];

        // Find suite
        TestSuiteDto? suite = null;
        foreach (var header in context.Headers.Index().Where(x=> suiteNames.Contains(x.Item.ToLower())))
        {
            if(row.Length > header.Index)
            {
                var suiteName = row[header.Index];
                if (!string.IsNullOrWhiteSpace(suiteName))
                {
                    suite = result.TestSuites.FirstOrDefault(x => x.Name == suiteName);
                    if (suite is null)
                    {
                        suite = new TestSuiteDto { Name = suiteName };
                        result.TestSuites.Add(suite);
                    }
                }
            }
        }

        var isTestBucketCsv = context.Headers.Contains("tb_slug");

        // Test case
        TestCaseDto testCase = new TestCaseDto() { TenantId = "", TestCaseName = "", Description = "" };
        foreach (var header in context.Headers.Index())
        {
            if (row.Length > header.Index)
            {
                var value = row[header.Index];
                if(string.IsNullOrWhiteSpace(value))
                {
                    continue;
                }

                switch (header.Item.ToLower())
                {
                    case "suite":
                        testCase.TestSuiteName = value;
                        break;
                    case "tb_suite_slug":
                        testCase.TestSuiteSlug = value;
                        break;

                    case "title":
                    case "name":
                    case "case":
                        testCase.TestCaseName = value;
                        break;
                    case "slug":
                        testCase.Slug = value;
                        break;
                    case "id":
                    case "case id":
                        testCase.ExternalDisplayId = value;
                        break;
                    case "path":
                    case "folder":
                    case "section hierarchy":
                        testCase.Path = value;
                        break;

                    case "preconditions":
                        testCase.Description += "\n\n## Pre-condition\n" + value;
                        break;

                    case "description":

                        // If this is import from TestBucket format, keep the description as-is
                        if (isTestBucketCsv)
                        {
                            testCase.Description = value;
                        }
                        else
                        {
                            testCase.Description += "## Description\n" + value;
                        }
                        break;

                    case "postconditions":
                        testCase.Description += "\n\n## Post-condition\n" + value;
                        break;

                    case "steps":
                    case "steps_actions":
                        testCase.Description += "\n\n## Steps\n" + value;
                        break;
                    case "steps_result":
                    case "expected result":
                        testCase.Description += "\n\n## Expected Result\n" + value;
                        break;
                }
            }
        }

        if(!string.IsNullOrWhiteSpace(testCase.TestCaseName))
        {
            result.TestCases.Add(testCase);
        }
        return ValueTask.CompletedTask;
    }

    public ValueTask SerializeAsync(TestRepositoryDto source, Stream destination)
    {
        using var textWriter = new StreamWriter(destination, Encoding.UTF8, leaveOpen: true);
        var writer = new CsvHelper.CsvWriter(textWriter, new CsvConfiguration(CultureInfo.InvariantCulture) { Delimiter = "," });

        // Write header
        List<string> header = 
            [
            "id", 
            "tb_slug", 
            "title", 
            "path",
            "description",

            "category",
            "feature",
            "component",
            "priority",

            "suite", 
            "tb_suite_slug"
            ];
        foreach (var column in header)
        {
            writer.WriteField(column);
        }
        writer.NextRecord();

        foreach (var testCase in source.TestCases)
        {
            var suite = source.TestSuites.FirstOrDefault(x => x.Slug == testCase.TestSuiteSlug);

            writer.WriteField(testCase.ExternalDisplayId?.ToString());
            writer.WriteField(testCase.Slug?.ToString());
            writer.WriteField(testCase.TestCaseName?.ToString());
            writer.WriteField(testCase.Path?.ToString());
            writer.WriteField(testCase.Description?.ToString());

            writer.WriteField(testCase.Traits?.TestCategory);
            writer.WriteField(testCase.Traits?.Feature);
            writer.WriteField(testCase.Traits?.Component);
            writer.WriteField(testCase.Traits?.TestPriority);

            writer.WriteField(suite?.Name?.ToString());
            writer.WriteField(suite?.Slug?.ToString());
            writer.NextRecord();

        }

        return ValueTask.CompletedTask;
    }
}
