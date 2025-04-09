using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestBucket.Contracts.Testing.Models;

/// <summary>
/// A numerical result / metric output from the test 
/// </summary>
public class NumericalResultDto
{
    /// <summary>
    /// Identifier for the metric
    /// snake-case recommended
    /// </summary>
    public required string Id { get; set; }

    /// <summary>
    /// Value for the metric. Base unit recommended (e.g bytes, not kilobytes)
    /// </summary>
    public double Value { get; set; }

    /// <summary>
    /// Unit of the Value, e.g. B for bytes, or s for seconds.
    /// SI units recommended.
    /// </summary>
    public string? Unit { get; set; }
}
