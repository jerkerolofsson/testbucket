using TestBucket.Components.Requirements;
using TestBucket.Components.Tests.Services;
using TestBucket.Domain.Features.Traceability.Models;

namespace TestBucket.Components.Features.Traceability.Graph;

public record class NodeDto
{
    public required IconDto Icon { get; set; }
    public required string Name { get; set; }
    public required string Id { get; set; }
    public required string Type { get; set; }
    public required string[] References { get; set; }

    public static NodeDto Create(TraceabilityNode node, TraceabilityNode? parent)
    {
        string icon = "";
        if (node.Requirement is not null)
        {
            icon = RequirementIcons.GetIcon(node.Requirement);
        }
        else if (node.TestCase is not null)
        {
            icon = TestIcons.GetIcon(node.TestCase);
        }

        var type = node.Requirement is not null ? "Requirement" : "TestCase";
        var dto = new NodeDto
        {
            Icon = new IconDto { Svg = icon, Color = "white", Tooltip = node.Name},
            Name = node.Name,
            Id = GetId(node),
            Type = type,
            References =
            [
                //..node.Upstream.Select(x => GetId(x)),
                ..node.Downstream.Select(x => GetId(x)),
            ]
        };
        if(parent is not null)
        {
            //dto.References = [.. dto.References, GetId(parent)];
        }
        return dto;
    }

    private static string GetId(TraceabilityNode node)
    {
        if (node.TestCase is not null)
        {
            return "T" + node.TestCase.Id;
        }
        if (node.Requirement  is not null)
        {
            return "R" + node.Requirement.Id;
        }
        return "invalid";
    }
}
