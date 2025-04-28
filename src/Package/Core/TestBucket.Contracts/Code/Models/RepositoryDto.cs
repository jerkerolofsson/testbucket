namespace TestBucket.Contracts.Code.Models;

public class RepositoryDto
{
    /// <summary>
    /// URL
    /// </summary>
    public required string Url { get; set; }

    /// <summary>
    /// External ID
    /// </summary>
    public string? ExternalId { get; set; }
}
