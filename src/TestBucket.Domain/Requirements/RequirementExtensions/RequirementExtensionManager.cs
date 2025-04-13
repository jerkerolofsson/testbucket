using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Contracts.Integrations;

namespace TestBucket.Domain.Requirements.RequirementExtensions;
internal class RequirementExtensionManager : IRequirementExtensionManager
{
    private readonly List<IExternalRequirementProvider> _providers;

    public IReadOnlyList<IExternalRequirementProvider> ExternalProviders => _providers;

    public RequirementExtensionManager(IEnumerable<IExternalRequirementProvider> providers)
    {
        _providers = providers.ToList();
    }
}
