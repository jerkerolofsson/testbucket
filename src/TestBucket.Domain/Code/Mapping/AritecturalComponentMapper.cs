using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Domain.Code.Models;

namespace TestBucket.Domain.Code.Mapping;
public static class AritecturalComponentMapper
{
    public static AritecturalComponentDto ToDto(this Feature feature)
    {
        return new AritecturalComponentDto
        {
            Id = feature.Id,
            Name = feature.Name,
            Description = feature.Description ?? string.Empty,
            DevelopmentLead = feature.DevLead,
            TestLead = feature.TestLead,
            GlobPatterns = feature.GlobPatterns?.ToList() ?? [],
        };
    }
    public static Feature ToFeature(this AritecturalComponentDto feature)
    {
        return new Feature
        {
            Id = feature.Id,
            Name = feature.Name,
            Description = feature.Description,
            DevLead = feature.DevelopmentLead,
            TestLead = feature.TestLead,
            GlobPatterns = feature.GlobPatterns?.ToList() ?? [],
        };
    }
}
