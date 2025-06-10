using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestBucket.Contracts.Fields;
public class FieldDto
{
    public TraitType? TraitType { get; set; }
    public required string Name { get; set; }
    public string? Value { get; set; }
}
