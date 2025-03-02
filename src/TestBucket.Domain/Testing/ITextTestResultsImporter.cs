
using TestBucket.Domain.Testing.Formats;

namespace TestBucket.Domain.Testing;
public interface ITextTestResultsImporter
{
    /// <summary>
    /// Imports a text based test result file like a junitxml
    /// </summary>
    /// <param name="tenantId"></param>
    /// <param name="teamId"></param>
    /// <param name="projectId"></param>
    /// <param name="format"></param>
    /// <param name="text"></param>
    /// <returns></returns>
    Task ImportTextAsync(string tenantId, long? teamId, long? projectId, TestResultFormat format, string text);
}