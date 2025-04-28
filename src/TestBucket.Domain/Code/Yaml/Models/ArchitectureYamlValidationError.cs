using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestBucket.Domain.Code.Yaml.Models;
public class ArchitectureYamlValidationError
{
    public long? Line { get; set; }
    public long? Column { get; set; }
    public required string Message { get; set; }
}
