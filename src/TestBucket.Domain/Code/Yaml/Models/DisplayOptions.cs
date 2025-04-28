namespace TestBucket.Domain.Code.Yaml.Models;

public class DisplayOptions
{
    /// <summary>
    /// Sort order within parent
    /// </summary>
    public int? Sort { get; set; }

    /// <summary>
    /// Background color
    /// </summary>
    public string? BackColor { get; set; }

    /// <summary>
    /// Foreground color
    /// </summary>
    public string? ForeColor { get; set; }
}
