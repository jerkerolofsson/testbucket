using System.Text.Json;

using TestBucket.Contracts.Requirements;

using TestBucket.Domain.Export.Models;

namespace TestBucket.Domain.Export.Handlers.Requirements;
public class RequirementSerialization
{
    public static async Task<RequirementEntityDto?> DeserializeAsync(ExportEntity exportedEntity)
    {
        if (exportedEntity.Type == "requirement-specification")
        {
            return await DeserializeSpecificationAsync(exportedEntity);
        }
        if (exportedEntity.Type == "requirement")
        {
            return await DeserializeRequirementAsync(exportedEntity);
        }
        return null;
    }

    public static async Task<RequirementDto?> DeserializeRequirementAsync(ExportEntity exportedEntity)
    {
        using var stream = exportedEntity.Open();
        return await JsonSerializer.DeserializeAsync<RequirementDto>(stream);
    }

    public static async Task<RequirementSpecificationDto?> DeserializeSpecificationAsync(ExportEntity exportedEntity)
    {
        using var stream = exportedEntity.Open();
        return await JsonSerializer.DeserializeAsync<RequirementSpecificationDto>(stream);
    }
}
