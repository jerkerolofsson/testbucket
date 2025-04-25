using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Domain.Testing.Models;

namespace TestBucket.Domain.Testing.Mapping;
public static class TestCaseMapping
{
    public static TestCase ToDbo(this TestCaseDto item)
    {
        var dto = new TestCase
        {
            Id = item.Id,
            Created = item.Created,
            Name = item.TestCaseName,
            TenantId = item.TenantId,
            Description = item.Description,
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
            TestCaseName = item.Name,
            TenantId = item.TenantId ?? throw new InvalidOperationException("Missing tenant Id"),
            Description = item.Description,
            Slug = item.Slug,
            Traits = new Formats.Dtos.TestTraitCollection(),
            ExecutionType = item.ExecutionType,
        };

        dto.Traits.ExternalId = item.ExternalId;

        return dto;
    }
}
