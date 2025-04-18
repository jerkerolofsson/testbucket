using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Contracts.Testing.Models;

namespace TestBucket.Contracts.Integrations;

/// <summary>
/// Enables running tests from within the application, integrating with the markdown editor
/// </summary>
public interface IMarkdownTestRunner
{
    /// <summary>
    /// Matches the markup language.
    /// 
    /// Example for powershell
    /// ```powershell
    /// ```
    /// 
    /// Example for python
    /// ```python
    /// ```
    /// 
    /// Example for javascript
    /// ```javascript
    /// ```
    /// </summary>
    Task<bool> SupportsLanguageAsync(ClaimsPrincipal principal, string language);

    /// <summary>
    /// Returns supported languages
    /// </summary>
    /// <returns></returns>
    Task<string[]> GetSupportedLanguagesAsync(ClaimsPrincipal principal);

    /// <summary>
    /// Runs the code
    /// </summary>
    /// <param name="code"></param>
    /// <returns></returns>
    Task<TestRunnerResult> RunAsync(ClaimsPrincipal principal, TestExecutionContext context, string language, string code, CancellationToken cancellationToken);
}
