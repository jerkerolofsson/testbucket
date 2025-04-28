using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static TestBucket.Contracts.Code.Models.CommitFileDto;

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
    public string? Ref { get; set; }

    /// <summary>
    /// The sha value of the reference.
    /// </summary>
    public string? Sha { get; set; }

    public string? Message { get; set; }
    public IReadOnlyList<CommitFileDto> Files { get; set; } = [];
}
