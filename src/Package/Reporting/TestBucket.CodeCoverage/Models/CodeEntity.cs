namespace TestBucket.CodeCoverage.Models;
public abstract record class CodeEntity
{
    public abstract string GetName();
    public abstract IReadOnlyList<CodeEntity> GetChildren();

    public abstract Lazy<int> LineCount { get; }
    public abstract Lazy<int> CoveredLineCount { get; }

    /// <summary>
    /// Line Coverage in % between 0.0 and 100.0
    /// </summary>
    public virtual Lazy<double> CoveragePercent
    {
        get
        {
            return new Lazy<double>(() =>
            {
                if (LineCount.Value == 0)
                {
                    return 0.0;
                }
                return Math.Round((double)CoveredLineCount.Value / LineCount.Value * 100, 2);
            });
        }
    }
}
