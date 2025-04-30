using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Domain.Code.Models;
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


    #region Features

    Task<IReadOnlyList<Feature>> SearchFeaturesAsync(ClaimsPrincipal principal, long projectId, string text, int offset, int count);
    Task<IReadOnlyList<Feature>> GetFeaturesAsync(ClaimsPrincipal principal, long projectId);
    Task<Feature?> GetFeatureByNameAsync(ClaimsPrincipal principal, long projectId, string name);

    #endregion Features

    #region Layers

    Task<ArchitecturalLayer?> GetLayerByNameAsync(ClaimsPrincipal principal, long projectId, string name);

    #endregion Layers

    #region Components
    Task<IReadOnlyList<Component>> SearchComponentsAsync(ClaimsPrincipal principal, long projectId, string text, int offset, int count);
    Task<IReadOnlyList<Component>> GetComponentsAsync(ClaimsPrincipal principal, long projectId);
    Task<Component?> GetComponentByNameAsync(ClaimsPrincipal principal, long projectId, string name);
    #endregion Components

    Task<ProductSystem?> GetSystemByNameAsync(ClaimsPrincipal principal, long projectId, string name);

    Task AddCommitsToFeatureAsync(ClaimsPrincipal principal, Feature feature, IEnumerable<Commit> commits);
}
