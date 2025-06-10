using System.Security.Claims;

using TestBucket.Formats;
using TestBucket.Formats.Dtos;

namespace TestBucket.Domain.Testing.Services.Import;
public interface ITextTestResultsImporter
{
    Task ImportRunAsync(ClaimsPrincipal principal, long teamId, long projectId, TestRunDto run, ImportHandlingOptions options);

    /// <summary>
    /// Imports a text based test result file like a junitxml
    /// </summary>
    /// <param name="tenantId"></param>
    /// <param name="teamId"></param>
    /// <param name="projectId"></param>
    /// <param name="format"></param>
    /// <param name="text"></param>
    /// <returns></returns>
    Task ImportTextAsync(ClaimsPrincipal principal, long teamId, long projectId, TestResultFormat format, string text, ImportHandlingOptions options);
}