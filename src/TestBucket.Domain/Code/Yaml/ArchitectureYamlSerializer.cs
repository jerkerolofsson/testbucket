using TestBucket.Domain.Code.Yaml.Models;

using YamlDotNet.Core;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace TestBucket.Domain.Code.Yaml;

/// <summary>
/// Provides methods for serializing, deserializing, and validating project architecture models using YAML format.
/// </summary>
public class ArchitectureYamlSerializer
{
    /// <summary>
    /// Validates the provided YAML string and returns a list of validation errors, if any.
    /// </summary>
    /// <param name="yaml">The YAML string representing a <see cref="ProjectArchitectureModel"/>.</param>
    /// <returns>
    /// A list of <see cref="ArchitectureYamlValidationError"/> objects describing any issues found in the YAML.
    /// Returns an empty list if the YAML is valid.
    /// </returns>
    public List<ArchitectureYamlValidationError> Validate(string yaml)
    {
        List<ArchitectureYamlValidationError> errors = new();
        try
        {
            var deserializer = new DeserializerBuilder().WithNamingConvention(UnderscoredNamingConvention.Instance).Build();
            var model = deserializer.Deserialize<ProjectArchitectureModel>(yaml);
            if (model is null)
            {
                errors.Add(new ArchitectureYamlValidationError() { Message = "Definition must contain atleast one system" });
            }
            else
            {
                if (model.Systems is null || model.Systems.Count == 0)
                {
                    errors.Add(new ArchitectureYamlValidationError() { Message = "Definition must contain atleast one system" });
                }
            }
        }
        catch (YamlException ex)
        {
            errors.Add(new ArchitectureYamlValidationError() { Message = ex.Message, Column = ex.Start.Column, Line = ex.Start.Line });
        }
        return errors;
    }

    /// <summary>
    /// Serializes the specified <see cref="ProjectArchitectureModel"/> to a YAML string.
    /// </summary>
    /// <param name="model">The <see cref="ProjectArchitectureModel"/> to serialize.</param>
    /// <returns>A YAML string representation of the model.</returns>
    public string Serialize(ProjectArchitectureModel model)
    {
        var serializer = new SerializerBuilder().WithNamingConvention(UnderscoredNamingConvention.Instance).Build();
        return serializer.Serialize(model);
    }

    /// <summary>
    /// Parses the provided YAML string into a <see cref="ProjectArchitectureModel"/>.
    /// Throws an <see cref="ArgumentException"/> if the YAML is invalid.
    /// </summary>
    /// <param name="yaml">The YAML string to parse.</param>
    /// <returns>The deserialized <see cref="ProjectArchitectureModel"/>.</returns>
    /// <exception cref="ArgumentException">Thrown if the YAML is invalid.</exception>
    public ProjectArchitectureModel Parse(string yaml)
    {
        if (Validate(yaml).Count() > 0)
        {
            throw new ArgumentException("Invalid YAML");
        }

        var deserializer = new DeserializerBuilder().WithNamingConvention(UnderscoredNamingConvention.Instance).Build();
        return deserializer.Deserialize<ProjectArchitectureModel>(yaml);
    }
}