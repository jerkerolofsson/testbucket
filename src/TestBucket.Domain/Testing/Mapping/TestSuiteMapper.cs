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
        };
    }
}
