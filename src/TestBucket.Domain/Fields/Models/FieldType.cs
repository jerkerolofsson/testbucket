using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestBucket.Domain.Fields.Models;

/// <summary>
/// Type of field
/// </summary>
public enum FieldType
{
    String = 0,
    Integer = 1,
    Double = 2,
    Boolean = 3,
    DateTimeOffset = 4,

    SingleSelection = 5,
}
