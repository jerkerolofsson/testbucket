using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Mediator;

using TestBucket.Domain.Shared.Specifications;
using TestBucket.Domain.Testing.Models;
using TestBucket.Domain.TestResources.Models;
using TestBucket.Domain.TestResources.Specifications;

namespace TestBucket.Domain.TestResources.Allocation;

/// <summary>
/// Collects resources required to run a test case and returns a "bag" that has those resources
/// </summary>
public class TestResourceDependencyAllocator
{
    private readonly ITestResourceManager _testResourceManager;
    private readonly ITestResourceRepository _resourceRepository;
    private readonly IMediator _mediator;
    private static readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1);

    public TestResourceDependencyAllocator(ITestResourceManager testResourceManager, ITestResourceRepository resourceRepository, IMediator mediator)
    {
        _testResourceManager = testResourceManager;
        _resourceRepository = resourceRepository;
        _mediator = mediator;
    }

    public async Task<TestResourceBag> CollectDependenciesAsync(
        ClaimsPrincipal principal, 
        TestExecutionContext context,
        CancellationToken cancellationToken = default)
    {
        var bag = new TestResourceBag(principal, _testResourceManager);

        await _semaphore.WaitAsync(cancellationToken);
        try
        {
            if (context.Dependencies is not null)
            {
                foreach (var dependency in context.Dependencies)
                {
                    if (dependency.ResourceType is not null)
                    {
                        TestResource? testResource = await AllocateResourceAsync(principal, dependency.ResourceType);
                        if(testResource is null)
                        {
                            context.CompilerErrors.Add(new CompilerError 
                            { 
                                Code = 123,
                                Message = $"Failed to allocate resource of type {dependency.ResourceType}",
                                Line = -1,
                                Column = -1,
                            });
                        }
                        else
                        {
                            await bag.AddAsync(testResource, context.ResourceExpiry, context.Guid);
                        }
                    }
                }
            }
        }
        catch
        {
            // Release any resources already allocated
            await _mediator.Send(new ReleaseResourcesRequest(context.Guid, principal.GetTenantIdOrThrow()));
        }
        finally
        {
            _semaphore.Release();
        }

        return bag;
    }

    private async Task<TestResource?> AllocateResourceAsync(ClaimsPrincipal principal, string resourceType)
    {
        FilterSpecification<TestResource>[] filters = [
            new FindUnlockedResource(),
            new FindResourceByType(resourceType),
            new FilterByTenant<TestResource>(principal.GetTenantIdOrThrow())
        ];

        var page = await _resourceRepository.SearchAsync(filters, 0, 1);
        if(page.Items.Length > 0)
        {
            return page.Items[0];
        }
        return null;
    }
}
