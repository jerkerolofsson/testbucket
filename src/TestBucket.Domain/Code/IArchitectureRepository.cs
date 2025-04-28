using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Domain.Code.Models;
using TestBucket.Domain.Shared.Specifications;

namespace TestBucket.Domain.Code;

/// <summary>
/// Persistance storage for code architecture components
/// </summary>
public interface IArchitectureRepository
{
    #region Systems
    Task AddSystemAsync(ProductSystem system);
    Task<PagedResult<ProductSystem>> SearchSystemsAsync(FilterSpecification<ProductSystem>[] filters, int offset, int count);
    Task UpdateSystemAsync(ProductSystem system);
    #endregion Systems

    #region Layer
    Task AddLayerAsync(ArchitecturalLayer component);
    Task<PagedResult<ArchitecturalLayer>> SearchLayersAsync(FilterSpecification<ArchitecturalLayer>[] filters, int offset, int count);
    Task UpdateLayerAsync(ArchitecturalLayer component);
    #endregion Layer

    #region Components
    Task AddComponentAsync(Component component);
    Task<PagedResult<Component>> SearchComponentsAsync(FilterSpecification<Component>[] filters, int offset, int count);
    Task UpdateComponentAsync(Component component);
    #endregion Components

    #region Features
    Task AddFeatureAsync(Feature feature);
    Task<PagedResult<Feature>> SearchFeaturesAsync(FilterSpecification<Feature>[] filters, int offset, int count);
    Task UpdateFeatureAsync(Feature feature);
    #endregion Features
}
