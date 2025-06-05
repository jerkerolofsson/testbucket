namespace TestBucket.CodeCoverage.Models;

/// <summary>
/// Represents a code coverage condition, such as a branch or logical condition, within a code entity.
/// </summary>
public record class CodeCoverageCondition : CodeEntity
{
    /// <summary>
    /// Gets or sets the unique number identifying this condition within its context.
    /// </summary>
    public int Number { get; set; }

    /// <summary>
    /// Gets or sets the type of the condition (e.g., branch, switch, etc.).
    /// </summary>
    public string? Type { get; set; }

    /// <summary>
    /// Gets or sets the coverage value for the condition, between 0.0 (not covered) and 1.0 (fully covered).
    /// </summary>
    public double Coverage { get; set; }

    /// <inheritdoc />
    public override Lazy<int> LineCount => new Lazy<int>(() => 1);

    /// <inheritdoc />
    public override Lazy<int> CoveredLineCount => new Lazy<int>(() => Coverage > 0 ? 1 : 0);

    /// <summary>
    /// Gets the display name for this condition.
    /// </summary>
    /// <returns>A string representing the name of the condition.</returns>
    public override string GetName() => $"Condition {Number}";

    /// <summary>
    /// Gets the child code entities of this condition. Always returns an empty list.
    /// </summary>
    /// <returns>An empty list of <see cref="CodeEntity"/>.</returns>
    public override IReadOnlyList<CodeEntity> GetChildren() => [];

    /// <summary>
    /// Gets the coverage percentage for this condition, rounded to two decimal places.
    /// </summary>
    public override Lazy<double> CoveragePercent
    {
        get
        {
            return new Lazy<double>(() => Math.Round(Coverage * 100.0, 2));
        }
    }
}