using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
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

    #region Systems
    Task<IReadOnlyList<ProductSystem>> GetSystemsAsync(ClaimsPrincipal principal, long projectId);
    Task AddSystemAsync(ClaimsPrincipal principal, ProductSystem system);
    Task UpdateSystemAsync(ClaimsPrincipal principal, ProductSystem system);
    Task DeleteSystemAsync(ClaimsPrincipal principal, ProductSystem system);
    Task<ProductSystem?> GetSystemByNameAsync(ClaimsPrincipal principal, long projectId, string name);
    #endregion Systems

    #region Features

    Task<IReadOnlyList<Feature>> SearchFeaturesAsync(ClaimsPrincipal principal, long projectId, string text, int offset, int count);
    Task<IReadOnlyList<Feature>> GetFeaturesAsync(ClaimsPrincipal principal, long projectId);
    Task<Feature?> GetFeatureByNameAsync(ClaimsPrincipal principal, long projectId, string name);
    Task AddFeatureAsync(ClaimsPrincipal principal, Feature feature);
    Task UpdateFeatureAsync(ClaimsPrincipal principal, Feature feature);
    Task DeleteFeatureAsync(ClaimsPrincipal principal, Feature feature);

    #endregion Features

    #region Layers
    Task<IReadOnlyList<ArchitecturalLayer>> GetLayersAsync(ClaimsPrincipal principal, long projectId);
    Task AddLayerAsync(ClaimsPrincipal principal, ArchitecturalLayer layer);
    Task UpdateLayerAsync(ClaimsPrincipal principal, ArchitecturalLayer layer);
    Task DeleteLayerAsync(ClaimsPrincipal principal, ArchitecturalLayer layer);

    Task<ArchitecturalLayer?> GetLayerByNameAsync(ClaimsPrincipal principal, long projectId, string name);

    #endregion Layers

    #region Components

    Task AddComponentAsync(ClaimsPrincipal principal, Component component);
    Task UpdateComponentAsync(ClaimsPrincipal principal, Component component);
    Task DeleteComponentAsync(ClaimsPrincipal principal, Component component);

    Task<IReadOnlyList<Component>> SearchComponentsAsync(ClaimsPrincipal principal, long projectId, string text, int offset, int count);
    Task<IReadOnlyList<Component>> GetComponentsAsync(ClaimsPrincipal principal, long projectId);
    Task<Component?> GetComponentByNameAsync(ClaimsPrincipal principal, long projectId, string name);

    #endregion Components
}
