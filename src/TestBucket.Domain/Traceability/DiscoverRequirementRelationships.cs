using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Mediator;

using TestBucket.Domain.Requirements;
using TestBucket.Domain.Requirements.Models;
using TestBucket.Domain.Requirements.Specifications;
using TestBucket.Domain.Requirements.Specifications.Links;
using TestBucket.Domain.Requirements.Specifications.Requirements;
using TestBucket.Domain.Shared.Specifications;
using TestBucket.Domain.Traceability.Models;

namespace TestBucket.Domain.Traceability;
public record class DiscoverRequirementRelationshipsRequest(ClaimsPrincipal Principal, Requirement Requirement, int Depth) : IRequest<TraceabilityNode>;

public class DiscoverRequirementRelationshipsHandler : IRequestHandler<DiscoverRequirementRelationshipsRequest, TraceabilityNode>
{
    private readonly IRequirementRepository _requirementRepository;
    private readonly ITestCaseRepository _testCaseRepository;

    public DiscoverRequirementRelationshipsHandler(IRequirementRepository requirementRepository, ITestCaseRepository testCaseRepository)
    {
        _requirementRepository = requirementRepository;
        _testCaseRepository = testCaseRepository;
    }

    public async ValueTask<TraceabilityNode> Handle(DiscoverRequirementRelationshipsRequest request, CancellationToken cancellationToken)
    {
        request.Principal.ThrowIfNoPermission(PermissionEntityType.Requirement, PermissionLevel.Read);
        var tenantId = request.Principal.GetTenantIdOrThrow();

        var root = new TraceabilityNode { Requirement = request.Requirement };

        TraceabilityNode node = root;
        int depth = request.Depth;

        await ProcessBranchAsync(tenantId, node, depth, upstream: true, downstream: true);

        return root;
    }

    private async Task ProcessBranchAsync(string tenantId, TraceabilityNode node, int depth, bool upstream, bool downstream)
    {
        if (node.Requirement is not null)
        {
            if (upstream && node.Requirement.ParentRequirementId is not null)
            {
                FilterSpecification<Requirement>[] parentFilters = [
                    new FilterByTenant<Requirement>(tenantId),
                    new FilterRequirementById(node.Requirement.ParentRequirementId.Value)
                    ];
                var parentResult = await _requirementRepository.SearchRequirementsAsync(parentFilters, 0, 1);
                foreach (var parent in parentResult.Items)
                {
                    var upstreamNode = new TraceabilityNode { Requirement = parent };
                    node.Upstream.Add(upstreamNode);
                    if (depth > 0)
                    {
                        await ProcessBranchAsync(tenantId, upstreamNode, depth - 1, upstream: true, downstream: false);
                    }
                }
            }

            if (downstream)
            {
                // Find child requirements
                FilterSpecification<Requirement>[] childFilters = [new FilterByTenant<Requirement>(tenantId), new FilterRequirementByParentId(node.Requirement.Id)];

                var childResult = await _requirementRepository.SearchRequirementsAsync(childFilters, 0, 100);
                foreach (var child in childResult.Items)
                {
                    var downstreamNode = new TraceabilityNode { Requirement = child };
                    node.Downstream.Add(downstreamNode);
                    if (depth > 0)
                    {
                        await ProcessBranchAsync(tenantId, downstreamNode, depth - 1, upstream: false, downstream: true);
                    }
                }

                // Find tests
                FilterSpecification<RequirementTestLink>[] linkFilters = [new FilterByTenant<RequirementTestLink>(tenantId), new FilterRequirementTestLinkByRequirement(node.Requirement.Id)];
                var testLinks = await _requirementRepository.SearchRequirementLinksAsync(linkFilters);
                foreach(var testLink in testLinks)
                {
                    //var testCase = await _testCaseRepository.GetTestCaseByIdAsync(tenantId, testLink.TestCase);
                    var downstreamNode = new TraceabilityNode { TestCase = testLink.TestCase };
                    node.Downstream.Add(downstreamNode);
                }
            }
        }
    }
}
