
namespace TestBucket.Contracts.Code.Models;
public class CommitDto
{
    /// <summary>
    /// The URL associated with this reference.
    /// </summary>
    public string? Url { get; set; }

    /// <summary>
    /// The reference label.
    /// </summary>
    public string? Label { get; set; }

    /// <summary>
    /// The reference identifier.
    /// </summary>
    public required string Ref { get; set; }

    /// <summary>
    /// The sha value of the reference.
    /// </summary>
    public required string Sha { get; set; }

    /// <summary>
    /// Commmit message
    /// </summary>
    public string? Message { get; set; }

    public IReadOnlyList<CommitFileDto>? Files { get; set; }
}
