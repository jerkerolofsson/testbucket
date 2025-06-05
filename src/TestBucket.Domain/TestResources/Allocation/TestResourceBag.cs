using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Domain.Identity;
using TestBucket.Domain.TestResources.Models;

namespace TestBucket.Domain.TestResources.Allocation;

/// <summary>
/// This contains resources resources which are unlocked when the test completes
/// </summary>
public class TestResourceBag
{
    private readonly List<TestResource> _resources = [];
    private readonly ClaimsPrincipal _principal;
    private readonly ITestResourceManager _manager;

    /// <summary>
    /// Gets allocated resources
    /// </summary>
    public IReadOnlyList<TestResource> Resources => _resources;

    public TestResourceBag(ClaimsPrincipal principal, ITestResourceManager manager)
    {
        _principal = principal;
        _manager = manager;
    }

    public async ValueTask AddAsync(TestResource resource, DateTimeOffset lockExpires, string lockOwner)
    {
        _resources.Add(resource);
        resource.Locked = true;
        resource.LockOwner = lockOwner;
        resource.LockExpires = lockExpires;
        await _manager.UpdateAsync(_principal, resource);
    }

    /// <summary>
    /// Assigns environment variables
    /// </summary>
    /// <param name="variables"></param>
    public void ResolveVariables(TestResource resource, string requestedType, Dictionary<string, string> variables)
    {
        foreach(var type in resource.Types)
        {
            var resourcesByType = _resources.Where(x => x.Types.Contains(type)).ToList();
            var index = resourcesByType.IndexOf(resource);

            var key = $"resources__{type}__{index}";

            foreach(var attribute in resource.Variables)
            {
                var fullName = key + "__" + attribute.Key;
                variables[fullName] = attribute.Value;  
            }
        }
    }
}
