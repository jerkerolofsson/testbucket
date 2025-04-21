using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Domain.TestResources.Allocation;

namespace TestBucket.Domain.Testing.Compiler;

/// <summary>
/// Takes in input that can contain template references, variables, and replaces it into rendered markup with actual values for variables
/// </summary>
public interface ITestCompiler
{
    /// <summary>
    /// Returns compiled markup by resolving templates, variables..
    /// </summary>
    /// <param name="principal"></param>
    /// <param name="context"></param>
    /// <param name="source"></param>
    /// <returns></returns>
    Task<string> CompileAsync(ClaimsPrincipal principal, TestExecutionContext context, string source);

   
    /// <summary>
    /// Resolves variables 
    /// </summary>
    /// <param name="principal"></param>
    /// <param name="context"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task ResolveVariablesAsync(ClaimsPrincipal principal, TestExecutionContext context, CancellationToken cancellationToken);
}
