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

    private async ValueTask ProcessRowAsync(RepositoryDeserializationContext context, TestRepositoryDto result, string[] row)
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
                if (string.IsNullOrWhiteSpace(value))
                {
                    continue;
                }

                await AssignValueToTestCaseAsync(isTestBucketCsv, testCase, header, value);
            }
        }

        if(!string.IsNullOrWhiteSpace(testCase.TestCaseName))
        {
            result.TestCases.Add(testCase);
        }
    }

    private static async Task AssignValueToTestCaseAsync(bool isTestBucketCsv, TestCaseDto testCase, (int Index, string Item) header, string value)
    {
        var headerKey = await HeaderTranslator.TranslateHeaderAsync(header.Item);

        testCase.Traits ??= new();

        switch (headerKey)
        {
            case "suite":
                testCase.TestSuiteName = value;
                break;

            case "tb-slug":
                testCase.Slug = value;
                break;
            case "tb-suite-slug":
                testCase.TestSuiteSlug = value;
                break;
            case "tb-project-slug":
                testCase.ProjectSlug = value;
                break;

            case "name":
                testCase.TestCaseName = value;
                break;
            case "slug":
                testCase.Slug = value;
                break;
            case "id":
                testCase.ExternalDisplayId = value;
                break;
            case "path":
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
                testCase.Description += "\n\n## Steps\n" + value;
                break;
            case "expected-results":
                testCase.Description += "\n\n## Expected Result\n" + value;
                break;

            case "component":
                testCase.Traits.Component = value;
                break;
            case "feature":
                testCase.Traits.Feature = value;
                break;
            case "category":
                testCase.Traits.TestCategory = value;
                break;
            case "priority":
                testCase.Traits.TestPriority = value;
                break;
            case "quality-characteristic":
                testCase.Traits.QualityCharacteristic = value;
                break;

            default:
                // Add as a trait
                testCase.Traits.Traits.Add(new TestTrait
                {
                    Value = value,
                    Name = header.Item,
                    Type = Traits.Core.TraitType.Custom,
                    ExportType = TraitExportType.Static
                });
                break;
        }
    }

    public ValueTask SerializeAsync(TestRepositoryDto source, Stream destination)
    {
        using var textWriter = new StreamWriter(destination, Encoding.UTF8, leaveOpen: true);
        var writer = new CsvHelper.CsvWriter(textWriter, new CsvConfiguration(CultureInfo.InvariantCulture) { Delimiter = "," });

        // Write header
        List<string> header = 
            [
            "id", 
            "tb-slug", 
            "title", 
            "path",
            "description",

            "category",
            "feature",
            "component",
            "priority",
            "quality-characteristic",

            "suite", 
            "tb-suite-slug",
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
            writer.WriteField(testCase.Traits?.QualityCharacteristic);

            writer.WriteField(suite?.Name?.ToString());
            writer.WriteField(suite?.Slug?.ToString());
            writer.NextRecord();

        }

        return ValueTask.CompletedTask;
    }
}
