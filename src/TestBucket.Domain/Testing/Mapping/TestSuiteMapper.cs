using TestBucket.Domain.Export.Handlers;
using TestBucket.Domain.Requirements.Models;
using TestBucket.Domain.Testing.Models;
using TestBucket.Formats.Dtos;

namespace TestBucket.Domain.Testing.Mapping;

public static class TestSuiteMapper
{
    public static TestSuite ToDbo(this TestSuiteDto testSuite)
    {
        return new TestSuite
        {
            Name = testSuite.Name,
            Description = testSuite.Description,
            Id = testSuite.Id,
            Variables = testSuite.Variables,
            CiCdSystem = testSuite.CiCdSystem,
            DefaultCiCdRef = testSuite.DefaultCiCdRef,
            ExternalSystemId = testSuite.ExternalSystemId,
            Slug = testSuite.Slug,
            Icon = testSuite.Icon,
            Color = testSuite.Color,
            Dependencies = testSuite.Dependencies?.ToList(),
            AddPipelinesStartedFromOutside = testSuite.AddPipelinesStartedFromOutside,
            Comments = CommentSerializer.Deserialize(testSuite.Comments),
        };
    }

    public static TestSuiteDto ToDto(this TestSuite testSuite)
    {
        return new TestSuiteDto
        {
            Name = testSuite.Name,
            Description = testSuite.Description,
            Id = testSuite.Id,
            Variables = testSuite.Variables,
            CiCdSystem = testSuite.CiCdSystem,
            DefaultCiCdRef = testSuite.DefaultCiCdRef,
            ExternalSystemId = testSuite.ExternalSystemId,
            Slug = testSuite.Slug,
            Icon = testSuite.Icon,
            Color = testSuite.Color,
            Dependencies = testSuite.Dependencies?.ToList(),
            Comments = CommentSerializer.Serialize(testSuite.Comments),
            AddPipelinesStartedFromOutside = testSuite.AddPipelinesStartedFromOutside
        };
    }
}
