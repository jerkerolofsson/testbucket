using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using YamlDotNet.Serialization.NamingConventions;
using YamlDotNet.Serialization;
using TestBucket.Domain.Code.Yaml.Models;
using YamlDotNet.Core;

namespace TestBucket.Domain.Code.Yaml;
public class ArchitectureYamlSerializer
{
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
        catch(YamlException ex)
        {
            errors.Add(new ArchitectureYamlValidationError() { Message = ex.Message, Column = ex.Start.Column, Line = ex.Start.Line });
        }
        return errors;
    }

    public string Serialize(ProjectArchitectureModel model)
    {
        var serializer = new SerializerBuilder().WithNamingConvention(UnderscoredNamingConvention.Instance).Build();
        return serializer.Serialize(model);
    }
    public ProjectArchitectureModel Parse(string yaml)
    {
        if(Validate(yaml).Count() > 0)
        {
            throw new ArgumentException("Invalid YAML");
        }

        var deserializer = new DeserializerBuilder().WithNamingConvention(UnderscoredNamingConvention.Instance).Build();
        return deserializer.Deserialize<ProjectArchitectureModel>(yaml);
    }
}
