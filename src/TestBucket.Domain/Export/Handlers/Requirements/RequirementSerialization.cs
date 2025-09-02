using System.Text.Json;

using TestBucket.Domain.Export.Models;
using TestBucket.Formats.Dtos;

namespace TestBucket.Domain.Export.Handlers.Requirements;
public class TestSerialization
{
    public static async Task<TestSuiteDto?> DeserializeTestSuiteAsync(ExportEntity exportedEntity)
    {
        using var stream = exportedEntity.Open();
        return await JsonSerializer.DeserializeAsync<TestSuiteDto>(stream);
    }

    public static async Task<TestCaseDto?> DeserializeTestCaseAsync(ExportEntity exportedEntity)
    {
        using var stream = exportedEntity.Open();
        return await JsonSerializer.DeserializeAsync<TestCaseDto>(stream);
    }
}
