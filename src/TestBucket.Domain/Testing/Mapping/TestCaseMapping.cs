using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Contracts.Fields;
using TestBucket.Domain.Fields.Mapping;
using TestBucket.Domain.Testing.Models;
using TestBucket.Formats.Dtos;

namespace TestBucket.Domain.Testing.Mapping;
public static class TestCaseMapping
{
    public static TestCase ToDbo(this TestCaseDto item)
    {
        var dto = new TestCase
        {
            Id = item.Id,
            Created = item.Created,
            Path = item.Path ?? "",
            Name = item.TestCaseName,
            TenantId = item.TenantId,
            Description = item.Description,
            ExternalDisplayId = item.ExternalDisplayId,
            Slug = item.Slug,
            ExternalId = item.Traits?.ExternalId,
            ExecutionType = item.ExecutionType
        };
        return dto;
    }

    public static TestCaseDto ToDto(this TestCase item)
    {
        var dto = new TestCaseDto
        {
            Id = item.Id,
            Created = item.Created,
            Path = item.Path,
            TestCaseName = item.Name,
            ExternalDisplayId = item.ExternalDisplayId,
            TenantId = item.TenantId ?? throw new InvalidOperationException("Missing tenant Id"),
            Description = item.Description,
            Slug = item.Slug,
            Traits = new Formats.Dtos.TestTraitCollection(),
            ExecutionType = item.ExecutionType,
        };

        dto.Traits.ExternalId = item.ExternalId;

        // Serialize fields as traits
        if (item.TestCaseFields is not null)
        {
            foreach (var field in item.TestCaseFields)
            {
                if (field.FieldDefinition is null)
                {
                    continue; // Skip fields without a definition
                }

                var trait = new TestTrait
                {
                    Name = field.FieldDefinition.Name,
                    Type = field.FieldDefinition.TraitType,
                    Value = field.GetValueAsString()
                };
                dto.Traits.Traits.Add(trait);
            }
        }

        return dto;
    }
}
