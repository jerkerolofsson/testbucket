using TestBucket.Domain.Fields.Mapping;

using Xunit;

namespace TestBucket.Domain.UnitTests.Fields;

/// <summary>
/// Contains unit tests for the <see cref="FieldDefinition"/> mapping functionality,
/// specifically verifying the behavior of the <c>ToDto</c> extension method.
/// </summary>
[Feature("Fields")]
[UnitTest]
[Component("Fields")]
[EnrichedTest]
[FunctionalTest]
public class FieldDefinitionMapperTests
{
    /// <summary>
    /// Verifies that <c>ToDto</c> correctly maps all properties from a <see cref="FieldDefinition"/>
    /// to its corresponding DTO, including all value and reference types.
    /// </summary>
    [Fact]
    public void ToDto_MapsAllPropertiesCorrectly()
    {
        // Arrange
        var now = DateTimeOffset.UtcNow;
        var field = new FieldDefinition
        {
            Name = "Priority",
            Trait = "priority",
            TraitType = Traits.Core.TraitType.TestPriority,
            Target = Contracts.Fields.FieldTarget.TestCase,
            Created = now,
            CreatedBy = "user1",
            Modified = now.AddMinutes(5),
            ModifiedBy = "user2",
            Description = "The priority of the test case",
            Icon = "star",
            OptionIcons = new Dictionary<string, string> { { "High", "fire" }, { "Low", "leaf" } },
            Options = new List<string> { "High", "Medium", "Low" },
            Type = FieldType.String,
            Inherit = true,
            UseClassifier = false,
            WriteOnly = false,
            ReadOnly = true
        };
        var projectSlug = "myproject";

        // Act
        var dto = field.ToDto(projectSlug);

        // Assert
        Assert.Equal(projectSlug, dto.ProjectSlug);
        Assert.Equal(field.Name, dto.Name);
        Assert.Equal(field.Trait, dto.Trait);
        Assert.Equal(field.TraitType, dto.TraitType);
        Assert.Equal(field.Target, dto.Target);
        Assert.Equal(field.Created, dto.Created);
        Assert.Equal(field.CreatedBy, dto.CreatedBy);
        Assert.Equal(field.Modified, dto.Modified);
        Assert.Equal(field.ModifiedBy, dto.ModifiedBy);
        Assert.Equal(field.Description, dto.Description);
        Assert.Equal(field.Icon, dto.Icon);
        Assert.Equal(field.OptionIcons, dto.OptionIcons);
        Assert.Equal(field.Options, dto.Options);
        Assert.Equal(field.Type, dto.Type);
        Assert.Equal(field.Inherit, dto.Inherit);
        Assert.Equal(field.UseClassifier, dto.UseClassifier);
        Assert.Equal(field.WriteOnly, dto.WriteOnly);
        Assert.Equal(field.ReadOnly, dto.ReadOnly);
    }

    /// <summary>
    /// Verifies that <c>ToDto</c> correctly handles <c>null</c> values for collection and reference type properties.
    /// </summary>
    [Fact]
    public void ToDto_NullableCollectionsHandled()
    {
        // Arrange
        var field = new FieldDefinition
        {
            Name = "Status",
            Trait = "status",
            TraitType = Traits.Core.TraitType.TestPriority,
            Target = Contracts.Fields.FieldTarget.TestCase,
            Created = DateTimeOffset.UtcNow,
            CreatedBy = "user",
            Modified = DateTimeOffset.UtcNow,
            ModifiedBy = "user",
            Description = "Status field",
            Icon = null,
            OptionIcons = null,
            Options = null,
            Type = FieldType.String,
            Inherit = false,
            UseClassifier = false,
            WriteOnly = false,
            ReadOnly = false
        };

        // Act
        var dto = field.ToDto("slug");

        // Assert
        Assert.Null(dto.Icon);
        Assert.Null(dto.OptionIcons);
        Assert.Null(dto.Options);
    }
}