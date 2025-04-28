using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Domain.Code.Yaml;

namespace TestBucket.Domain.Code.Services;
public interface IArchitectureManager
{
    /// <summary>
    /// Returns a model of the project architecture
    /// </summary>
    /// <param name="principal"></param>
    /// <param name="project"></param>
    /// <returns></returns>
    Task<ProjectArchitectureModel> GetProductArchitectureAsync(ClaimsPrincipal principal, TestProject project);

    /// <summary>
    /// Imports a model of the project architecture, adding systems, components, features
    /// </summary>
    /// <param name="principal"></param>
    /// <param name="project"></param>
    /// <param name="model"></param>
    /// <returns></returns>
    Task ImportProductArchitectureAsync(ClaimsPrincipal principal, TestProject project, ProjectArchitectureModel model);
}
