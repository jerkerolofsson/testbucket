using TestBucket.Contracts.Testing.States;
using TestBucket.Domain.Export.Handlers;
using TestBucket.Domain.Testing.Models;
using TestBucket.Formats.Dtos;

namespace TestBucket.Domain.Testing.Mapping;

/// <summary>
/// Provides extension methods for mapping between TestCase, TestCaseDto, TestStep, and TestStepDto.
/// </summary>
public static class TestCaseMapping
{
    /// <summary>
    /// Maps a <see cref="TestStepDto"/> to a <see cref="TestStep"/> database entity.
    /// </summary>
    /// <param name="dto">The source <see cref="TestStepDto"/>.</param>
    /// <param name="testCaseId">The ID of the parent test case.</param>
    /// <returns>A new <see cref="TestStep"/> entity.</returns>
    public static TestStep ToDbo(this TestStepDto dto, long testCaseId)
    {
        return new TestStep
        {
            Description = dto.Description,
            ExpectedResult = dto.ExpectedResult,
            TestCaseId = testCaseId
        };
    }

    /// <summary>
    /// Maps a <see cref="TestStep"/> database entity to a <see cref="TestStepDto"/>.
    /// </summary>
    /// <param name="step">The source <see cref="TestStep"/> entity.</param>
    /// <returns>A new <see cref="TestStepDto"/>.</returns>
    public static TestStepDto ToDto(this TestStep step)
    {
        return new TestStepDto
        {
            Description = step.Description,
            ExpectedResult = step.ExpectedResult
        };
    }

    /// <summary>
    /// Maps a <see cref="TestCaseDto"/> to a <see cref="TestCase"/> database entity.
    /// </summary>
    /// <param name="item">The source <see cref="TestCaseDto"/>.</param>
    /// <returns>A new <see cref="TestCase"/> entity.</returns>
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
            ExecutionType = item.ExecutionType,
            ScriptType = item.ScriptType,
            Preconditions = item.Preconditions,
            Postconditions = item.Postconditions,
            TestSteps = item.Steps?.Select(s => s.ToDbo(item.Id)).ToList() ?? new List<TestStep>(),
            Comments = CommentSerializer.Deserialize(item.Comments),
            RunnerLanguage = item.RunnerLanguage,

        };
        return dto;
    }

    /// <summary>
    /// Maps a <see cref="TestCase"/> database entity to a <see cref="TestCaseDto"/>.
    /// </summary>
    /// <param name="item">The source <see cref="TestCase"/> entity.</param>
    /// <returns>A new <see cref="TestCaseDto"/>.</returns>
    /// <exception cref="InvalidOperationException">Thrown if <see cref="TestCase.TenantId"/> is null.</exception>
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
            Traits = new TestTraitCollection(),
            ExecutionType = item.ExecutionType,
            ScriptType = item.ScriptType,
            Preconditions = item.Preconditions,
            Postconditions = item.Postconditions,
            Steps = item.TestSteps?.Select(s => s.ToDto()).ToList() ?? new List<TestStepDto>(),
            Comments = CommentSerializer.Serialize(item.Comments),
            RunnerLanguage = item.RunnerLanguage,

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