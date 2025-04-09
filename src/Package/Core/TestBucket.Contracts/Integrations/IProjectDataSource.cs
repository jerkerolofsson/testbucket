using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Contracts.Projects;
using TestBucket.Traits.Core;

namespace TestBucket.Contracts.Integrations;

/// <summary>
/// A data source for external project information, such as getting information from GitHub, GitLab..
/// </summary>
public interface IProjectDataSource
{
    /// <summary>
    /// Returns the traits that are supported by the data source
    /// </summary>
    TraitType[] SupportedTraits { get; }


    /// <summary>
    /// Name matching the ExternalSystem record
    /// </summary>
    string SystemName { get; }

    /// <summary>
    /// Gets field values of the specified type
    /// </summary>
    /// <param name="system"></param>
    /// <param name="trait"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<string[]> GetFieldOptionsAsync(ExternalSystemDto system, TraitType trait, CancellationToken cancellationToken);
}
