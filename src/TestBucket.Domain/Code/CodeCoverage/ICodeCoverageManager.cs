using TestBucket.Domain.Code.CodeCoverage.Models;

namespace TestBucket.Domain.Code.CodeCoverage;
public interface ICodeCoverageManager
{
    Task<CodeCoverageSettings> LoadSettingsAsync(ClaimsPrincipal principal, long projectId);
}