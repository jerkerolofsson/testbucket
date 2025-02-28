
using TestBucket.Domain.Testing.Formats;

namespace TestBucket.Domain.Testing;
public interface ITextTestResultsImporter
{
    Task ImportTextAsync(string tenantId, long? projectId, TestResultFormat format, string text);
}