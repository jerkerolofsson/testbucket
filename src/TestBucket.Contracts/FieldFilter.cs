using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestBucket.Contracts;
public record class FieldFilter
{
    public required long FilterDefinitionId { get; set; }

    /// <summary>
    /// The field is a boolean
    /// </summary>
    public bool? BooleanValue { get; set; }

    /// <summary>
    /// The field is an integer
    /// </summary>
    public long? LongValue { get; set; }

    /// <summary>
    /// The field is a double
    /// </summary>
    public double? DoubleValue { get; set; }

    /// <summary>
    /// The field has is a single string
    /// </summary>
    public string? StringValue { get; set; }

    /// <summary>
    /// The field has multiple strings
    /// </summary>
    public List<string>? StringValuesList { get; set; }
}
