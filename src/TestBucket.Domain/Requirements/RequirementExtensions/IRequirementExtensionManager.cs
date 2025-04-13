using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Contracts.Integrations;
using TestBucket.Contracts.Requirements;

namespace TestBucket.Domain.Requirements.RequirementExtensions;
public interface IRequirementExtensionManager
{
    /// <summary>
    /// Returns all requirement provider extensions
    /// </summary>
    /// <returns></returns>
    IReadOnlyList<IExternalRequirementProvider> ExternalProviders { get; }
}
